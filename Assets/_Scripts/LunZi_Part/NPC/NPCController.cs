using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




namespace LunziSpace{

    public class NPCController : MonoBehaviour
    {
        
        private string ID;
        [Header("触发区域")]
       [SerializeField] private CircleCollider2D myCircleCollider;
        [Header("触发器大小")]
        [SerializeField] private int circleRadius =3;
        void Start()
        {
            Init();
        }

        private void Update()
        {
            
        }

     
        /// <summary>
        /// 初始化设置,主要是设置触发器的圆形区域
        /// </summary>
        private void Init()
        {
            //如果NPC数据为空,就用默认数据
         
            //圆形触发器
            if(myCircleCollider == null)
            {
                myCircleCollider = GetComponent<CircleCollider2D>();
                if(myCircleCollider == null)
                {
                    gameObject.AddComponent<CircleCollider2D>();
                }
            }
            //设置相关参数
            myCircleCollider.isTrigger = true;
            myCircleCollider.radius = circleRadius;


        }

     
    }


}

