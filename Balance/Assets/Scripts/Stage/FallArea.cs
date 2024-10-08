﻿using UnityEngine;

/// <summary>
/// 落ちた敵を消す、救出アクションの準備
/// </summary>
public class FallArea : MonoBehaviour
{
   
    [SerializeField] private GameObject[] RescueActAreas;
    private GameObject fallPlayerInstance;
    private PlayerManager m_PM;
   
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

    private void Update()
    {
        if (waitRescue)
        {
            //救出アクション終わった時（多分）
            if (m_PM.State == RescueState.None)
            {
                ResetFlag();
                PostProcess();
            }

            /*
            if (m_PM.RescueState == RescueState.SuperLand)
            {
                PostProcess();
            }*/
        }
    }

    private void SetPlayerManager()
    {
        m_PM = fallPlayerInstance.GetComponent<PlayerManager>();
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
            //ここ絶対エラー出るからどうにかしたい
           // if ( m_PM.RescueState != RescueState.None )
               // return;

            if (!waitRescue)
            {
                HitPlayerProcess(other);
                waitRescue = true;
            }
        }
    }

    private void ResetFlag()
    {
        waitRescue = false;
    }

    /// <summary>
    /// OnTriggerEnterでプレイヤーが触れた時の一連の処理 もう少し細かく分けたい
    /// </summary>
    private void HitPlayerProcess(Collider playerCol)
    {
        SetFallInstance(playerCol);
       
        //紐が二本つかないように
        if (m_PM.State != RescueState.None)
            return;
     
        m_PM.State = RescueState.Wait;
        CalcShortestDist();
        Debug.Log("state is " + m_PM.State);
        JointManager jointManager =  fallPlayerInstance.GetComponent<JointManager>();
        jointManager.SetJointAndLine();
    }

    private void SetFallInstance(Collider col)
    {
        fallPlayerInstance = col.gameObject;
        fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
        SetPlayerManager();
    }

    private Vector3 playerPos;
    private GameObject shortestDistCube;
    private float shortestDist = 0;
    private float dist;
    private void CalcShortestDist()
    {
        //最短距離の計算とそのcubeの取得
        //できれば他スクリプトで行いたい
        /*GameManager.instance.Pivot = null;
        GameManager.instance.Axis = Vector3.zero;*/
        foreach (GameObject cube in RescueActAreas)
        {
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
        
        if (shortestDistCube == null)
            Debug.LogError("shortestDistCube are null");
        
        shortestDistCube.SetActive(true);
        
        //最短距離の救出アクションエリアに対応するpivotを取得->振り子のためにRBと方向をセット
        GameObject childPivot = shortestDistCube.transform.GetChild(0).gameObject;
        GameManager.instance.Pivot = childPivot;
        
        childPivot.GetComponent<RopeLine>().SetEndPoint(fallPlayerInstance);
        
        GameManager.instance.Axis = (playerPos - childPivot.transform.position).normalized;
        
        //最短距離のオブジェクトだけon
        shortestDistCube.GetComponent<Renderer>().enabled = true;
        shortestDistCube.GetComponent<Rescue>().SetRescuedPlayer(fallPlayerInstance);
        
     //   shortestDistCube.GetComponent<Rescue>().SaveTarget(fallPlayerInstance.transform.position);
    }

    private void PostProcess()
    {
      //  shortestDistCube.GetComponent<Renderer>().enabled = false;
      //  shortestDistCube.SetActive(false);
      //  shortestDistCube = null;
      shortestDist = 0;
      foreach (GameObject area in RescueActAreas)
      {
          area.SetActive(false);
          area.GetComponent<Renderer>().enabled = false;
      }
    }
}
