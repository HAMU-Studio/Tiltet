using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugRespawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   [SerializeField] private bool isRestart;

    private void OnCollisionEnter(Collision other)
    {
        
        Debug.Log("自機から落ちました");
        if (other.gameObject.CompareTag("Player")) 
        {
            if (isRestart)
            {
                GameManager.instance.SceneManager.StartTransition("MainStage");
                GameManager.instance.CurrentState = GameState.Restart;
            }
            else
            {
                GameManager.instance.RespawnPlayer(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Debug.Log("自機から落ちました");

        if (other.gameObject.CompareTag("Player"))
        {
            if (isRestart)
            {
               
                GameManager.instance.SceneManager.StartTransition("MainStage");
                GameManager.instance.CurrentState = GameState.Restart;
            }
            else
            {
                Debug.Log("Call Respawn");
                GameManager.instance.SetPlayerPos();
            }
        }

        if (other.gameObject.CompareTag("SphereEnemy") ||
            other.gameObject.CompareTag("EllipseEnemy"))
        {
            Destroy(other.gameObject);
        }
   
    }
}
