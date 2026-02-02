using luoyu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTargetController : MonoBehaviour
{
    private Slider slider_Target;
    private int currentWin_Num;
    private int targetWin_Num;
    private TMP_Text text_target;

    private void Awake()
    {
        slider_Target = GetComponentInChildren<Slider>();
        text_target = GetComponentInChildren<TMP_Text>();
        currentWin_Num = 0;
        targetWin_Num = 10;
        slider_Target.maxValue = targetWin_Num;
        text_target.text = currentWin_Num.ToString() + "/" + targetWin_Num.ToString();
    }

    private void Start()
    {
        Time_Fight.instance.EndFightAction += EndFight;
    }

    private void EndFight()
    {
        if(Time_Fight.instance.GetResult())
        {
            currentWin_Num++;
            slider_Target.value = currentWin_Num;
            text_target.text = currentWin_Num.ToString() + "/" + targetWin_Num.ToString();
        }

        if(currentWin_Num >= targetWin_Num)
        {
            //ÓÎÏ·Ê¤Àû
        }
    }
}
