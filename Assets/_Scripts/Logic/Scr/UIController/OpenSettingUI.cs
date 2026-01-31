using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingUI : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    public static bool isOpen;
    private void Start()
    {
        isOpen = false;
    }
    private void Update()
    {
        OpenSetting();
    }
    public void OpenSetting()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;
            if(!isOpen)
            {
                Time.timeScale = 0f;
                canvas.gameObject.SetActive(true);
            }else
            {
                Time.timeScale = 1.0f;
                canvas.gameObject.SetActive(false);
            }
        }
    }
}
