using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }
    public void PutDown()
    {
        SceneManager.LoadScene("LogicScene");
    }
}
