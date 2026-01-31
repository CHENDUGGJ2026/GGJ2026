using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject BasePanel;
    public GameObject DialogPanel;
    /// <summary>
    /// µ¥Àý
    /// </summary>
    public static UIManager Instance {  get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        DialogPanel = transform.Find("DialogPanel").gameObject;
        BasePanel = transform.Find("BasePanel").gameObject;
    }

}
