using UnityEngine;
using Player;

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
            if (!waitRescue)
            {
                FallPlayerProcess(other);
                waitRescue = true;
            }
            else
            {
                if (Player2Check(other))
                {
                    if (playerCondition.RescueState != State.Wait)
                    {
                        FallPlayerProcess(other);
                        return;
                    }

                    FallPlayerProcess(other);
                    StartCoroutine(GameManager.instance.GameOver());
                }
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
    private void FallPlayerProcess(Collider playerCol)
    {
        SetFallInstance(playerCol);
       
        //紐が二本つかないように
        if (playerCondition.RescueState != State.None)
            return;
     
        playerCondition.RescueState = State.Wait;
        CalcShortestDist();
        JointManager jointManager =  fallPlayerInstance.GetComponent<JointManager>();
        jointManager.SetJointAndLine();
    }

    private bool Player2Check(Collider playerCol)
    {
        if (playerCol.GetComponent<PlayerCondition>().RescueState == State.None)
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
        fallPlayerInstance.GetComponent<PlayerController>().ChangePlayerState(true);
        SetPlayerCondition();
    }

    private Vector3 playerPos;
    private GameObject shortestDistArea;
    private GameObject nextShortestDistArea;
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
                
                nextShortestDistArea = shortestDistArea;
                shortestDistArea = area;
            }
        }
        
        if (shortestDistArea == null)
            Debug.LogError("shortestDistCube are null");


        // もうプレイヤーが付いている(fly中など)場合は二番目に近いPivotにぶら下げる
        if (shortestDistArea.GetComponent<RescueMovement>().CanAccepted == false)
        {
            shortestDistArea = nextShortestDistArea;
            Debug.Log("最短距離のAreaは使用中です");
        }
        
        //最短距離の救出アクションエリアに対応するpivotを取得->振り子のためにRBと方向をセット
        GameObject childPivot = shortestDistArea.transform.GetChild(0).gameObject;
        GameManager.instance.Pivot = childPivot;
        
        childPivot.GetComponent<RopeLine>().SetEndPoint(fallPlayerInstance);
        
        GameManager.instance.Axis = (playerPos - childPivot.transform.position).normalized;
        
        //最短距離のオブジェクトだけon
        MeshRenderer[] childrens =  shortestDistArea.GetComponentsInChildren<MeshRenderer>();
        //childrens[0].enabled = true;
        childrens[1].enabled = true;
        RescueMovement rescue = shortestDistArea.GetComponent<RescueMovement>();
        rescue.SetRescuedPlayer(fallPlayerInstance);
        rescue.CanAccepted = false;
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
            area.GetComponent<Renderer>().enabled = false;
            children = area.GetComponentsInChildren<Renderer>();
            children[0].enabled = false;
            children[1].enabled = false;
        }
    }
}
