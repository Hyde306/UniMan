using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;  //速度

    private void Update()
    {

        //マウス押してるか押してないかの処理
        if (Input.GetMouseButton(0)) //押してるとき
        {
            //transformでZを45度回転
            transform.rotation = Quaternion.Euler(0, 0, 45);

            //確認用ログ
            Debug.Log("押してる");    
        }
        else　　　　　　　　　　　　//押してないとき
        {
            //transformでZを-45度回転
            transform.rotation = Quaternion.Euler(0, 0,-45);

            //確認用ログ
            Debug.Log("押してない"); 
        }

        //一定速度で上に進む
        //transform.position += Vector3.up * speed * Time.deltaTime;    (旧)Vector3.upだと向きを変えても関係なく真上にしか進まない
        transform.position += transform.up * speed * Time.deltaTime;    //Vectorをtransformに変更
    }


}