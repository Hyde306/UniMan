using UnityEngine;

//プレイヤーを追いかけるカメラ
[DisallowMultipleComponent]
public sealed class Camera : MonoBehaviour
{
    public static UnityEngine.Camera main => UnityEngine.Camera.main;

    [SerializeField] private Transform target;
    [SerializeField] private float verticalOffset = 3f;
    [SerializeField, Min(0.01f)] private float smoothTime = 0.15f;
    [SerializeField] private bool neverMoveDown = true;

    private float yVelocity;
    private float minimumY;

    private void Awake()
    {
        minimumY = transform.position.y;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float desiredY = target.position.y + verticalOffset;
        if (neverMoveDown)
        {
            desiredY = Mathf.Max(minimumY, desiredY);
        }

        Vector3 position = transform.position;
        position.y = Mathf.SmoothDamp(position.y, desiredY, ref yVelocity, smoothTime);
        transform.position = position;

        if (neverMoveDown)
        {
            minimumY = Mathf.Max(minimumY, position.y);
        }
    }
}
