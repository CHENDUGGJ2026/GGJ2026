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
        private Image _image;

        private float _warnningTime = 0.7f;
        private int _circle = 4;
        private float _minA = 0.7f;
        public BubbleFrame(RectTransform transform,GameObject frame,Vector2Int pos , Vector2Int size)
        {

            _frame = GameObject.Instantiate(frame, transform,false );
            RectTransform = _frame.GetComponent<RectTransform>();
            RectTransform.anchoredPosition = pos;
            RectTransform.sizeDelta = size;
           

            _timeSlider = _frame.transform.Find("Time").GetComponent<Slider>();
            _image = _timeSlider.fillRect.GetComponent<Image>();
        }

        public void ChangeTimeSlider(float time)
        {
            _timeSlider.value = time;
             _image.color = UIManager.Instance.Gradient.Evaluate(time);
            if( time > _warnningTime)
            {
                var t = (time - _warnningTime)*_circle / (1-_warnningTime)*Mathf.PI;
                _image.color = new Color(_image.color.r,_image.color.g,_image.color.b,_minA + (1-_minA)* Mathf.Cos(t)* Mathf.Cos(t));
            }
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