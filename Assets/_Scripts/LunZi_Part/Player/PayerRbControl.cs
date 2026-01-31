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

        private GameObject npcGameObject;

        private bool hasNPC = false;
        void Start()
        {
            //获得碰撞体和刚体组件
            playerCollider = GetComponent<BoxCollider2D>();
            playerRb = GetComponent<Rigidbody2D>();
            circleTrigger = GetComponent<CircleCollider2D>();

            playerRb.freezeRotation = true;
            gameObject.tag = "Player";
            circleTrigger.isTrigger = true;

        }

        private void Update()
        {
            if(hasNPC && Input.GetKeyDown(KeyCode.M))
            {
                npcGameObject.GetComponent<NPCCollider>().SendToDialog();
                npcGameObject.GetComponent<NPCCollider>().enabled = false;
                hasNPC = false;
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
                gameObject.tag = "Finish";
                //浮现提示按钮
            }
        }

    }

}
