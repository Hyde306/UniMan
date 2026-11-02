
using UnityEngine;

// Inspectorで選択できる操作タイプ
public enum PlayerInputType
{
    Space,      // スペースキー
    LeftShift,  // 左Shiftキー
    MouseLeft,  // マウス左クリック
    MouseRight  // マウス右クリック
}

public class PlayerMove : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 5f; // 通常の移動速度
    [SerializeField] private float dashSpeed = 10f; // ダッシュ中の移動速度

    [Header("操作設定")]
    [SerializeField] private PlayerInputType directionKey = PlayerInputType.Space; // 移動・方向切り替え
    [SerializeField] private PlayerInputType dashKey = PlayerInputType.LeftShift; // ダッシュ

    [Header("当たり判定設定")]
    [SerializeField] private LayerMask wallLayer; // 壁のレイヤー
    [SerializeField] private float checkDistance = 0.05f; // 壁との判定距離

    private Rigidbody2D rb; // Rigidbody2D
    private Collider2D playerCollider; // プレイヤーの当たり判定

    private bool isMoving = false; // 移動中かどうか
    private bool isBlocked = false; // 壁に当たっているか
    private int moveDirection = 1; // 1なら右、-1なら左

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // 設定した操作で移動開始・方向反転
        if (InputDown(directionKey))
        {
            if (!isMoving)
            {
                isMoving = true;
            }
            else
            {
                moveDirection *= -1;
            }

            // 方向を切り替えたので停止状態を解除
            isBlocked = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null || playerCollider == null)
        {
            return;
        }

        // 移動開始前は停止
        if (!isMoving)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // 進行方向に壁があるか確認
        Bounds bounds = playerCollider.bounds;

        Vector2 checkPosition = new Vector2(
            moveDirection > 0 ? bounds.max.x : bounds.min.x,
            bounds.center.y
        );

        RaycastHit2D hit = Physics2D.Raycast(
            checkPosition,
            Vector2.right * moveDirection,
            checkDistance,
            wallLayer
        );

        // 壁に当たったら停止
        if (hit.collider != null)
        {
            isBlocked = true;
        }

        // 壁に当たっている間は停止
        if (isBlocked)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // 設定した操作を押している間はダッシュ速度にする
        float currentSpeed = moveSpeed;

        if (InputHeld(dashKey))
        {
            currentSpeed = dashSpeed;
        }

        // 設定された方向に移動
        rb.linearVelocity = new Vector2(
            moveDirection * currentSpeed,
            rb.linearVelocity.y
        );
    }

    // キーまたはマウスボタンを押した瞬間を判定
    private bool InputDown(PlayerInputType inputType)
    {
        switch (inputType)
        {
            case PlayerInputType.Space:
                return Input.GetKeyDown(KeyCode.Space);

            case PlayerInputType.LeftShift:
                return Input.GetKeyDown(KeyCode.LeftShift);

            case PlayerInputType.MouseLeft:
                return Input.GetMouseButtonDown(0);

            case PlayerInputType.MouseRight:
                return Input.GetMouseButtonDown(1);
        }

        return false;
    }

    // キーまたはマウスボタンを押している間を判定
    private bool InputHeld(PlayerInputType inputType)
    {
        switch (inputType)
        {
            case PlayerInputType.Space:
                return Input.GetKey(KeyCode.Space);

            case PlayerInputType.LeftShift:
                return Input.GetKey(KeyCode.LeftShift);

            case PlayerInputType.MouseLeft:
                return Input.GetMouseButton(0);

            case PlayerInputType.MouseRight:
                return Input.GetMouseButton(1);
        }

        return false;
    }

    void OnDrawGizmosSelected()
    {
        // 壁の判定位置をSceneビューに表示
        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
        {
            return;
        }

        Bounds bounds = col.bounds;

        Vector2 checkPosition = new Vector2(
            moveDirection > 0 ? bounds.max.x : bounds.min.x,
            bounds.center.y
        );

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            checkPosition,
            checkPosition + Vector2.right * moveDirection * checkDistance
        );
    }
}
