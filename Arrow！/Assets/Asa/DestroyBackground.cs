using UnityEngine;

public class DestroyBackground : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null) return;

        float cameraBottom =
            Camera.main.transform.position.y -
            Camera.main.orthographicSize;

        if (transform.position.y < cameraBottom - 30f)
        {
            Destroy(gameObject);
        }
    }
}