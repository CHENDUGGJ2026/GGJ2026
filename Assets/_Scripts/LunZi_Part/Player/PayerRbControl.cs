using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LunziSpace
{
    public class PayerRbControl : MonoBehaviour
    {
       
        public Rigidbody2D playerRb;
        public BoxCollider2D playerCollider;
        void Start()
        {
            //获得碰撞体和刚体组件
            playerCollider = GetComponent<BoxCollider2D>();
            playerRb = GetComponent<Rigidbody2D>();

            playerRb.freezeRotation = true;
            gameObject.tag = "Player";

        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
           
        }

    }

}
