using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LunziSpace;
using System;
namespace luoyu
{
    public interface IResultInformastion
    {
        public bool ResultJudgment(OverInformation information, int face_num, int condition);
    }
    public class Time_Fight : MonoBehaviour
    {
        public static Time_Fight instance;
        private float waitTime;
        private float currentWaitTime;
        private Slider slider_Time;
        private IResultInformastion resultInformastion;
        //�����Ϣ�ӿ�
        private bool _result;
        private OverInformation _overInformation;
        private TargetFace _targetFace;

        public Action EndFightAction;


        //����ս�����
        public bool GetResult()
        {
            return _result;
        }

        private void Awake()
        {
            slider_Time = GetComponentInChildren<Slider>();
            waitTime = 10f;

            if(instance == null )
            {
                instance = this;  
            }
        }

        private void Start()
        {
            _overInformation = new OverInformation();
            _targetFace = new TargetFace();

            //����ս����ʼ
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightAction += StartFight;
        }


        public void GetInformation()
        {
            //_overInformation.happy = ;
            //_overInformation.sad = ;
            //_overInformation.afraid = ;
            //_overInformation.angry = ;
            _targetFace.face_num = UIManager.Instance.DialogPanel.GetComponent<DialogController>().TargetValue();
            _targetFace.condition = UIManager.Instance.DialogPanel.GetComponent<DialogController>().GetConditionValue();
        }

        public IEnumerator TimeDiscount()
        {
            while (currentWaitTime > 0)
            {
                currentWaitTime -= Time.deltaTime;
                slider_Time.value = currentWaitTime;

                yield return null;

                if (currentWaitTime <= 0)
                {
                    //if (resultInformastion.ResultJudgment())
                    //{
                    //    _result = true;
                    //}
                    //else
                    //{
                    //    _result = false;
                    //}
                    yield break;
                }
            }
        }

        //ս����ʼ
        public void StartFight()
        {
            slider_Time.maxValue = waitTime;
            currentWaitTime = waitTime;

            StartCoroutine(TimeDiscount());
        }
    }

    public class Over : IResultInformastion
    {
        //ս������ж�
        public bool ResultJudgment(OverInformation information, int face_num, int condition)
        {
            if (Win(information, face_num, condition))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Win(OverInformation information, int face_num, int target)
        {
            Debug.Log($"need : {face_num}  need value: {target}   current value:\nafraid: {information.afraid}\n" +
                $"happy: {information.happy}\nsad : {information.sad}\nangry : {information.angry}");
            switch (face_num)
            {
                case 1:
                    if (information.happy > information.sad && information.happy > information.afraid 
                        && information.happy > information.angry && information.happy>=target)
                    {
                        return true;
                    }
                    break;
                case 2:
                    if (information.sad > information.happy && information.sad > information.afraid
                        && information.sad > information.angry&&information.sad>=target)
                    {
                        return true;
                    }
                    break;
                case 4:
                    if (information.afraid > information.happy && information.afraid > information.sad
                        && information.afraid > information.angry && information.afraid >= target)
                    {
                        return true;
                    }
                    break ;
                case 3:
                    if (information.angry > information.happy && information.angry > information.sad 
                        && information.angry > information.afraid && information.angry >= target)
                    {
                        return true;
                    }
                    break;
                default:
                    return false;
            }
            return false;
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
