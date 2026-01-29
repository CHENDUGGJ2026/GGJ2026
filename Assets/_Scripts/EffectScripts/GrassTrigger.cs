using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTrigger : MonoBehaviour
{
    private GrassController grassController;
    private GameObject _player;
    private Material _material;
    private Rigidbody2D _playerRb;

    private bool _easeInCoroutineRunning; // 修正拼写错误（Corouting→Coroutine）
    private bool _easeOutCoroutineRunning;

    private int _externalInfluence = Shader.PropertyToID("_ExternalInfluence");

    private float _startingVelocity;
    private float _velocityLastFrame;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null) // 增加空值判断，避免空引用
        {
            Debug.LogError("未找到标签为Player的物体！");
            return;
        }
        _playerRb = _player.GetComponent<Rigidbody2D>();
        if (_playerRb == null)
        {
            Debug.LogError("Player物体上未找到Rigidbody2D组件！");
            return;
        }

        grassController = GetComponentInParent<GrassController>();
        if (grassController == null)
        {
            Debug.LogError("未找到父物体上的GrassController组件！");
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("当前物体上未找到SpriteRenderer组件！");
            return;
        }
        _material = spriteRenderer.material; // 使用实例化的material，避免影响其他草叶

        _startingVelocity = _material.GetFloat(_externalInfluence);
        _velocityLastFrame = _playerRb.velocity.x; // 初始化上一帧速度，避免首次判断异常
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player && _playerRb != null)
        {
            // 只有当EaseIn未运行、EaseOut未运行，且玩家速度超过临界值时触发
            if (!_easeInCoroutineRunning && !_easeOutCoroutineRunning
                && Mathf.Abs(_playerRb.velocity.x) > Mathf.Abs(grassController.velocity))
            {
                StartCoroutine(EaseIn(_playerRb.velocity.x * grassController.ExternalInfluenceStrength));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player && !_easeOutCoroutineRunning)
        {
            StartCoroutine(EaseOut());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != _player || _playerRb == null) return;

        // 先更新上一帧速度（修正赋值时机）
        float currentVelocity = _playerRb.velocity.x;

        // 情况1：玩家速度从高于临界值→低于临界值，触发EaseOut
        if (!_easeOutCoroutineRunning && Mathf.Abs(_velocityLastFrame) > Mathf.Abs(grassController.velocity)
            && Mathf.Abs(currentVelocity) < Mathf.Abs(grassController.velocity))
        {
            StartCoroutine(EaseOut());
        }
        // 情况2：玩家速度从低于临界值→高于临界值，触发EaseIn
        else if (!_easeInCoroutineRunning && Mathf.Abs(_velocityLastFrame) < Mathf.Abs(grassController.velocity)
            && Mathf.Abs(currentVelocity) > Mathf.Abs(grassController.velocity))
        {
            StartCoroutine(EaseIn(currentVelocity * grassController.ExternalInfluenceStrength));
        }
        // 情况3：速度稳定高于临界值，直接设置影响值（无插值）
        else if (!_easeInCoroutineRunning && !_easeOutCoroutineRunning
            && Mathf.Abs(currentVelocity) > Mathf.Abs(grassController.velocity))
        {
            grassController.InfluenceGrass(_material, currentVelocity * grassController.ExternalInfluenceStrength);
        }

        // 最后更新上一帧速度
        _velocityLastFrame = currentVelocity;
    }

    private IEnumerator EaseIn(float targetXVelocity)
    {
        _easeInCoroutineRunning = true;
        // 以当前影响值为插值起点（修正：不再用初始值）
        float startValue = _material.GetFloat(_externalInfluence);
        float durationTime = 0f;

        while (durationTime < grassController.easeInTime)
        {
            durationTime += Time.deltaTime;
            float t = durationTime / grassController.easeInTime;
            // 可选：增加缓动曲线，让插值更自然（如Mathf.SmoothStep）
            float lerpedAmount = Mathf.Lerp(startValue, targetXVelocity, t);
            grassController.InfluenceGrass(_material, lerpedAmount);
            yield return null;
        }

        // 确保最终值准确设置
        grassController.InfluenceGrass(_material, targetXVelocity);
        _easeInCoroutineRunning = false; // 重置EaseIn状态
    }

    private IEnumerator EaseOut()
    {
        _easeOutCoroutineRunning = true;
        // 以当前影响值为插值起点，目标值为初始值
        float startValue = _material.GetFloat(_externalInfluence);
        float durationTime = 0f;

        while (durationTime < grassController.easeOutTime) // 修正拼写：easeOurTime→easeOutTime
        {
            durationTime += Time.deltaTime;
            float t = durationTime / grassController.easeOutTime;
            // 修正Lerp参数顺序：从当前值→初始值
            float lerpedAmount = Mathf.Lerp(startValue, _startingVelocity, t);
            grassController.InfluenceGrass(_material, lerpedAmount);
            yield return null;
        }

        // 确保最终恢复到初始值
        grassController.InfluenceGrass(_material, _startingVelocity);
        _easeOutCoroutineRunning = false;
    }
}