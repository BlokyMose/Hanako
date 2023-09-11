using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    public class TeketekeController : MonoBehaviour
    {
        [SerializeField] private float speed;//プレイヤーの移動速度
        [SerializeField] private float maxY, minY; //移動範囲の制限

       

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            MovePlayer(); //プレイヤーを動かすメソッドを呼び出す

            
        }

        void MovePlayer()
        {
            //もし上矢印キーが押されたら
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                Vector3 playerPos = transform.position; //Vector3型のplayerPosに現在の位置情報を格納
                playerPos.y += speed * Time.deltaTime; //y座標にspeedを加算

                //もしplayerPosのY座標がmaxY（最大Y座標）より大きくなったら
                if (maxY < playerPos.y)
                {
                    playerPos.y = maxY; //PlayerPosのY座標にmaxYを代入
                }

                transform.position = playerPos; //現在の位置情報に反映させる
            }

            
            else if (Input.GetAxisRaw("Vertical") < 0) //もし下矢印キーが押されたら
            {
                Vector3 playerPos = transform.position;
                playerPos.y -= speed * Time.deltaTime;

                if (minY > playerPos.y)
                {
                    playerPos.y = minY;
                }

                transform.position = playerPos;
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("desk") || collision.CompareTag("chair") || collision.CompareTag("lectern"))
            {
                collision.gameObject.SetActive(false);
            }
        }

    }
}
