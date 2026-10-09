using UnityEngine;

public class CameraAutoScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;

    void Update()
    {
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }
}