using UnityEngine;
using UnityEngine.SceneManagement;

//左右の進行方向を切り替えながら常に上へ進む
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[DisallowMultipleComponent]
public sealed class Player : MonoBehaviour
{
    [Header("移動")]
    [SerializeField, Min(0.1f)] private float speed = 7f;
    [SerializeField, Range(5f, 80f)] private float diagonalAngle = 45f;
    [SerializeField, Min(1f)] private float visualTurnSpeed = 2400f;
    [SerializeField] private bool snapVisualRotation = true;
    [SerializeField] private Transform visual;

    [Header("入力")]
    [SerializeField] private KeyCode turnKey = KeyCode.Space;
    [SerializeField] private bool allowMouseClick = true;

    [Header("ゲームオーバー")]
    [SerializeField] private UnityEngine.Camera gameplayCamera;
    [SerializeField, Min(1f)] private float deathDistanceBelowCamera = 7f;
    [SerializeField] private KeyCode restartKey = KeyCode.R;

    private Rigidbody2D body;
    private int horizontalDirection = 1;
    private Vector2 moveDirection;
    private Vector2 wallDirection;
    private Vector2 wallNormal;
    private bool touchingWall;
    private bool dead;

    public bool IsDead => dead;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.freezeRotation = true;

        if (gameplayCamera == null)
        {
            gameplayCamera = UnityEngine.Camera.main;
        }

        RefreshInputDirection();
        UpdateVisualDirection(true);
    }

    private void Update()
    {
        if (dead)
        {
            if (Input.GetKeyDown(restartKey))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            return;
        }

        if (Input.GetKeyDown(turnKey) || (allowMouseClick && Input.GetMouseButtonDown(0)))
        {
            horizontalDirection *= -1;
            RefreshInputDirection();

            // 壁から離れる方向に切り替えたときだけ、壁沿い移動を解除する。
            if (touchingWall && Vector2.Dot(moveDirection, wallNormal) >= 0f)
            {
                touchingWall = false;
            }

            UpdateVisualDirection(true);
        }

        UpdateVisualDirection(false);

        if (gameplayCamera != null &&
            transform.position.y < gameplayCamera.transform.position.y - deathDistanceBelowCamera)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        if (dead)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = touchingWall ? wallDirection : moveDirection;
        body.linearVelocity = direction.normalized * speed;
    }

    private void RefreshInputDirection()
    {
        moveDirection = Quaternion.Euler(0f, 0f, -horizontalDirection * diagonalAngle) * Vector2.up;
    }

    private void UpdateVisualDirection(bool forceImmediate)
    {
        if (visual == null)
        {
            return;
        }

        Vector2 direction = touchingWall ? wallDirection : moveDirection;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        if (snapVisualRotation || forceImmediate)
        {
            visual.rotation = targetRotation;
            return;
        }

        visual.rotation = Quaternion.RotateTowards(
            visual.rotation,
            targetRotation,
            visualTurnSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleWallCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleWallCollision(collision);
    }

    private void HandleWallCollision(Collision2D collision)
    {
        if (dead || !collision.gameObject.CompareTag("Wall") || collision.contactCount == 0)
        {
            return;
        }

        Vector2 bestSideNormal = Vector2.zero;
        float bestHorizontalScore = 0f;

        for (int index = 0; index < collision.contactCount; index++)
        {
            Vector2 normal = collision.GetContact(index).normal;

            // 障害物の下面に衝突したらゲームオーバー。
            if (normal.y < -0.65f)
            {
                Die();
                return;
            }

            float horizontalScore = Mathf.Abs(normal.x);
            bool movingIntoWall = Vector2.Dot(moveDirection, normal) < 0f;
            if (horizontalScore > 0.35f && movingIntoWall && horizontalScore > bestHorizontalScore)
            {
                bestHorizontalScore = horizontalScore;
                bestSideNormal = normal;
            }
        }

        // 側面にぶつかったら、移動と見た目をその壁と平行に揃える。
        if (bestHorizontalScore > 0f)
        {
            wallNormal = bestSideNormal;
            wallDirection = new Vector2(-wallNormal.y, wallNormal.x).normalized;
            if (wallDirection.y < 0f)
            {
                wallDirection = -wallDirection;
            }

            touchingWall = true;
            body.linearVelocity = wallDirection * speed;
            UpdateVisualDirection(true);
        }
        else
        {
            touchingWall = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            touchingWall = false;
        }
    }

    private void Die()
    {
        if (dead)
        {
            return;
        }

        dead = true;
        body.linearVelocity = Vector2.zero;
        Debug.Log("GAME OVER - Rキーでリトライ", this);
    }
}
