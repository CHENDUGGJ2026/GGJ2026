using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LunziSpace
{
    public class NPCCollider :MonoBehaviour
    {
        private GameObject _player;
        private bool _playerComeInCircle =false;
        private CharacterData characterData;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            characterData = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomCharacterData(Random.Range(1, 9));
            Debug.Log(characterData.name);
            UIManager.Instance.DialogPanel.GetComponent<DialogController>().FightStarAction += () => { this.enabled = false; };
        }

        private void Update()
        {
            if( _playerComeInCircle && Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("开始对话逻辑");
                UIManager.Instance.DialogPanel.GetComponent<DialogController>().SetCurNPCid(characterData.npcID);
                TextAsset newDialogText = UIManager.Instance.DialogPanel.GetComponent<DialogController>().dialogDataBase.GetRandomNormalDialog(characterData.npcID);
                UIManager.Instance.DialogPanel.GetComponent<DialogController>().UpdataCurText(newDialogText);
                UIManager.Instance.DialogPanel.SetActive(true);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if(collision.gameObject == _player)//如果进来的是玩家
            {
                _playerComeInCircle = true;
                Debug.Log("提示:按下M键开始战斗逻辑");

                //触发战斗逻辑后关闭玩家的控制器

            }
        }


    }

}
