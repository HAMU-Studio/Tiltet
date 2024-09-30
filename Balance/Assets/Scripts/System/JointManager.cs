using System;
using System.Collections;
using UnityEngine;

public class JointManager : MonoBehaviour
{
  //  private HingeJoint m_hingeJoint;
    private SpringJoint m_springJoint;
    private Rigidbody m_pivotRB;

    private PlayerManager m_PM; //インスタンスから取得、操作

    //private RopeLine m_ropeLine;
    void Start()
    {
        GetPlayerManager();
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
        //  m_hingeJoint.anchor = new Vector3(0, 10, 0);

        m_springJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
        //  m_springJoint.anchor = new Vector3(0, 10, 0);
        m_springJoint.spring = 100f;
        m_springJoint.damper = 0.2f;
        
        //SetSpring(m_hingeJoint.spring, 2000, 1, true);
        m_RB.freezeRotation = true;
        SetAxis();
        
        Vector3 force = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f));
        m_RB.AddForce(force, ForceMode.Impulse);
        
    }

    private void AddJoint()
    {
        //Debug.Log("state = " + GameManager.instance.RescueState);
        
      //gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
     //m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    public void JointOff()
    {
     //   m_hingeJoint = GetComponent<HingeJoint>();
    //    Destroy(m_hingeJoint);
        Destroy(m_springJoint);
    }

    public void RopeOff()
    {
        RopeLine ropeLine = GameManager.instance.Pivot.GetComponent<RopeLine>();
        ropeLine.ResetRope();
    }

    private void SetPivot()
    {
   //     m_hingeJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
    }

    private void SetAxis()
    {
        //ここのX,Z上げて大げさにしても良い
  //      m_hingeJoint.axis = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, 0f, 5f));
    }

    private void SetSpring(JointSpring jointSpring, float spring, float damper, bool isHinge)
    {
        /*JointSpring JS = jointSpring;   //m_hingeJoint.spring
        JS.spring = spring;             //1000
        JS.damper = damper;  */           //10

        if (isHinge)
        {
          //  m_hingeJoint.spring = JS;
         //   m_hingeJoint.useSpring = true; 
        }
    }

    /// <summary>
    /// 救出アクションでステージの引っかかり防止に使う
    /// </summary>
    public void RescueAdjust()
    {
        SetMotor();
        SetLimit();
      //  m_hingeJoint.useSpring = false;
    }
    private void SetMotor()
    {
        /*JointMotor motor = m_hingeJoint.motor;
        motor.force = 100;
        motor.targetVelocity = 90;
        motor.freeSpin = false;
        m_hingeJoint.motor = motor;
        m_hingeJoint.useMotor = true;*/
    }

    private void SetLimit()
    {
        /*JointLimits limits = m_hingeJoint.limits;
        limits.max = 50f;
        m_hingeJoint.limits = limits;
        m_hingeJoint.useLimits = true;*/
    }

    private void GetPlayerManager()
    {
        m_PM = GetComponent<PlayerManager>();
    }

    private void CheckDistanceFromStage()
    {
        Vector3 pivotPos = GameManager.instance.Pivot.transform.position;
        
        float dist = Vector3.Distance(transform.position, pivotPos);

        Debug.Log("dist = " + dist);
        if (dist >= 7f)
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
        if (m_PM.RescueState == RescueState.Wait)
        {
            //DecreaseAnchorXZ();
           
        }
        
        /*if (m_PM.RescueState == RescueState.Throwing)
           StartCoroutine("DelayFly");*/
    }

    private void FixedUpdate()
    {
        if (m_PM.RescueState == RescueState.Move)
        {
            CheckDistanceFromStage();
            
            Vector3 force = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f));
            m_RB.AddForce(force);
        }
    }

    private IEnumerator DelayFly()
    {
        Debug.Log("CallDelayFly");
        JointOff();
        m_PM.RescueState = RescueState.Fly;
        
        yield return new WaitForSeconds(0.7f);
        
        RopeOff();
       
    }
}
