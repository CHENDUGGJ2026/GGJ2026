using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PlayerBehaviourController : MonoBehaviour
{
    [Header("动画相关")]
    [SerializeField] private Animation animation; // 预留的Animation组件引用
    [SerializeField] private Animator animator;   // 推荐使用Animator（更常用）

    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f; // 移动速度
    [SerializeField] private float jumpForce = 7f; // 跳跃力度

    [Header("物理相关")]
    [SerializeField] private Rigidbody2D rb;       // 2D刚体组件
    [SerializeField] private Transform groundCheck;// 地面检测点
    [SerializeField] private LayerMask groundLayer;// 地面图层

    // 私有变量
    private float horizontalInput; // 水平输入值
    private bool isFacingRight = true; // 是否面向右侧
    private bool isGrounded; // 是否在地面上

    // 动画参数哈希值（性能更优）
    private int animSpeedHash;
    private int animJumpHash;
    private int animGroundedHash;

    private void Awake()
    {
        // 获取组件引用（如果未在Inspector面板赋值）
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

            rb.freezeRotation = true;

        // 初始化动画参数哈希（避免字符串查找，提升性能）
        animSpeedHash = Animator.StringToHash("Speed");
        animJumpHash = Animator.StringToHash("IsJumping");
        animGroundedHash = Animator.StringToHash("IsGrounded");
    }

    private void Update()
    {
        // 检测输入
        GetPlayerInput();

        // 检测是否在地面
        CheckGroundStatus();

        // 更新动画状态
        UpdateAnimationParameters();
    }

    private void FixedUpdate()
    {
        // 处理物理相关的移动
        HandleMovement();
    }


    #region 辅助方法
    /// <summary>
    /// 获取玩家输入
    /// </summary>
    private void GetPlayerInput()
    {
        // 获取水平输入（A/D键 或 左右方向键）
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 跳跃输入（空格键）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    /// <summary>
    /// 处理玩家移动逻辑
    /// </summary>
    private void HandleMovement()
    {
        // 设置水平移动速度
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;

        // 处理角色翻转
        FlipCharacter();
    }

    /// <summary>
    /// 跳跃逻辑
    /// </summary>
    private void Jump()
    {
        // 给刚体添加向上的力
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // 立即更新跳跃动画状态
        if (animator != null)
            animator.SetBool(animJumpHash, true);
    }

    /// <summary>
    /// 角色翻转（根据移动方向）
    /// </summary>
    private void FlipCharacter()
    {
        // 如果向右移动但角色朝左，或向左移动但角色朝右，则翻转
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            // 翻转本地缩放的X轴
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    /// <summary>
    /// 检测是否在地面上
    /// </summary>
    private void CheckGroundStatus()
    {
        // 通过圆形检测判断是否接触地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        // 更新地面状态动画参数
        if (animator != null)
            animator.SetBool(animGroundedHash, isGrounded);
    }

    /// <summary>
    /// 更新动画参数
    /// </summary>
    private void UpdateAnimationParameters()
    {
        // 如果使用Animator（推荐）
        if (animator != null)
        {
            // 传递移动速度（用于走路/跑步动画）
            animator.SetFloat(animSpeedHash, Mathf.Abs(horizontalInput));

            // 如果落地了，重置跳跃动画
            if (isGrounded)
                animator.SetBool(animJumpHash, false);
        }

        // 预留的Animation组件接口（如果需要使用旧的Animation系统）
        if (animation != null)
        {
            // 这里是Animation组件的预留逻辑示例
            // 例如：播放移动动画
            if (Mathf.Abs(horizontalInput) > 0 && isGrounded && !animation.isPlaying)
            {
                // animation.Play("Walk"); // 根据你的动画名称修改
            }
        }
    }

    // 绘制Gizmos，方便调试地面检测点
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }

    #endregion
}