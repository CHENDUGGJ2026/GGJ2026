using LunziSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanelController : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += () => { gameObject.SetActive(true); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
