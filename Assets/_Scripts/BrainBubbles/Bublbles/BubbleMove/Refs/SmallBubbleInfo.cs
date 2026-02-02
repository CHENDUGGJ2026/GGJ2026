//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrame.BrainBubbles.Bubbles.BubbleMove.Refs
{
    [CreateAssetMenu(fileName = "SmallBubbleData", menuName = "ScriptableObjects/SmallBubblesInfo", order = 1)]
    public class SmallBubbleInfo : ScriptableObject
    {
        [SerializeField]
        private List<SmallBubbleData> datas;
        private Dictionary<BubbleType, GameObject> dic;
        private Dictionary<BubbleType,GameObject> ToDic()
        {
            Dictionary<BubbleType, GameObject> dic = new Dictionary<BubbleType, GameObject>();
            foreach (var data in datas)
            {
                dic[data.Type] = data.GameObject;
            }
            return dic;
        }

        public bool TryCreateSmallBubble(BubbleType type, out GameObject obj)
        {
            obj = null;
            if ( dic == null ) dic = ToDic();
            if(!dic.TryGetValue(type, out var o))
            {
                Debug.LogError("dic not contained");
                return false;
            }
            obj = GameObject.Instantiate(o);
            Debug.Log("Successfully Created SmallBubble");
            return true;
        }
    }
}

