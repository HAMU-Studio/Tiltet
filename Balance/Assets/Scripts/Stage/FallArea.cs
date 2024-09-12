﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 落ちた敵を消す、救出アクションの準備
/// </summary>
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
            if ( GameManager.instance.RescueState != RescueState.None)
                return;
            
            //インスタンスの取得できた
           fallPlayerInstance = other.gameObject;
           fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
           waitRescue = true;
           GameManager.instance.RescueState = RescueState.Wait;
           CalcShortestDist();

           fallPlayerInstance.GetComponent<HingeManager>().SetJointAndLine();
        }
    }

    private Vector3 playerPos;
    private GameObject shortestDistCube;
    private float shortestDist = 0;
    private float dist;
    private void CalcShortestDist()
    {
        //最短距離の計算とそのcubeの取得
        //できれば他スクリプトで行いたい
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
        
        // shortestDistCube.SetActive(true);
        
        //最短距離の救出アクションエリアに対応するpivotを取得->振り子のためにRBと方向をセット
        GameObject childPivot = shortestDistCube.transform.GetChild(0).gameObject;
        GameManager.instance.Pivot = childPivot.GetComponent<Rigidbody>();
        //childPivot.SetActive(true);
        
        childPivot.GetComponent<RopeLine>().SetEndPoint(fallPlayerInstance);
        
        GameManager.instance.Axis = (playerPos - childPivot.transform.position).normalized;
        
        shortestDistCube.GetComponent<Renderer>().enabled = true;
        shortestDistCube.GetComponent<Rescue>().SetRescuedPlayer(fallPlayerInstance);
    }

}
