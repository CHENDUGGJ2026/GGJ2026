//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Core;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace MyFrame.BrainBubbles.Bubbles.Refs
{

    [Serializable]
    public class BubblesInfo
    {
        private GameObject _outFace;
        private List<(string, TypeValue)> _contents;

        public BubblesInfo(GameObject outFace, List<(string, TypeValue)> contents)
        {
            _outFace = outFace;
            _contents = contents;
        }

        public int Count => _contents.Count;
        public bool TryGetValue(int index, out string content, out TypeValue value)
        {
            value = null;
            content = null;
            if (_contents.Count <= index || index < 0) return false;
            var t = _contents[index];
            value = t.Item2;
            content = t.Item1;
            return true;
        }
        public bool TryCreateBubbleObject(RectTransform _target, BubblePos _pos, out GameObject bubble)
        {
            bubble = null;
            bubble = GameObject.Instantiate(_outFace, _target);

            var obj = bubble;

            var t = obj.GetComponent<RectTransform>();
            if (t != null)
            {
                t.anchoredPosition = Vector2.zero;
                t.SetLocalPositionAndRotation(new Vector3(_pos.X, _pos.Y), Quaternion.identity);
            }
            return true;
        }
        public static BubblesInfo Bulid( BubblesData bubblesData)
        {
            var type_d = new List<(string, TypeValue)>();
            foreach (var data in bubblesData.values)
            {
                Dictionary<BubbleType, float> d = new Dictionary<BubbleType, float>();
                foreach (var dic in data.value)
                {
                    d[dic.type] = dic.value;
                }
                TypeValue t = new TypeValue(d);

                type_d.Add((data.content, t));
            }
            return new BubblesInfo(bubblesData.OutFace, type_d);
        }
    }
}
