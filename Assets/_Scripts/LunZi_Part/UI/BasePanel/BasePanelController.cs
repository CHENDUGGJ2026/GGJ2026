using LunziSpace;
using MyFrame.BrainBubbles.Bubbles.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanelController : MonoBehaviour
{
    [SerializeField] GameObject bFrame;
    [SerializeField] RectTransform pos;
    public BubbleData bubbleData;

    private float FadeTime = 0.75f; 


    private CanvasGroup _canvasGroup;
    private float _currentAlpha; 
    private bool _isFading;      // 是否正在淡入

    // Start is called before the first frame update
    void Start()
    {
        // 初始化CanvasGroup
        InitCanvasGroup();
        // 初始隐藏物体
        gameObject.SetActive(false);

        // 绑定战斗星星事件：激活物体+触发淡入
        UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += () =>
        {
            gameObject.SetActive(true);
            FadeIn(); // 直接调用淡入方法，0.5秒完成
        };

        // 初始化气泡数据
        bubbleData = new BubbleData()
        {
            prefab = bFrame,
            uiPos = pos,
            a = new Vector2Int(-200, -100),
            b = new Vector2Int(-200, -100)
        };
    }

    /// <summary>
    /// 初始化CanvasGroup组件
    /// </summary>
    private void InitCanvasGroup()
    {
        // 获取/添加CanvasGroup，无需给子物体单独挂载
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = true; // 阻挡射线（点击事件）
        _canvasGroup.interactable = true;   // 允许交互
        _currentAlpha = 0;
        _isFading = false;
    }

    /// <summary>
    /// 不协程,控制在0.5s - 淡入核心方法（自身+所有子物体）
    /// </summary>
    private void FadeIn()
    {
        // 重置淡入状态，开始淡入
        _currentAlpha = 0;
        _canvasGroup.alpha = _currentAlpha;
        _isFading = true;
    }


    private void Update()
    {
        
        if (!_isFading) return;

        
        float alphaStep = Time.deltaTime / FadeTime;
        _currentAlpha += alphaStep;
     
        _currentAlpha = Mathf.Clamp01(_currentAlpha);
     
        _canvasGroup.alpha = _currentAlpha;

        // 淡入完成
        if (_currentAlpha >= 1f)
        {
            _isFading = false;
            _canvasGroup.alpha = 1f;
        }
    }

    // 气泡数据类（原有逻辑不变）
    public class BubbleData
    {
        public RectTransform uiPos;
        public GameObject prefab;
        public Vector2Int a;
        public Vector2Int b;
    }
}