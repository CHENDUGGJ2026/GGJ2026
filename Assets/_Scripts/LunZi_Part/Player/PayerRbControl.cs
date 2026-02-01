using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LunziSpace
{
    public class PayerRbControl : MonoBehaviour
    {
       
        public Rigidbody2D playerRb;
        public BoxCollider2D playerCollider;
        public CircleCollider2D circleTrigger;
        private GameObject key_f;
        private GameObject npcGameObject;

        private bool hasNPC = false;
        void Start()
        {
            //获得碰撞体和刚体组件
            playerCollider = GetComponent<BoxCollider2D>();
            playerRb = GetComponent<Rigidbody2D>();
            circleTrigger = GetComponent<CircleCollider2D>();
            key_f = transform.Find("Key_F").gameObject;

            playerRb.freezeRotation = true;
            gameObject.tag = "Player";
            circleTrigger.isTrigger = true;
            key_f.gameObject.SetActive(false);

        }

        private void Update()
        {
            if(hasNPC && Input.GetKeyDown(KeyCode.F))
            {
                npcGameObject.GetComponent<NPCCollider>().SendToDialog();
                npcGameObject.GetComponent<NPCCollider>().enabled = false;
                hasNPC = false;
                npcGameObject.tag = "Finish";
                key_f.gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
           
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                npcGameObject = collision.gameObject;
                hasNPC = true;
               
                //浮现提示按钮
                key_f.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("NPC") )
            {
             
                key_f.gameObject.SetActive(false);
            }
        }

    }

}
