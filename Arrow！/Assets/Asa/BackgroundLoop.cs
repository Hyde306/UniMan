using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    private Transform cam;
    private float height;

    void Start()
    {
        cam = Camera.main.transform;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        float cameraBottom =
            cam.position.y - Camera.main.orthographicSize;

        if (cameraBottom > transform.position.y + height / 2f)
        {
            transform.position += Vector3.up * height * 2f;
        }
    }
}