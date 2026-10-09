using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AutoForward2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // �ړ����x�i�P��: m/s�j
    [SerializeField] private Vector2 direction = Vector2.right; // �i�s�����i�E�����j

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Rigidbody2D �̐ݒ�i�������������肳����j
        rb.gravityScale = 0f; // �d�͂𖳌���
        rb.freezeRotation = true; // ��]���Œ�
    }

    void FixedUpdate()
    {
        // ��葬�x�ňړ�
        rb.linearVelocity = direction.normalized * moveSpeed;
    }

    // �i�s�������O������ύX����֐��i��F�ǂɓ��������甽�]�Ȃǁj
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }
}