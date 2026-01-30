//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using System.Collections.Generic;
using UnityEngine;


namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BubblesInfo", order = 1)]
    public class BubblesData : ScriptableObject
    {
        public GameObject OutFace;
        public List<BubblesDataPairs> values;
    }
}
