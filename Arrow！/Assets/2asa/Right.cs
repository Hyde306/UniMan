using UnityEngine;

public class Right : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}