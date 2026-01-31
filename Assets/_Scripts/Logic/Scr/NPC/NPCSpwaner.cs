using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpwaner : MonoBehaviour
{
    [SerializeField] float dis_x;
    [SerializeField] float dis_y;
    [SerializeField] GameObject NPCPrefab;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Random.Range(3f, 10f);
    }
    private void Update()
    {
        if(spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
        }else
        {
            spawnTime = Random.Range(15f, 30f);
            GameObject newNPC = Instantiate(NPCPrefab,transform);
            //设置NPC行走方向  
            //dis_y > 0 向上走
            //dis_y < 0 向下走
            //dis_x > 0 向右走
            //dis_y < 0 向左走
            newNPC.GetComponent<NPCMoveController>().NPCMove(dis_x,dis_y);
        }
    }

}
