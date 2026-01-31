using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void PutDown()
    {
        // 编辑器里停止运行
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 打包后真正退出
        Application.Quit();
#endif
    }
}
