using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] GameObject UI;
    public VideoController controller;

    public void PutDown()
    {
        UI.gameObject.SetActive(false);
        controller.StartVideo();
    }
}
