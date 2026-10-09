using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public BackgroundManager manager;

    private float nextSpawn = 40f;

    void Update()
    {
        if (transform.position.y > nextSpawn)
        {
            manager.SpawnBackground();

            nextSpawn += 20f;
        }
    }
}