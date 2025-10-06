using Player;
using System;
using Player.Rescue;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class JointManager : MonoBehaviour
{
    private HingeJoint m_hingeJoint;
    private SpringJoint m_springJoint;
    private Rigidbody m_pivotRB;

    private PlayerCondition m_condition; 　 //インスタンスから取得、操作
    private Rigidbody m_RB;
    
    void Start()
    {
        GetPlayerManager();
        onceForce = false;
        m_RB = gameObject.GetComponent<Rigidbody>();
    }

    //最初からHingejointがあるとエラーが出るため、落下してからjointを追加する
   
    public void SetJointAndLine()
    {
        m_RB.freezeRotation = false;

        AddJoint();
        SetSpring();
        SetPivot();
        
        //この値によって挙動が変わってしまう。要注意 ->AutoConnectedAnchorだから関係ないかも
        m_hingeJoint.anchor = m_hingeJoint.connectedAnchor;
        SetLimit();

        m_springJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
        m_springJoint.spring = 30f;
        m_springJoint.damper = 0.2f;
       
        // GameManagerのAxisは二点間のベクトル、それを軸とすると手前側と奥側の挙動がおかしくなる
        Vector3 velocity = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f));
        SetAxis(velocity);
        
        m_RB.AddForce(velocity, ForceMode.Impulse);
    }
    
    private void SetSpring()
    {
        JointSpring hingeSpring = m_hingeJoint.spring;
        hingeSpring.spring = 100;
        hingeSpring.damper = 200;
        m_hingeJoint.spring = hingeSpring;
        m_hingeJoint.useSpring = true;
    }

    private void SetLimit()
    {
        JointLimits hingeLimits = m_hingeJoint.limits;
        hingeLimits.max = 30;
        hingeLimits.min = -30;
        m_hingeJoint.limits = hingeLimits;
        m_hingeJoint.useLimits = true;
    }

    private void AddJoint()
    {
        gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
        m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    public void Reset()
    {
        m_hingeJoint = GetComponent<HingeJoint>();
        
        Destroy(m_hingeJoint);
        Destroy(m_springJoint);
        
        GameManager.instance.ResetRBVelocity(m_RB);
        
        m_RB.rotation = quaternion.identity;
      
        m_RB.isKinematic = true;

        StartCoroutine(WaitAndRelease());
        m_RB.freezeRotation = true;
    
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

    private void RopeOff()
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
     
        m_hingeJoint.axis = Vector3.zero;
    }
    
    private void GetPlayerManager()
    {
        m_condition = GetComponent<PlayerCondition>();
    }

  　private float lowerLimit = 15f;
   /// <summary>
   /// ある程度離れていたら飛ばす
   /// </summary>
    private float CheckDistanceFromStage()
    {
        Vector3 pivotPos = GameManager.instance.Pivot.transform.position;
        
        float dist = Vector3.Distance(transform.position, pivotPos);

        return dist;
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
        if (m_condition.RescueState == State.OutsideMove)
        {
            if (CheckDistanceFromStage() > lowerLimit)
            {
                StartCoroutine("DelayFly");
            }
            
            if (onceForce)
                return;
            
            SetOutsideForce();
            
            if (CheckDistanceFromStage() <= 4f)
            {
                // 距離がかなり近い時はさらに飛ばす
                force =  new Vector3(force.x, Mathf.Pow(force.y, 0), force.z);
                direction = Vector3.Scale(direction, force);
            }
            
            // Yは0乗して1に、力の調整しない
            force =  new Vector3(force.x, Mathf.Pow(force.y, 0), force.z);
            direction = Vector3.Scale(direction, force);

           JointOff();
    
           onceForce = true;
        }
    }

    private void SetOutsideForce()
    {
        if (GameManager.instance.Pivot == null)
        {
            Debug.LogError("pivot is null! ");
            return;
        }
        direction = GameManager.instance.Pivot.GetComponentInParent<RescueMovement>().CalcOutsideForce();
    }
    
    // axisを0にすると最初の揺れは合ってるけど外側に力を加えた時正しく動いてくれない -> 消したはずのjointの影響が残っていたせいだった。
    // 一時的にisKinematicをonにすれば直った
    private IEnumerator ReleaseAndAddForce()
    {
        yield return new WaitForSeconds(0.1f); 　// 物理挙動がおかしくならないように少し待つ

        if (m_RB.isKinematic == false)
        {
            // 弾く必要なし
            yield break;
        }
        
        m_RB.isKinematic = false; 　// 再度物理的に解放
      
        // ステージの反対方向に、上方向は徐々に力加える。 呼ばれる場所が違うの要修正
        m_RB.AddForce(direction, ForceMode.Impulse);
    }

    private IEnumerator WaitAndRelease()
    {
        yield return new WaitForSeconds(0.1f); // 物理挙動がおかしくならないように少し待つ

        if (m_RB.isKinematic == false)
        {
            // 弾く必要なし
            yield break;
        }

        m_RB.isKinematic = false; // 再度物理的に解放
    }

    private IEnumerator DelayFly()
    {
        m_condition.RescueState = State.Fly;
        
        yield return new WaitForSeconds(0.7f);
      
        RopeOff();
        onceForce = false;
    }
}
