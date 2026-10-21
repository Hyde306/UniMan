using System.Collections.Generic;
using UnityEngine;

public class DestroyBackground : MonoBehaviour
{
    public BackgroundManager manager;
    public List<GameObject> Dest_ObjList = new List<GameObject>();

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.Log("ÉJÉÅÉâÇ»Ç¢ÇÊ");
            return;
        }

        if (Dest_ObjList.Count == 0)
        {
            return;
        }

        float cameraBottom =
            Camera.main.transform.position.y -
            Camera.main.orthographicSize;

        float back_posi = Dest_ObjList[0].transform.position.y;
        float back_scale = Dest_ObjList[0].transform.localScale.y / 2;

        if (back_posi + back_scale < cameraBottom)
        {
            Debug.Log("back_posi:" + back_posi + back_scale);
            Debug.Log("camera:" + cameraBottom);
            Destroy(Dest_ObjList[0]);

            Dest_ObjList.RemoveAt(0);

            manager.SpawnBackground();
        }
    }
}