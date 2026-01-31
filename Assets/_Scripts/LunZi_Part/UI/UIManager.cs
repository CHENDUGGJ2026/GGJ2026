using LunziSpace;
using MyFrame.BrainBubbles.Bubbles.Manager;
using MyFrame.EventSystem.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UIManager : MonoBehaviour
{

    public GameObject BasePanel;
    public GameObject DialogPanel;
    private List<Sprite> Sprites = new List<Sprite>();
   
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

        GameManager.Instance._eventBus.Subscribe<GameOverEvent>(GameOverListenner);
        InitSprites();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameOverEvent"></param>
    void GameOverListenner(GameOverEvent gameOverEvent)
    {
        Debug.LogWarning("结算事件触发");
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

        //睡眠后关闭面板
        StartCoroutine(GoSleep(1f));


        /*DialogPanel.GetComponent<DialogController>().FightOver?.Invoke();*/
     
    }

    
    IEnumerator GoSleep(float delayTime)
    {
       
        yield return new WaitForSeconds(delayTime);
        //可接入淡出逻辑
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

    private void InitSprites()
    {
        for (int i = 0;i<4;i++)
        {
            Sprites.Add(Resources.Load<Sprite>($"Sprites/NPC/Expression/{i+1}"));
          
        }
    }
}
