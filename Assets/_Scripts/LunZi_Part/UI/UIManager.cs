using LunziSpace;
using MyFrame.BrainBubbles.Bubbles.Manager;
using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public class CurExpressionUpDataController
    {
        private List<Sprite> Sprites = new List<Sprite>();
        

        public CurExpressionUpDataController()
        {
            InitSprites();
        }
        /// <summary>
        /// 将四个表情的引用放入一个列表
        /// </summary>
        private void InitSprites()
        {
            for (int i = 0; i < 5; i++)
            {
                Sprites.Add(Resources.Load<Sprite>($"Sprites/NPC/Expression/{i + 1}"));

            }
        }

        /// <summary>
        /// 更换UI当前表情的方法
        /// </summary>
        /// <param name="targetImage"></param>
        /// <param name="index">0对应happy的sprite,1对应sad,2对应angry,3对应affarid</param>
        public void changeExpression(Image targetImage, int index)
        {
            if(targetImage != null )
            {
                targetImage.sprite = Sprites[index];
            }
        }

        public void UseDefaultExpression(Image targetImage, int index = 4)
        {
            targetImage.sprite = Sprites[index];
        }

        
    }

    public CurExpressionUpDataController curExpController ;

    public GameObject BasePanel;
    public GameObject DialogPanel;

    public Gradient Gradient;
    /// <summary>
    /// 单例
    /// </summary>
    public static UIManager Instance {  get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        DialogPanel = transform.Find("DialogPanel").gameObject;
        BasePanel = transform.Find("BasePanel").gameObject;
        curExpController = new CurExpressionUpDataController();
        GameManager.Instance._eventBus.Subscribe<GameOverEvent>(GameOverListenner);
        

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameOverEvent"></param>
    void GameOverListenner(GameOverEvent gameOverEvent)
    {

        Destroy(transform.Find("Frame(Clone)").gameObject);
        DialogPanel.transform.Find("FightBtn").gameObject.SetActive(false);

        if (gameOverEvent.Res == true)
        {
            DialogPanel.SetActive(true);

            UnityEngine.TextAsset newDialogText = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomSuccessDialog();
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().UpdataCurText(newDialogText);
        }
        else
        {
            UnityEngine.TextAsset newDialogText = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomFailDialog();
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().UpdataCurText(newDialogText);
        }
        string message = gameOverEvent.Message;
        Debug.LogWarning(message);


        GameValue thisValue = gameOverEvent.Value;
        var a = thisValue.GetValues();
        string messages = "";
        foreach( var value in a )
        {
            messages += (value.ToString() + "\n");
            Debug.Log(messages);
        }
        
        int curIndex = GetMaxValueIndex(a);
        curExpController.changeExpression(BasePanel.transform.Find("CurrentExpression").gameObject.GetComponent<Image>(), curIndex);

            StartCoroutine(GoSleep(2f));

        curExpController.UseDefaultExpression(BasePanel.transform.Find("CurrentExpression").gameObject.GetComponent<Image>());
        DialogPanel.GetComponent<DialogController>().FightOver?.Invoke();




    }

    private int GetMaxValueIndex(KeyValuePair<BubbleType, float>[] kvpArray)
    {
        // 1. 空数组校验：避免空指针，抛出明确异常
        if (kvpArray == null || kvpArray.Length == 0)
        {
            throw new ArgumentNullException(nameof(kvpArray), "键值对数组不能为空或长度为0！");
        }

        // 2. 初始化最大值和索引：默认第一个元素为初始最大值
        int maxIndex = 0;
        float maxValue = kvpArray[0].Value;

        // 3. 遍历数组（从第二个元素开始，减少无效比较）
        for (int i = 1; i < kvpArray.Length; i++)
        {
            // 4. 比较并更新最大值和对应索引
            if (kvpArray[i].Value > maxValue)
            {
                maxValue = kvpArray[i].Value;
                maxIndex = i;
            }
            // 相等值取第一个出现的索引（原有逻辑保留，无需修改）
        }

        // 5. 核心新增：按最大值阈值判断返回结果
        // 最大值>100返回索引，≤100返回固定值4
        return maxValue > 100 ? maxIndex : 4;
    }
    IEnumerator GoSleep(float delayTime)
    {

        yield return new WaitForSeconds(delayTime);
        // ɽ  뵭   ߼ 
        DialogPanel.SetActive(false);
        BasePanel.SetActive(false);

    }
    void DisableAndFadeOutChild()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.Add(transform.Find("Frame(Clone)").gameObject);
        gameObjects.Add(BasePanel);
        gameObjects.Add(DialogPanel);

        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(false);
        }

    }


}
