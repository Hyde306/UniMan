using System.Collections.Generic;
using UnityEngine;

public class BackgroundManagerK : MonoBehaviour
{
    public GameObject[] backgrounds;
    public DestroyBackgroundK dest;
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

        GameObject spawnedBg = Instantiate(
            backgrounds[index], // 前回の解説の通り、自分の配列から生成するのがスムーズです
            new Vector3(0, nextY, 0),
            Quaternion.identity);

        dest.Dest_ObjList.Add(spawnedBg);

        nextY += blockHeight;
    }
}