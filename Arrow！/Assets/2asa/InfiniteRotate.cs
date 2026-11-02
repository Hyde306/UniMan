using UnityEngine;

public class InfiniteRotate : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 180f;

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}