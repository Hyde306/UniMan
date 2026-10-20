using UnityEngine;

public class AutoScrollK : MonoBehaviour
{
    public float scrollSpeed = 5f;

    void Update()
    {
        transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
    }
}