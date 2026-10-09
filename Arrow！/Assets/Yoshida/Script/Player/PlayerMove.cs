using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f; //速度
    bool wallHit = false;
    Vector2 wallDir;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Vector2 normalDir;
        if (Input.GetMouseButton(0))
        {
            normalDir = Quaternion.Euler(0, 0, 45) * Vector2.up;
        }
        else
        {
            normalDir = Quaternion.Euler(0, 0, -45) * Vector2.up;
        }
        if (wallHit)
        {
            transform.up = wallDir;
        }
        else
        {
            transform.up = normalDir;
        }
        transform.position += transform.up * speed * Time.deltaTime;
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
        Vector2 normalDir;
        if (Input.GetMouseButton(0))
        {
            normalDir = Quaternion.Euler(0, 0, 45) * Vector2.up;
        }
        else
        {
            normalDir = Quaternion.Euler(0, 0, -45) * Vector2.up;
        }
        float wallInput = Vector2.Dot(normalDir, normal);
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
        Debug.Log("GAME OVER");
        enabled = false;
    }
}