using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    public float scrollSpeed = 5f;

    void Update()
    {
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }
}