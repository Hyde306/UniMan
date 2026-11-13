using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Yoshida.TestScene
{
    public enum PlayerInputType
    {
        Space,
        LeftShift,
        MouseLeft,
        MouseRight
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("移動")]
        [SerializeField, Min(0f)] private float moveSpeed = 5f;
        [SerializeField, Min(0f)] private float dashSpeed = 10f;
        [SerializeField] private int startDirection = 1;
        [SerializeField, Min(0f)] private float verticalRecoverySpeed = 4f;

        [Header("操作")]
        [SerializeField] private PlayerInputType directionKey = PlayerInputType.Space;
        [SerializeField] private PlayerInputType dashKey = PlayerInputType.LeftShift;

        [Header("プレイヤー同士の衝突")]
        [SerializeField, Min(0f)] private float knockbackForce = 10f;
        [SerializeField, Min(0f)] private float dashKnockbackForce = 20f;
        [SerializeField, Min(0f)] private float knockbackDuration = 0.2f;
        [SerializeField, Min(0f)] private float facingBonus = 0.2f;
        [SerializeField, Min(0f)] private float selfRecoil = 0.25f;
        [SerializeField, Min(0f)] private float minimumImpactSpeed = 0.5f;

        private Rigidbody2D body;
        private CircleCollider2D bodyCollider;
        private SpriteRenderer playerSprite;
        private bool rightFacingFlip;
        private PhysicsMaterial2D contactMaterial;
        private float laneY;
        private int moveDirection;
        private bool isMoving;
        private bool dashHeld;
        private readonly List<RaycastHit2D> obstacleHits = new List<RaycastHit2D>(8);
        private ContactFilter2D solidColliders;
        private Vector2 velocityBeforeStep;
        private int facingDirection;
        private double resolvedAt = double.NegativeInfinity;
        private float knockbackRemaining;
        private float knockbackElapsed;
        private float knockbackPeakSpeed;
        private Vector2 knockbackDirection;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<CircleCollider2D>();
            playerSprite = GetComponent<SpriteRenderer>();
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            contactMaterial = new PhysicsMaterial2D("Test Player Contact")
            {
                friction = 0f,
                bounciness = 0f
            };
            bodyCollider.sharedMaterial = contactMaterial;
            laneY = body.position.y;
            moveDirection = startDirection >= 0 ? 1 : -1;
            if (playerSprite != null)
            {
                rightFacingFlip = playerSprite.flipX ^ (moveDirection < 0);
            }

            FaceMoveDirection();
            solidColliders = new ContactFilter2D { useTriggers = false };
        }

        private void OnDestroy()
        {
            if (contactMaterial != null)
            {
                Destroy(contactMaterial);
            }
        }

        private void Update()
        {
            if (Time.timeScale == 0f)
            {
                return;
            }

            if (ReadInput(directionKey, true))
            {
                if (isMoving)
                {
                    moveDirection *= -1;
                }

                isMoving = true;
                if (knockbackRemaining <= 0f)
                {
                    FaceMoveDirection();
                }
            }

            dashHeld = ReadInput(dashKey, false);
        }

        private void FixedUpdate()
        {
            Vector2 velocity = GetMoveVelocity();
            if (knockbackRemaining > 0f)
            {
                float remaining = Mathf.Clamp01(knockbackRemaining / knockbackDuration);
                velocity = knockbackDirection * (knockbackPeakSpeed * Mathf.SmoothStep(0f, 1f, remaining));
                knockbackElapsed += Time.fixedDeltaTime;
                knockbackRemaining = knockbackDuration - knockbackElapsed;
            }
            else
            {
                FaceMoveDirection();
            }

            // 衝突の前に記録し、両者の押し返しに同じ時点の速度を使う。
            velocityBeforeStep = StopAtObstacles(velocity);
            body.linearVelocity = velocityBeforeStep;
        }

        private Vector2 GetMoveVelocity()
        {
            float speed = dashHeld ? dashSpeed : moveSpeed;
            float x = isMoving ? speed * moveDirection : 0f;
            float y = Mathf.Clamp(
                (laneY - body.position.y) / Time.fixedDeltaTime,
                -verticalRecoverySpeed, verticalRecoverySpeed);
            return new Vector2(x, y);
        }

        private Vector2 StopAtObstacles(Vector2 velocity)
        {
            if (Mathf.Abs(velocity.x) < 0.001f)
            {
                return velocity;
            }

            const float skin = 0.01f;
            float step = Time.fixedDeltaTime;
            Vector2 direction = velocity.x > 0f ? Vector2.right : Vector2.left;
            float travel = Mathf.Abs(velocity.x) * step;
            int hitCount = body.Cast(direction, solidColliders, obstacleHits, travel + skin);

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit2D hit = obstacleHits[i];
                if (hit.collider.CompareTag("GameOver") ||
                    (hit.rigidbody != null && hit.rigidbody.TryGetComponent<PlayerMove>(out _)))
                {
                    continue;
                }

                if (hit.distance <= skin)
                {
                    ColliderDistance2D separation = bodyCollider.Distance(hit.collider);
                    if (!separation.isValid || Vector2.Dot(direction, separation.normal) >= -0.01f)
                    {
                        continue;
                    }
                }
                else if (Vector2.Dot(direction, hit.normal) >= -0.01f)
                {
                    continue;
                }

                travel = Mathf.Min(travel, Mathf.Max(0f, hit.distance - skin));
            }

            velocity.x = direction.x * travel / step;
            return velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (Time.timeScale == 0f || collision.contactCount == 0 || collision.rigidbody == null ||
                !collision.rigidbody.TryGetComponent(out PlayerMove other) || !other.isActiveAndEnabled)
            {
                return;
            }

            double now = Time.fixedTimeAsDouble;
            if (resolvedAt == now && other.resolvedAt == now)
            {
                return;
            }

            Vector2 normal = collision.GetContact(0).normal;
            Vector2 awayFromOther = (Vector2)(bodyCollider.bounds.center - other.bodyCollider.bounds.center);
            if (Vector2.Dot(normal, awayFromOther) < 0f)
            {
                normal = -normal;
            }

            float myApproach = Vector2.Dot(velocityBeforeStep, -normal);
            float otherApproach = Vector2.Dot(other.velocityBeforeStep, normal);
            float closingSpeed = myApproach + otherApproach;
            if (closingSpeed <= Mathf.Max(minimumImpactSpeed, other.minimumImpactSpeed))
            {
                return;
            }

            float approachSpeed = Mathf.Max(0f, myApproach) + Mathf.Max(0f, otherApproach);
            float closingRatio = Mathf.Clamp01(closingSpeed / approachSpeed);
            float myImpact = GetImpactSpeed(myApproach, -normal) * closingRatio;
            float otherImpact = other.GetImpactSpeed(otherApproach, normal) * closingRatio;
            resolvedAt = now;
            other.resolvedAt = now;
            StartKnockback(normal, otherImpact + myImpact * selfRecoil);
            other.StartKnockback(-normal, myImpact + otherImpact * other.selfRecoil);
        }

        private float GetImpactSpeed(float approach, Vector2 towardOpponent)
        {
            if (approach <= 0f)
            {
                return 0f;
            }

            float fullSpeed = Mathf.Abs(velocityBeforeStep.x);
            float speedRatio = Mathf.InverseLerp(moveSpeed, Mathf.Max(moveSpeed + 0.01f, dashSpeed), fullSpeed);
            float baseImpact = Mathf.Lerp(knockbackForce, dashKnockbackForce, speedRatio);
            float alignment = Mathf.Clamp01(towardOpponent.x * facingDirection);
            float referenceSpeed = Mathf.Max(0.01f, Mathf.Max(fullSpeed, moveSpeed));
            return approach / referenceSpeed * baseImpact * (1f + alignment * facingBonus);
        }

        private void StartKnockback(Vector2 direction, float speed)
        {
            knockbackDirection = direction;
            knockbackPeakSpeed = speed;
            knockbackElapsed = 0f;
            knockbackRemaining = knockbackDuration;
        }

        private void FaceMoveDirection()
        {
            facingDirection = moveDirection;
            if (playerSprite != null)
            {
                playerSprite.flipX = rightFacingFlip ^ (facingDirection < 0);
            }
        }

        private static bool ReadInput(PlayerInputType input, bool pressedThisFrame)
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.Controls.ButtonControl button = input switch
            {
                PlayerInputType.Space => Keyboard.current?.spaceKey,
                PlayerInputType.LeftShift => Keyboard.current?.leftShiftKey,
                PlayerInputType.MouseLeft => Mouse.current?.leftButton,
                PlayerInputType.MouseRight => Mouse.current?.rightButton,
                _ => null
            };

            return button != null && (pressedThisFrame ? button.wasPressedThisFrame : button.isPressed);
#elif ENABLE_LEGACY_INPUT_MANAGER
            KeyCode key = input switch
            {
                PlayerInputType.Space => KeyCode.Space,
                PlayerInputType.LeftShift => KeyCode.LeftShift,
                PlayerInputType.MouseLeft => KeyCode.Mouse0,
                PlayerInputType.MouseRight => KeyCode.Mouse1,
                _ => KeyCode.None
            };

            return pressedThisFrame ? Input.GetKeyDown(key) : Input.GetKey(key);
#else
            return false;
#endif
        }
    }
}
