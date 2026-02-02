using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    public void PutDown()
    {
        Time.timeScale = 1.0f;
        OpenSettingUI.isOpen = false;
        canvas.gameObject.SetActive(false);
    }
}
