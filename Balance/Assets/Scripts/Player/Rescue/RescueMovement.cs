using Player;
using Player.Rescue;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RescueMovement : MonoBehaviour
{
    private bool canAccepted = true;   // 落下したプレイヤーを受付可能か
    
    private Rigidbody m_RB;
    private GameObject rescuePlayer;
    private bool canRescueAct;
    private Vector3 direction;
    
    private GameObject rescuedPlayer;
    private PlayerCondition m_condition;
 
    void Start()
    {
        canRescueAct = false;
        isThrowing = false;
        once = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }

    public bool CanAccepted
    {
        get { return canAccepted; }
        set { canAccepted = value; }
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
        if (m_condition.RescueState == State.Wait)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                canRescueAct = false;
            }
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // 着地したらFreezePositionをオンオフし着地後の余計な動き抑制
        if (collision.rigidbody == m_RB)
        {
            RescPostProcess();
        }
    }
    
    public void SetRescuedPlayer(GameObject Player)
    {
        rescuedPlayer = Player;
        m_condition = rescuedPlayer.GetComponent<PlayerCondition>();
        m_condition.OnStateChange += OnStateChange; 
        m_RB = rescuedPlayer.GetComponent<Rigidbody>();
    }
    
    [Header("救出アクションで飛ばす先(反対側の板)")]
    [SerializeField] private GameObject m_throwPoint;
    private bool isThrowing;
    
    [Header("射出角度")]
    [SerializeField] float m_Angle = 60;
    
    private void RescueThrow()
    {
        if (canRescueAct == false) 
            return;
        
        ThrowPREP();
       　//射出速度を算出
        Vector3 velocity = CalclateVelocity( rescuedPlayer.transform.position,m_throwPoint.transform.position, m_Angle);
    
        rescuedPlayer.GetComponent<PlayerController>().ChangePlayerState(false);

        if (m_RB.isKinematic)
            m_RB.isKinematic = false;
        
        GameManager.instance.ResetRBVelocity(m_RB);
        m_RB.velocity = velocity;
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
    /// JointManagerで使用する、外側に弾く力を計算する関数。
    /// </summary>
    /// <returns></returns>
    public Vector3 CalcOutsideForce()
    {
        Vector3 velocity = CalclateVelocity( rescuedPlayer.transform.position,m_throwPoint.transform.position, m_Angle);

        velocity = new Vector3(-velocity.x, 0f, -velocity.z);

        return velocity.normalized;
    }

    /// <summary>
    /// 救出アクションの直前処理。
    /// </summary>
    private void ThrowPREP()
    {
        //ロープ作成時に回転制限オフにしたため
        m_RB.freezeRotation = true;
    }

    private void RescPostProcess()
    {
        m_RB.constraints |= RigidbodyConstraints.FreezePosition;
       
        m_RB.constraints &= ~RigidbodyConstraints.FreezePosition;
        
        canRescueAct = false;
        isThrowing = false;
        once = false;
        m_condition.OnStateChange -= OnStateChange;
        m_condition = null;
        gameObject.SetActive(false);
    }

    public void StartRescue()
    {
        m_condition.RescueState = State.OutsideMove;
        RescAreaDisable();
    }

    /// <summary>
    /// 視覚的な無効化
    /// </summary>
    private void RescAreaDisable()
    {
        MeshRenderer[] childrens = GetComponentsInChildren<MeshRenderer>();
        childrens[1].enabled = false;
        childrens[2].enabled = false;
    }

    private void OnStateChange(RescueEventArgs args)
    {
        if (args.PreviousState == State.SuperLand || args.PreviousState == State.Fly)
        {
            if (args.CurrentState == State.None || args.CurrentState == State.Wait)
                RescPostProcess();
        }
    }

    private bool once;
    private void Update()
    {
        if (once)
        {
      
            return;
        }
        
        if (m_condition.RescueState == State.Fly)
        {
            RescueThrow();
            once = true;
        }
    }
    
    [FormerlySerializedAs("upPowoer")] [Header("上方向の力加える倍率")] [SerializeField] private float upPower = 2f;
    private void FixedUpdate()
    {
        if (m_condition.RescueState == State.OutsideMove)
        {
            Vector3 force = Vector3.up * upPower / Time.fixedDeltaTime;
            m_RB.AddForce(force);
        }
    }
}
