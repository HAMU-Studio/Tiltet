using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpawnSensor : MonoBehaviour
{
    [Header("各プレイヤーのマテリアル")]
    [SerializeField] private Material P1mat;
    [SerializeField] private Material P2mat;
  
    [Header("各スポーン位置")]
    [SerializeField] private Transform P1Spawn;
    [SerializeField] private Transform P2Spawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            Material playerMat = player.GetComponentInChildren<Renderer>().materials[1];

            //普通に比較するとInstanceか通常かで比較が通らないから名前追加で無理やり通す
            if (playerMat.name == P1mat.name + " (Instance)")
            {
                player.transform.position = P1Spawn.position;
                
                if (GameManager.instance.P1Spawn)
                    return;
                
                DontDestroyOnLoad(player);
                
                if (player.transform.position == P1Spawn.position)
                {
                    Debug.Log("player1 spawn success");
                    GameManager.instance.P1Spawn = true;
                }
                else
                {
                    Debug.Log("player1 spawn fail");
                }
            }
            else if (playerMat.name == P2mat.name + " (Instance)")
            {
                player.transform.position = P2Spawn.position;
          
                if (GameManager.instance.P2Spawn)
                    return;
                
                DontDestroyOnLoad(player);
                if (player.transform.position == P2Spawn.position)
                {
                    Debug.Log("player2 spawn success");
                    GameManager.instance.P2Spawn = true;
                }
                else
                {
                    Debug.Log("player2 spawn fail");
                }
            }
        }
    }
}
