using UnityEngine;

public class DownRight : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    void Update()
    {
        Vector3 direction = new Vector3(1f, -1f, 0f).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}