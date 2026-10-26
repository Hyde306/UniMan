using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject[] backgrounds;
    public DestroyBackground dest;
    public float blockHeight = 20f;
    public int startCount = 5;

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

        // Œ»Ý‘¶Ý‚·‚é”wŒi‚Ìˆê”Ôã‚ð’T‚·
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

        int index = Random.Range(0, backgrounds.Length);

        GameObject spawnedBg = Instantiate(
            backgrounds[index],
            new Vector3(0, spawnY, 0),
            Quaternion.identity
        );

        if (spawnedBg.GetComponent<AutoScroll>() == null)
        {
            spawnedBg.AddComponent<AutoScroll>();
        }

        dest.Dest_ObjList.Add(spawnedBg);
    }
}