using UnityEngine;

public class Left : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}