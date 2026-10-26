using UnityEngine;
public class PlayerMoveK : MonoBehaviour
{
    public float speed = 5.0f; //速度
    public float turnSpeed = 450.0f;
    public Transform visual;
    bool wallHit = false;
    bool dead = false;

    int dir_player = 45;

    Vector2 moveDir;
    Vector2 wallDir;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        if (dead)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            dir_player = -dir_player;
        }

        Vector2 inputDir = Quaternion.Euler(0, 0, dir_player) * Vector2.up;

        if (wallHit)
        {
            moveDir = wallDir;
        }
        else
        {
            moveDir = inputDir;
        }
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90.0f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        if (visual != null)
        {
            visual.rotation = Quaternion.RotateTowards(
                visual.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }
    }
    void FixedUpdate()
    {
        if (dead)
        {
            return;
        }
        rb.MovePosition(
            rb.position + moveDir * speed * Time.fixedDeltaTime
        );
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Wall"))
        {
            return;
        }
        Vector2 normal = collision.contacts[0].normal;
        if (normal.y < -0.7f)
        {
            Die();
            return;
        }
        Vector2 inputDir;

        inputDir = Quaternion.Euler(0, 0, dir_player) * Vector2.up;

        float wallInput = Vector2.Dot(inputDir, normal);
        if (wallInput < 0)
        {
            wallDir = new Vector2(-normal.y, normal.x);
            if (wallDir.y < 0)
            {
                wallDir = -wallDir;
            }
            wallHit = true;
        }
        else
        {
            wallHit = false;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            wallHit = false;
        }
    }
    void Die()
    {
        dead = true;
        Debug.Log("GAME OVER");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("衝突した: " + collision.gameObject.name);
        Debug.Log("タグ: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Obstacleに衝突！プレイヤーを削除します");
            Destroy(gameObject);
        }
    }
}