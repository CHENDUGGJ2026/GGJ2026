//Author : _SourceCode
//CreateTime : 2026-01-30-18:19:41
//Version : 1.0
//UnityVersion : 2022.3.62f2c1

using MyFrame.BrainBubbles.Bubbles.Refs;
using MyFrame.EventSystem.Events;
using MyFrame.EventSystem.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyFrame.BrainBubbles.Bubbles.Core
{
    /// <summary>
    /// Engine Class
    /// </summary>
    public class BrainBubble : BubbleBase
    {
        private float _time;
        private BubblePos _pos;
        private string _content;

        private Button _trigger;
        private GameObject _outFace;
        private RectTransform _target;

        private float _easeInTime = 0.5f;
        private float _easeInTimer;
        private float _easeOutTime = 0.5f;
        private float _easeOutTimer;

        private bool _easeOut = false;
        private bool _start = false;
        

        public BrainBubble(float time,BubblePos pos , string content,
            Button trigger ,GameObject outFace, RectTransform target,
            string id, TypeValue values, IEventBusCore eventBusCore)
            :base(id,values,eventBusCore)
        {
            _time = time;
            _pos = pos;
            _content = content;
            _target = target;
            _trigger = trigger;
            _outFace = outFace;
            _target = target;

            
        }
        private void ClickBoom()
        {
            Over(BubbleBoomReason.Click,"On Click");
        }
        public override void Init()
        {
            _trigger.onClick.AddListener(ClickBoom);

            var text = _outFace.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if(text != null) text.text = _content;

            _easeInTimer = 0;
            _easeOutTimer = _easeOutTime;
            _easeOut = false;
            _start = true;
        }
        public override void OnUpdate(float deltaTime)
        {
            if(!_start) return;
            //Debug.Log($"Bubble {_id} Time: {_time}  _easeinTime: {_easeInTimer} _easeoutTime: {_easeOutTimer}");
            _time -= deltaTime;
            if (_easeInTimer < _easeInTime)
            {
                float ava = _easeInTimer / _easeInTime * Mathf.PI/2;
                float a = 1-Mathf.Cos(ava);

                var sprite = _outFace.GetComponent<Image>();
                if(sprite != null)
                {
                    sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,a);
                }
                _easeInTimer += deltaTime;
            }
            if(_time - _easeOutTime <= 0 && !_easeOut)
            {
                _easeOutTimer = 0;
                _easeOut = true;    
            }
            if (_time <= 0)
            {
                _time = 0;
                Over(BubbleBoomReason.TimeOff,"Time Off");
            }

            if(_easeOutTimer < _easeOutTime && _easeOut)
            {
                float ava = _easeOutTimer / _easeOutTime * Mathf.PI / 2;
                float a = Mathf.Cos(ava);

                var sprite = _outFace.GetComponent<Image>();
                if (sprite != null)
                {
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, a);
                }
                _easeOutTimer += deltaTime;
            }
        }

        private void Over(BubbleBoomReason reason , string message = "")
        {
            Boom(reason,message);
            GameObject.Destroy(_outFace);
            _start = false;
        }
    }
}

