using UnityEngine;

public class HorizontalMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}