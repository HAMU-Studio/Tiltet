using System;
using System.Collections;
using Unity.Mathematics;
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
        
        //この値によって挙動が変わってしまう。要注意 ->AutoConnectedAnchorだから関係ないかも
        m_hingeJoint.anchor = new Vector3(0, 10, 0);

        m_springJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
       
        m_springJoint.spring = 30f;
        m_springJoint.damper = 0.2f;
       
        //GameManagerのAxisは二点間のベクトル、それを軸とすると手前側と奥側の挙動がおかしくなる
        Vector3 velocity = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f));
        SetAxis(velocity);
     
        m_RB.AddForce(velocity, ForceMode.Impulse);
    }

    private void AddJoint()
    {
        //Debug.Log("state = " + GameManager.instance.RescueState);
        
        gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
        m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    private void JointOff()
    {
        m_hingeJoint = GetComponent<HingeJoint>();
        
        Destroy(m_hingeJoint);
        Destroy(m_springJoint);
        
        GameManager.instance.ResetRBVelocity(m_RB);
        
        m_RB.rotation = quaternion.identity;
    
        // 一時的にRigidbodyを固定
        m_RB.isKinematic = true;
        StartCoroutine(ReleaseAndAddForce());
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

    private void SetAxis(Vector3 velocity)
    {
        //二点間のベクトル利用してaxisを設定すると手前と奥だけ挙動がおかしくなる -> axisを全部0にすると動いてくれる
        
        m_direction = GameManager.instance.Pivot.GetComponent<DirectionManager>().direction;
     
        m_hingeJoint.axis = Vector3.zero;
    }
    
    private void GetPlayerManager()
    {
        m_PM = GetComponent<PlayerManager>();
    }

  　private float lowerLimit = 10f;
   /// <summary>
   /// ある程度離れていたら飛ばす
   /// </summary>
    private float CheckDistanceFromStage()
    {
        Vector3 pivotPos = GameManager.instance.Pivot.transform.position;
        
        float dist = Vector3.Distance(transform.position, pivotPos);

        return dist;
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

    private void FixedUpdate()
    {
        RescueAdjust();
    }
    
    [Header("外側に弾く力の倍率")]
    [SerializeField]
    private Vector3 force = new Vector3(5f, 1f, 5f);
    private bool onceForce;

    /// <summary>
    /// 救出アクションでステージの引っかかり防止に使う　ステージが引っ掛かりそうなら外に移動->飛ばす->ロープ切る
    /// jointあまり関係ないから違うスクリプトに移したい
    /// </summary>
    private Vector3 direction; 
    private void RescueAdjust()
    {
        //もう少し細かく分けたい
        if (m_PM.rescState == RescueState.Move)
        {
            if (CheckDistanceFromStage() > lowerLimit)
            {
                StartCoroutine("DelayFly");
            }
            
            if (onceForce)
                return;
            
            m_direction = GameManager.instance.Pivot.GetComponent<DirectionManager>().direction;
            
            //正面方向の救出アクションのみ符号反転すれば正常に動く->そんなことなかった
            /*if (m_direction == Direction.Foward)
            {
                direction = Vector3.forward;
            }
            else if (m_direction == Direction.Back)
            {
                direction = Vector3.back;
            }
            else if (m_direction == Direction.Left)
            {
                direction = Vector3.left;
            }
            else if (m_direction == Direction.Right)
            {
                direction = Vector3.right;
            }*/

            SetOutsideForce();
            
            if (CheckDistanceFromStage() <= 4f)
            {
                //距離がかなり近い時はさらに飛ばす
                force =  new Vector3(force.x, Mathf.Pow(force.y, 0), force.z);
                direction = Vector3.Scale(direction, force);
            }
            
            //Yは0乗して1に、力の調整しない
            force =  new Vector3(force.x, Mathf.Pow(force.y, 0), force.z);
            direction = Vector3.Scale(direction, force);

           JointOff();
    
           onceForce = true;
        }
    }

    private void SetOutsideForce()
    {
        if (GameManager.instance.Pivot != null)
        {
            Debug.Log("pivot = " + GameManager.instance.Pivot);
        }

        direction = GameManager.instance.Pivot.GetComponentInParent<Rescue>().CalcOutsideForce();
        
    }
    //axisを0にすると最初の揺れは合ってるけど外側に力を加えた時正しく動いてくれない -> 消したはずのjointの影響が残っていたせいだった。
    //一時的にisKinematicをonにすれば直った
    private IEnumerator ReleaseAndAddForce()
    {
        yield return new WaitForSeconds(0.1f); 　//物理挙動がおかしくならないように少し待つ

        if (m_RB.isKinematic == false)
        {
            //弾く必要なし
            yield break;
        }
        
        m_RB.isKinematic = false; 　//再度物理的に解放
        
        Debug.Log("call OutsideForce");
        //ステージの反対方向に、上方向は徐々に力加える。 呼ばれる場所が違うの要修正
        m_RB.AddForce(direction, ForceMode.Impulse);
    }

    private IEnumerator DelayFly()
    {
        m_PM.rescState = RescueState.Fly;
        
        yield return new WaitForSeconds(0.7f);
      
        RopeOff();
        onceForce = false;
    }
}
