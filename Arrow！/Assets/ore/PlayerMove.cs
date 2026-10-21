
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度

    private bool isMoving = false; // 移動中かどうか
    private int moveDirection = 1; // 1なら右、-1なら左

    void Update()
    {
        // スペースキーを押したら移動開始・方向反転
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isMoving)
            {
                isMoving = true;
            }
            else
            {
                moveDirection *= -1;
            }
        }

        // 移動中なら設定された方向に進み続ける
        if (isMoving)
        {
            transform.position += Vector3.right * moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
