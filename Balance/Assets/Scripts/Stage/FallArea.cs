using UnityEngine;
using Player;
using Player.Rescue;

/// <summary>
/// 落ちた敵を消す、救出アクションの準備
/// </summary>
public class FallArea : MonoBehaviour
{
    [SerializeField] private GameObject[] RescueActAreas;
    private GameObject fallPlayerInstance;
    private PlayerCondition playerCondition;
   
    public bool waitRescue;

    public bool WaitRescue
    {
        get { return waitRescue; }
    }

    private void Start()
    {
        waitRescue = false;
        shortestDist = 0;
        foreach (GameObject area in RescueActAreas)
        {
            area.GetComponent<MeshRenderer>().enabled = false;
            area.SetActive(false);
            area.GetComponent<Renderer>().enabled = false;
        }
    }

    private void Update()
    {
        if (waitRescue)
        {
            //救出アクション終わった時（多分）
            if (playerCondition.RescueState == State.None)
            {
                ResetFlag();
                PlayerFallPostProcess();
            }
        }
    }

    private void SetPlayerCondition()
    {
        playerCondition = fallPlayerInstance.GetComponent<PlayerCondition>();
    }

    //DestroyAreaに触れたら敵は消え、プレイヤーはその場で固定し救出待ちに
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            /*if (m_PM.rescState == RescueState.SuperLand)
            {
                Debug.Log("スーパー着地中に落下しました");
                m_PM.rescState = RescueState.None;
                ResetFlag();
                PostProcess();
            }*/

            if (!waitRescue)
            {
                //    Debug.Log("callHItPlayerProcess");
                HitPlayerProcess(other);
                waitRescue = true;
            }
            else
            {
                if (Player2Check(other) == true)
                {
                    other.GetComponent<PlayerCondition>().RescueState = State.Wait;
                    HitPlayerProcess(other);
                    StartCoroutine(GameManager.instance.GameOver());
                }
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
        if (playerCondition.RescueState != State.None)
            return;
     
        playerCondition.RescueState = State.Wait;
        CalcShortestDist();
      //  Debug.Log("state is " + m_PM.State);
        JointManager jointManager =  fallPlayerInstance.GetComponent<JointManager>();
        jointManager.SetJointAndLine();
    }

    private bool Player2Check(Collider playerCol)
    {
        SetFallInstance(playerCol);
        SetPlayerCondition();
        if (playerCondition.RescueState == State.None)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetFallInstance(Collider col)
    {
        fallPlayerInstance = col.gameObject;
     //   Debug.Log("FPI = " + fallPlayerInstance);
        fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
        SetPlayerCondition();
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
        MeshRenderer[] childrens=  shortestDistArea.GetComponentsInChildren<MeshRenderer>();
        childrens[0].enabled = false;
        childrens[1].enabled = false;
        shortestDistArea.GetComponent<RescueMovement>().SetRescuedPlayer(fallPlayerInstance);
        shortestDistArea.SetActive(true);
  
    }

    Renderer[] children = new Renderer[2];
    private void PlayerFallPostProcess()
    {
        playerCondition = null;
        shortestDistArea = null;
        shortestDist = 0;
        
        foreach (GameObject area in RescueActAreas)
        {
            //  area.SetActive(false);
            area.GetComponent<Renderer>().enabled = false;
            children = area.GetComponentsInChildren<Renderer>();
            children[0].enabled = false;
            children[1].enabled = false;
        }
    }
}
