using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds;

    public float blockHeight = 20f;
    public int startCount = 5;

    private float nextY;

    void Start()
    {
        nextY = 0;

        for (int i = 0; i < startCount; i++)
        {
            SpawnBackground();
        }
    }

    public void SpawnBackground()
    {
        int index = Random.Range(0, backgrounds.Length);

        Instantiate(
            backgrounds[index],
            new Vector3(0, nextY, 0),
            Quaternion.identity);

        nextY += blockHeight;
    }
}