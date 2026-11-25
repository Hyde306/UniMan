using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject firstBackground; // 最初に生成する固定マップ
    public GameObject[] backgrounds;   // 2個目以降のランダムマップ
    public DestroyBackground dest;

    public float blockHeight = 20f;
    public int startCount = 5;

    private int spawnCount = 0;

    void Start()
    {
        for (int i = 0; i < startCount; i++)
        {
            SpawnBackground();
        }
    }

    public void SpawnBackground()
    {
        float spawnY = 0f;

        // 現在存在する背景の一番上を探す
        if (dest.Dest_ObjList.Count > 0)
        {
            float highestY = float.MinValue;

            foreach (GameObject bg in dest.Dest_ObjList)
            {
                if (bg != null && bg.transform.position.y > highestY)
                {
                    highestY = bg.transform.position.y;
                }
            }

            spawnY = highestY + blockHeight;
        }

        GameObject prefab;

        // 最初だけ固定プレハブ、それ以降はランダム
        if (spawnCount == 0)
        {
            prefab = firstBackground;
        }
        else
        {
            int index = Random.Range(0, backgrounds.Length);
            prefab = backgrounds[index];
        }

        GameObject spawnedBg = Instantiate(
            prefab,
            new Vector3(0, spawnY, 0),
            Quaternion.identity
        );

        // すべてのマップにAutoScrollを付ける
        if (spawnedBg.GetComponent<AutoScroll>() == null)
        {
            spawnedBg.AddComponent<AutoScroll>();
        }

        dest.Dest_ObjList.Add(spawnedBg);

        spawnCount++;
    }
}