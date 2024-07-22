using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallArea : MonoBehaviour
{
   
    [SerializeField] private GameObject[] RescueActAreas;
    private GameObject fallPlayerInstance;

    private bool waitRescue;

    private void Start()
    {
        waitRescue = false;
        shortestDist = 0;
        foreach (GameObject area in RescueActAreas)
        {
            area.SetActive(false);
            area.GetComponent<Renderer>().enabled = false;
        }
    }

    //DestroyAreaに触れたら敵は消え、プレイヤーはその場で固定し救出待ちに
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            //インスタンスの取得できた
           fallPlayerInstance = other.gameObject;
           fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
           waitRescue = true;
           CalcShortestDistCube();
           //   other.gameObject.GetComponent<PlayerController>().ChangePlayerMove(true);
        }
    }

    private Vector3 playerPos;
    private GameObject shortestDistCube;
    private float shortestDist = 0;
    private float dist;
    private void CalcShortestDistCube()
    {
        //最短距離の計算とそのcubeの取得
        foreach (GameObject cube in RescueActAreas)
        {
            cube.SetActive(true);
            playerPos = fallPlayerInstance.transform.position;
            dist = Vector3.Distance(cube.transform.position, playerPos);
            
            if (shortestDist == 0)
            {
                shortestDist = dist;
                shortestDistCube = cube;
            }
            else if (dist < shortestDist)
            {
                shortestDist = dist;
                shortestDistCube = cube;
            }
        }

        shortestDistCube.GetComponent<Renderer>().enabled = true;
        shortestDistCube.GetComponent<Rescue>().SetRescuedPlayer(fallPlayerInstance);
    }
   
}
