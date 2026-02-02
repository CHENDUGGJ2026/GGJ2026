//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using System.Collections.Generic;
namespace MyFrame.BrainBubbles.Bubbles.Refs
{
    public class GameValue
    {
        private Dictionary<BubbleType, float> _values;

        public GameValue()
        {
            _values = new Dictionary<BubbleType, float>();
            _values[BubbleType.Happy] = 0f;
            _values[BubbleType.Angry] = 0f;
            _values[BubbleType.Sad] = 0f;
            _values[BubbleType.Scared] = 0f;
        }

        public void SetValue(BubbleType type, float value)
        {
            _values[type] = value;
        }
        public bool TryGetValue(BubbleType type, out float value)
        {
            value = 0f;
            if (!_values.TryGetValue(type, out var v)) return false;
            value = v;
            return true;
        }

        public KeyValuePair<BubbleType, float>[] GetValues()
        {
            List<KeyValuePair<BubbleType, float>> list = new();
            foreach (var k in _values.Keys)
            {
                var v = _values[k];
                list.Add(new KeyValuePair<BubbleType, float>(k, v));
            }
            return list.ToArray();
        }
    }
}
