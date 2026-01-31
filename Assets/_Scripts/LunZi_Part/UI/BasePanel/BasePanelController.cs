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

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += () => { gameObject.SetActive(true); };

        bubbleData = new BubbleData()
        {
            prefab = bFrame,
            uiPos = pos,
            a = new Vector2Int(-200, -100),
            b = new Vector2Int(-200, -100)
        };

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class BubbleData
    {
        public RectTransform uiPos;
        public GameObject prefab;
        public Vector2Int a;
        public Vector2Int b;

        
    }
}
