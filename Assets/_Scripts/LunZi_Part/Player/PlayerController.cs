using System;
using UnityEngine;

namespace LunziSpace
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private MyPlayerData curPlayer; // 玩家移动配置（序列化可在Inspector面板调节）

        private void Start()
        {
           
            if (curPlayer == null) curPlayer = new MyPlayerData { MoveSpeed = 5f };
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += DisableThisScript;


        }

        private void Update()
        {
            PlayerMoveByWASD(); // 每帧执行移动逻辑
        }

        /// <summary>
        /// WASD控制2D平面移动，使用Translate实现
        /// </summary>
        private void PlayerMoveByWASD()
        {

            float horizontal = Input.GetAxisRaw("Horizontal"); // 替换为GetAxisRaw，无平滑无延迟
            float vertical = Input.GetAxisRaw("Vertical");     // 同上

            Vector3 moveDir = new Vector3(horizontal, vertical, 0).normalized; // 保留归一化，防止斜向超速

            // 核心修正：Space.Self → Space.World，2D X/Y平面移动的正确坐标系
            transform.Translate(moveDir * curPlayer.MoveSpeed * Time.deltaTime, Space.World);
        }

        private void DisableThisScript()
        {
            this.enabled = false;
            Debug.Log("禁用玩家控制器");
        }
    }

    [Serializable]
    public class MyPlayerData 
    {
        [Tooltip("玩家移动速度")] // 可选：Inspector面板鼠标悬浮提示
        public float MoveSpeed = 5f; // 给默认值，直接可用
    }
}