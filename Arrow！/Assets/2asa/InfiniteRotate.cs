using UnityEngine;

public class InfiniteRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 180f;

    private float angle;

    void Start()
    {
        angle = transform.localEulerAngles.z;
    }

    void Update()
    {
        angle += rotateSpeed * Time.deltaTime;

        transform.localRotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}