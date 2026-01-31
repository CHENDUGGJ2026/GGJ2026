using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace luoyu
{
    public interface IPlayerAnim
    {
        void Stand();
        void Walk(int key);
    }
    public class PlayerAnimController : MonoBehaviour,IPlayerAnim
    {
        [SerializeField] Animator animator;

        void Start()
        {
            // 确保获取Animator组件
            if (animator == null)
                animator = GetComponent<Animator>();
        }
        public void Stand()
        {
            animator.SetBool("IsWalk", false);
        }

        public void Walk(int key)
        {
            animator.SetBool("IsWalk", true);
            animator.SetInteger("Dis", key);
        }
    }
}

