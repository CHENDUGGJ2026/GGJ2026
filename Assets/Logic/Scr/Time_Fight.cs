using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace luoyu
{
    public interface IResultInformastion
    {
        public bool ResultJudgment(OverInformation information, TargetFace target);
    }
    public interface IWinOrLose
    {
        public bool GetResult();
    }
    public class Time_Fight : MonoBehaviour,IWinOrLose
    {
        private float waitTime;
        private float currentWaitTime;
        private Slider slider_Time;
        private IResultInformastion resultInformastion;
        //结果信息接口
        private bool _result;
        private OverInformation _overInformation;
        private TargetFace _targetFace;

        

        public bool GetResult()
        {
            return _result;
        }

        private void Awake()
        {
            slider_Time = GetComponentInChildren<Slider>();
            currentWaitTime = waitTime;
            slider_Time.maxValue = waitTime;
        }

        private void Update()
        {
            if (currentWaitTime <= 0)
            {
                if(resultInformastion.ResultJudgment())
                {
                    _result = true;
                }
                else
                {
                    _result = false;
                }
            }
            else
            {
                currentWaitTime -= Time.deltaTime;
                slider_Time.value = currentWaitTime;
            }
        }

        public void GetInformation()
        {
            _overInformation.happy = ;
            _overInformation.sad = ;
            _overInformation.afraid = ;
            _overInformation.angry = ;
            _targetFace.face_num = ;
            _targetFace.condition = ;
        }
    }

    public class Over : IResultInformastion
    {

        public bool ResultJudgment(OverInformation information,TargetFace target)
        {
            if(Win(information,target.face_num,target.condition))
            {
                return true;
            }else
            {
                return false;
            }
        }

        public bool Win(OverInformation information, int face_num, int target)
        {
            switch(face_num)
            {
                case 1:
                    if(information.happy > information.sad && information.happy > information.afraid && information.happy > information.angry)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                case 2:
                    if(information.sad > information.happy && information.sad > information.afraid && information.sad > information.angry)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                case 4:
                    if(information.afraid > information.happy && information.afraid > information.sad && information.afraid > information.angry)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                case 3:
                    if(information.angry > information.happy && information.angry > information.sad && information.angry > information.afraid)
                    {
                        return true;
                    }else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

    }

    public class OverInformation
    {
        public int happy;//1
        public int sad;//2
        public int angry;//3
        public int afraid;//4
    }

    public class TargetFace
    {
        public int face_num;
        public int condition;
    }

}
