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
        }
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
            //これ意味ない説
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

        GameManager.instance.ResetRBVelocity(m_RB);
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
    /// 救出アクションの直前処理。
    /// </summary>
    private void ThrowPREP()
    {
        //ロープ作成時に回転制限オフにしたため
        m_RB.freezeRotation = true;
     //   GameManager.instance.ResetRBVelocity(m_RB);
    }

    private void RescPostProcess()
    {
       // Debug.Log("call PostProcess");
        m_RB.constraints |= RigidbodyConstraints.FreezePosition;
       
        m_RB.constraints &= ~RigidbodyConstraints.FreezePosition;
        
       // rescuedPlayer.GetComponent<PlayerController>().ChangePlayerCanMove(false);
        
        canRescueAct = false;
        isThrowing = false;
        once = false;
        m_PM = null;
        gameObject.SetActive(false);
    }

    public void StartRescue()
    {
        m_PM.State = RescueState.Move;
        //rescuedPlayer.GetComponent<JointManager>().RescueAdjust();
    }

    private bool once;
    private void Update()
    {
        if (once)
        {
            if (m_PM.State == RescueState.SuperLand || m_PM.State == RescueState.None)
            {
                RescPostProcess();
            }
            return;
        }
        
        if (m_PM.State == RescueState.Fly)
        {
            RescueThrow();
            once = true;
        }
    }
    
    [Header("上方向の力加える倍率")] [SerializeField] private float upPowoer = 2f;
    private void FixedUpdate()
    {
        if (m_PM.State == RescueState.Move)
        {
            Vector3 force = Vector3.up * upPowoer / Time.fixedDeltaTime;
            m_RB.AddForce(force);
        }
    }
}
