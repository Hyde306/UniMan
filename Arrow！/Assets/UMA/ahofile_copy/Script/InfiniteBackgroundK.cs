using UnityEngine;

public class InfiniteBackgroundK : MonoBehaviour
{
    public BackgroundManagerK manager;

    private float nextSpawn = 40f;

    void Update()
    {
        if (transform.position.y > nextSpawn)
        {
            //manager.SpawnBackground();

            nextSpawn += 20f;
        }
    }
}