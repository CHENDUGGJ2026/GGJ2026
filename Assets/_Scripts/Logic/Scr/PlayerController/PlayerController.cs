using System;
using UnityEngine;
using luoyu;
namespace LunziSpace
{
    public class PlayerController_Anim : MonoBehaviour
    {
        [SerializeField] private MyPlayerData_Anim curPlayer; // 玩家移动配置（序列化可在Inspector面板调节）
        private IPlayerAnim playerAnim; // 动画接口引用

        private void Start()
        {

            if (curPlayer == null) curPlayer = new MyPlayerData_Anim { MoveSpeed = 5f };
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += DisableThisScript;
            playerAnim = GetComponent<PlayerAnimController>();
        }

        private void Update()
        {
            PlayerMoveByWASD(); // 每帧执行移动逻辑
        }

        /// <summary>
        /// WASD控制2D平面移动，使用Translate实现
        /// </summary>
        /// <summary>
        /// WASD控制2D平面移动，并同步播放对应方向动画
        /// </summary>
        private void PlayerMoveByWASD()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // 判断是否有移动输入
            if (horizontal != 0 || vertical != 0)
            {
                Vector3 moveDir = new Vector3(horizontal, vertical, 0).normalized;
                // 执行移动
                transform.Translate(moveDir * curPlayer.MoveSpeed * Time.deltaTime, Space.World);

                // 根据输入判断移动方向并播放对应动画
                // 方向定义：1=上 2=下 3=左 4=右（你可以根据自己的动画参数调整）
                if (vertical > 0)
                {
                    playerAnim.Walk(1); // 向上走
                }
                else if (vertical < 0)
                {
                    playerAnim.Walk(2); // 向下走
                }
                else if (horizontal < 0)
                {
                    playerAnim.Walk(3); // 向左走
                }
                else if (horizontal > 0)
                {
                    playerAnim.Walk(4); // 向右走
                }
            }
            else
            {
                // 没有输入时播放静止动画
                playerAnim.Stand();
            }
        }

        private void DisableThisScript()
        {
            this.enabled = false;
            Debug.Log("禁用玩家控制器");
        }
    }

    [Serializable]
    public class MyPlayerData_Anim
    {
        [Tooltip("玩家移动速度")] // 可选：Inspector面板鼠标悬浮提示
        public float MoveSpeed = 5f; // 给默认值，直接可用
    }
}