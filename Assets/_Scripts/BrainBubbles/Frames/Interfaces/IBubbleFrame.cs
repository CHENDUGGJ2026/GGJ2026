//Author : _SourceCode
//CreateTime : 2026-01-30-20:40:14
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MyFrame.BrainBubbles.Frame.Interfaces
{
    //public interface IBubbleFrame
    //{
    //    bool InFrame(BubblePos pos);
    //}
}
namespace MyFrame.BrainBubbles.Frame.Core
{
    public class BubbleFrame
    {
        public RectTransform RectTransform { get; private set; }
        private Vector4 _bubbleRect = new Vector4(0.1f, 0.1f, 0.9f, 0.9f);
        private GameObject _frame;
        private Slider _timeSlider;

        public BubbleFrame(RectTransform transform,GameObject frame,Vector2Int pos , Vector2Int size)
        {

            _frame = GameObject.Instantiate(frame, transform,false );
            RectTransform = _frame.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = pos;
            RectTransform.sizeDelta = size;

            _timeSlider = _frame.transform.Find("Time").GetComponent<Slider>();
        }

        public void ChangeTimeSlider(float time)
        {
            _timeSlider.value = time;
        }

        public bool InFrame(BubblePos pos) => (Xmin <= pos.X)
            && (pos.Y >= Ymin)
            && (pos.X <= Xmax)
            && (pos.Y <= Ymax);
        public float Xmin => RectTransform.rect.width * (_bubbleRect.x - 0.5f);
        public float Xmax => RectTransform.rect.width * (_bubbleRect.z - 0.5f);
        public float Ymin => RectTransform.rect.height* (_bubbleRect.y - 0.5f);
        public float Ymax => RectTransform.rect.height* (_bubbleRect.w - 0.5f);
    }
}