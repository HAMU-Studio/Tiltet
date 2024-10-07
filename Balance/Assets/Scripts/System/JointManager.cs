using System.Collections;
using UnityEngine;

public class JointManager : MonoBehaviour
{
    private HingeJoint m_hingeJoint;
    private SpringJoint m_springJoint;
    private Rigidbody m_pivotRB;

    private PlayerManager m_PM; 　 //インスタンスから取得、操作
    private Direction m_direction;

    //private RopeLine m_ropeLine;
    void Start()
    {
        GetPlayerManager();
        onceForce = false;
    }

    //最初からHingejointがあるとエラーが出るため、落下してからjointを追加する
    private Rigidbody m_RB;
    public void SetJointAndLine()
    {
        m_RB = gameObject.GetComponent<Rigidbody>();
        m_RB.freezeRotation = false;

        AddJoint();
      
        SetPivot();
        
        //この値によって挙動が変わってしまう。要注意
        m_hingeJoint.anchor = new Vector3(0, 10, 0);

        m_springJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
       
        m_springJoint.spring = 15f;
        m_springJoint.damper = 0.2f;
        
        SetAxis();
        
        Vector3 force = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f));
        m_RB.AddForce(force, ForceMode.Impulse);
    }

    private void AddJoint()
    {
        //Debug.Log("state = " + GameManager.instance.RescueState);
        
        gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
        m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    public void JointOff()
    {
        m_hingeJoint = GetComponent<HingeJoint>();
        Destroy(m_hingeJoint);
        Destroy(m_springJoint);
        m_RB.freezeRotation = true;
    }

    public void RopeOff()
    {
        RopeLine ropeLine = GameManager.instance.Pivot.GetComponent<RopeLine>();
        ropeLine.ResetRope();
    }

    private void SetPivot()
    {
        m_hingeJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
    }

    private void SetAxis()
    {
        //ここのX,Z上げて大げさにしても良い
        m_hingeJoint.axis = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, 1f, 5f));
    }
    
    private void GetPlayerManager()
    {
        m_PM = GetComponent<PlayerManager>();
    }

  　private float lowerLimit = 10f;
   /// <summary>
   /// ある程度離れていたら飛ばす
   /// </summary>
    private void CheckDistanceFromStage()
    {
        Vector3 pivotPos = GameManager.instance.Pivot.transform.position;
        
        float dist = Vector3.Distance(transform.position, pivotPos);

       // Debug.Log("dist = " + dist);
        if (dist >= lowerLimit)
        {
            StartCoroutine("DelayFly");
        }
    }

    /// <summary>
    /// AnchorのXとZを徐々に減らしていけば真下にぶら下げれそう->その影響を受けてなのか大暴れしだす
    /// </summary>
    private Vector3 temp;
    private void DecreaseAnchorXZ()
    {
        /*if (m_springJoint.autoConfigureConnectedAnchor)
        {
            temp = m_springJoint.connectedAnchor;

            m_springJoint.anchor = temp;
         
            m_springJoint.autoConfigureConnectedAnchor = false;
        }
        else
        {
            temp = m_springJoint.anchor;
        }
   
        
        if (temp.x < 0)
        {
            temp.x += 0.1f;
        }
        else if (temp.x > 0)
        {
            temp.x -= 0.1f;
        }
        
        if (temp.z < 0)
        {
            temp.z += 0.1f;
        }
        else if (temp.z > 0)
        {
            temp.z -= 0.1f;
        }
        
        Vector3 originalVelocity = m_RB.velocity;
        m_RB.isKinematic = true;

        m_springJoint.anchor = temp;
        
        m_RB.isKinematic = false;
        m_RB.velocity = originalVelocity;*/
    }

    private void Update()
    {
        /*if (m_PM.RescueState == RescueState.Wait)
        {
            //DecreaseAnchorXZ();
        }*/
    }
    private Vector3 force = new Vector3(-50f, 1f, -50f);
    private bool onceForce;
    private void FixedUpdate()
    {
        RescueAdjust();
    }
    
    /// <summary>
    /// 救出アクションでステージの引っかかり防止に使う
    /// jointあまり関係ないから違うスクリプトに移したい
    /// </summary>
    private void RescueAdjust()
    {
        //もう少し細かく分けたい
        if (m_PM.RescueState == RescueState.Move)
        {
            CheckDistanceFromStage();
            
            if (onceForce)
                return;

            //pivotと落下したプレイヤー、二点間のベクトルを取得->Y軸以外反転
            Vector3 direction = GameManager.instance.Pivot.transform.position;
            
            direction = (direction - transform.position).normalized;
            direction = Vector3.Scale(direction, force);
            
            m_direction = GameManager.instance.Pivot.GetComponent<DirectionManager>().direction;

            //breakForce設定してちょっと力加われば自動で千切れるように(Componentごと消える)
            m_hingeJoint.breakForce = 5f;
            m_springJoint.breakForce = 5f;
            
            //正面方向の救出アクションのみ符号反転すれば正常に動く
            if (m_direction == Direction.Foward)
            {
                if (Mathf.Sign(direction.x) < 0)
                {
                    direction = Vector3.Scale(direction, new Vector3(-1f, -1f, 1f));
                }

                if (Mathf.Sign(direction.z) < 0)
                {
                    direction = Vector3.Scale(direction, new Vector3(1f, 1f, -1f));
                }
            }
            
            //ステージの反対方向と上方向に力加える
            
            m_RB.AddForce(direction, ForceMode.Impulse);
            m_RB.AddForce(Vector3.up * 5);
            
            onceForce = true;
        }
    }

    private IEnumerator DelayFly()
    {
        JointOff();
        m_PM.RescueState = RescueState.Fly;
        
        yield return new WaitForSeconds(0.7f);
        
        RopeOff();
        
        onceForce = false;
    }
}
