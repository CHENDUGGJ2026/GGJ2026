using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LunziSpace
{
    public class NPCCollider : MonoBehaviour
    {
               
        private CharacterData characterData;


        private void Start()
        {
            //玩家的随机生成
            characterData = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomCharacterData(Random.Range(1, 9));
            Debug.Log(characterData.name);
        
        }

        

        public void SendToDialog()
        {
            Debug.Log("开始对话逻辑");
            //更新立绘,按照NPCid随机获得对话脚本
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().SetCurNPCid(characterData.npcID);

            TextAsset newDialogText = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomNormalDialog(characterData.npcID);
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().UpdataCurText(newDialogText);
            UIManager.Instance.DialogPanel.SetActive(true);

        }

    }

}
