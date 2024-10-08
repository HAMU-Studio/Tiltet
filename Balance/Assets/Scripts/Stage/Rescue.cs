using System;
using UnityEngine;

public class Rescue : MonoBehaviour
{
    private Rigidbody m_RB;
    private GameObject rescuePlayer;
    private bool canRescueAct;
    private Vector3 direction;
 
    void Start()
    {
        canRescueAct = false;
        isThrowing = false;
        once = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canRescueAct = true;
            rescuePlayer = other.gameObject;
            Debug.Log("canRescueAct = " + canRescueAct);
        }
    }
    
    private Vector3 m_targetVec;
    public void SaveTarget(Vector3 targetVector)
    {
        m_targetVec = targetVector;
        
        //ここの座標で透明なcube作成、onTriggerで到着したか判定させたい
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canRescueAct = false;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //着地したらFreezePositionをオンオフし着地後の余計な動き抑制
        //m_RB = rescuedPlayer.GetComponent<Rigidbody>();
        if (collision.rigidbody == m_RB)
        {
            RescPostProcess();
        }
    }

    private GameObject rescuedPlayer;
    private PlayerManager m_PM;
    public void SetRescuedPlayer(GameObject Player)
    {
        rescuedPlayer = Player;
        m_PM = rescuedPlayer.GetComponent<PlayerManager>();
        m_RB = rescuedPlayer.GetComponent<Rigidbody>();
    }
    
    [Header("救出アクションで飛ばす先(反対側の板)")]
    [SerializeField] private GameObject m_throwPoint;
    private bool isThrowing;
    
    [Header("射出角度")]
    [SerializeField] float m_Angle = 60;
    
    public void RescueThrow()
    {
       
        //この辺構造おかしいこの関数は救出アクション中着地するまで実行し続けるべき
        if (canRescueAct == false)
        {
            return;
        }
      
        ThrowPREP();
       　//射出速度を算出
        Vector3 velocity = CalclateVelocity( rescuedPlayer.transform.position,m_throwPoint.transform.position, m_Angle);
    
        rescuedPlayer.GetComponent<PlayerController>().ChangePlayerState(false);

        ResetRBVelocity();
        m_RB.velocity = velocity;

        GetComponent<Renderer>().enabled = false;
    }
   
    /// <param name="pointA">飛ばす元(落ちたプレイヤー)</param>
    /// <param name="pointB">飛ばす先</param>
    /// <param name="angle">射出角度</param>
    private Vector3 CalclateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;
        
        //水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        
        //垂直方向の距離y
        float y = pointA.y - pointB.y;
        
        //斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) /
                                 (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        //Not a Number 0除算とか無効な操作を検知
        if (float.IsNaN(speed))
        {
            //条件を満たす初速を産出できなければzeroベクトルを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    /// <summary>
    /// 救出アクションの直前処理。ステージが引っ掛かりそうなら外に移動->ロープ切る->飛ばす
    /// </summary>
    private void ThrowPREP()
    {
        //ロープ作成時に回転制限オフにしたため
        m_RB.freezeRotation = true;
    }

    private void ResetRBVelocity()
    {
        m_RB = rescuedPlayer.GetComponent<Rigidbody>();
        m_RB.velocity = Vector3.zero;
        m_RB.angularVelocity = Vector3.zero;
    }

    private void RescPostProcess()
    {
        m_RB.constraints |= RigidbodyConstraints.FreezePosition;
       
        m_RB.constraints &= ~RigidbodyConstraints.FreezePosition;
        
        rescuedPlayer.GetComponent<PlayerController>().ChangePlayerCanMove(false);
        
        canRescueAct = false;
        isThrowing = false;
        once = false;
    }

    public void StartRescue()
    {
        m_PM.RescueState = RescueState.Move;
        //rescuedPlayer.GetComponent<JointManager>().RescueAdjust();
    }

    private bool once;
    private void Update()
    {
        if (once)
           return;
        
        if (m_PM.RescueState == RescueState.Fly)
        {
            RescueThrow();
            once = true;
        }
    }

}
