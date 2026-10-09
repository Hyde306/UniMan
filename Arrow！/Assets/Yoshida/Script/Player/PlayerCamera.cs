using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;    //カメラが追いかけるターゲット
    public float z = 10.0f;
    public float xLimit = 3.0f;



    private void Update()   //毎フレーム処理
    {

        float distanceX = target.position.x - transform.position.x;
        float cameraX = transform.position.x;

        //端っこ行かない限りカメラの横移動は最小限にする処理
        if (distanceX > xLimit)
        {
            cameraX = target.position.x - xLimit;
        }
        else if(distanceX < -xLimit)
        {
            cameraX = target.position.x + xLimit;
        }



            transform.position = new Vector3(
                //target.position.x,    //プレイヤーのXを代入　このままでは常にプレイヤーを中心にしか捉えないので専用処理を作る
                cameraX,

                target.position.y,      //プレイヤーのYを代入
                transform.position.z    //今のカメラのZをそのまま使う
            );
    }

}
