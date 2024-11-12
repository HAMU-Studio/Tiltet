using UnityEngine;

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
            if (m_PM.rescState == RescueState.None)
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
                Debug.Log("callHItPlayerProcess");
                HitPlayerProcess(other);
                waitRescue = true;
            }
        }
    }

    private void ResetFlag()
    {
        waitRescue = false;
     //   Debug.Log("reset waitRescue");
    }

    /// <summary>
    /// OnTriggerEnterでプレイヤーが触れた時の一連の処理 もう少し細かく分けたい
    /// </summary>
    private void HitPlayerProcess(Collider playerCol)
    {
        SetFallInstance(playerCol);
       
        //紐が二本つかないように
        if (m_PM.rescState != RescueState.None)
            return;
     
        m_PM.rescState = RescueState.Wait;
        CalcShortestDist();
      //  Debug.Log("state is " + m_PM.State);
        JointManager jointManager =  fallPlayerInstance.GetComponent<JointManager>();
        jointManager.SetJointAndLine();
    }

    private void SetFallInstance(Collider col)
    {
        fallPlayerInstance = col.gameObject;
        Debug.Log("FPI = " + fallPlayerInstance);
        fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
        SetPlayerManager();
    }

    private Vector3 playerPos;
    private GameObject shortestDistArea;
    private float shortestDist = 0;
    private float dist;
    private void CalcShortestDist()
    {
        //最短距離の計算とそのcubeの取得
        //できれば他スクリプトで行いたい
       
        foreach (GameObject area in RescueActAreas)
        {
            playerPos = fallPlayerInstance.transform.position;
            dist = Vector3.Distance(area.transform.position, playerPos);
            
            if (shortestDist == 0)
            {
                shortestDist = dist;
                shortestDistArea = area;
            }
            else if (dist < shortestDist)
            {
                shortestDist = dist;
                shortestDistArea = area;
            }
        }
        
        if (shortestDistArea == null)
            Debug.LogError("shortestDistCube are null");
        
     
        
        //最短距離の救出アクションエリアに対応するpivotを取得->振り子のためにRBと方向をセット
        GameObject childPivot = shortestDistArea.transform.GetChild(0).gameObject;
        GameManager.instance.Pivot = childPivot;
        
        childPivot.GetComponent<RopeLine>().SetEndPoint(fallPlayerInstance);
        
        GameManager.instance.Axis = (playerPos - childPivot.transform.position).normalized;
        
        //最短距離のオブジェクトだけon
        shortestDistArea.GetComponent<Renderer>().enabled = true;
        shortestDistArea.GetComponent<Rescue>().SetRescuedPlayer(fallPlayerInstance);
        shortestDistArea.SetActive(true);
        
     //   shortestDistCube.GetComponent<Rescue>().SaveTarget(fallPlayerInstance.transform.position);
    }

    private void PostProcess()
    {
      //  shortestDistCube.GetComponent<Renderer>().enabled = false;
      //  shortestDistCube.SetActive(false);
      m_PM = null;
      shortestDistArea = null;
      shortestDist = 0;
      foreach (GameObject area in RescueActAreas)
      {
        //  area.SetActive(false);
          area.GetComponent<Renderer>().enabled = false;
      }
    }
}
