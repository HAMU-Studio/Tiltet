using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HingeManager : MonoBehaviour
{
    
    private HingeJoint m_hingeJoint;
    private SpringJoint m_springJoint;
    private Rigidbody m_pivotRB;

    //private RopeLine m_ropeLine;
    void Start()
    {
        GameManager.instance.RescueState = RescueState.None;
    }


    //最初からHingejointがあるとエラーが出るため、落下してからjointを追加する
    public void SetJointAndLine()
    {
        Rigidbody m_RB = gameObject.GetComponent<Rigidbody>();
        m_RB.freezeRotation = false;

        AddJoint();
      
        SetPivot();
        
        //この値によって挙動が変わってしまう。要注意
        m_hingeJoint.anchor = new Vector3(0, 10, 0);

        m_springJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
        m_springJoint.anchor = new Vector3(0, 10, 0);
        m_springJoint.spring = 1000f;
        m_springJoint.damper = 0.1f;
        
        SetSpring(m_hingeJoint.spring, 2000, 1, true);
        SetAxis();
        m_RB.AddForce(Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, -10f, 5f)), ForceMode.Impulse);
        
    }

    private void AddJoint()
    {
        Debug.Log("state = " + GameManager.instance.RescueState);
        
        gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
        m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    public void JointOff()
    {
        RopeLine ropeLine = GameManager.instance.Pivot.GetComponent<RopeLine>();
        ropeLine.ResetRope();
        
        m_hingeJoint = GetComponent<HingeJoint>();
        Destroy(m_hingeJoint);
        Destroy(m_springJoint);
    }

    private void SetPivot()
    {
        m_hingeJoint.connectedBody = GameManager.instance.Pivot.GetComponent<Rigidbody>();
    }

    private void SetAxis()
    {
        //ここのX,Z上げて大げさにしても良い
        m_hingeJoint.axis = Vector3.Scale(GameManager.instance.Axis, new Vector3(5f, 0f, 5f));
    }

    private void SetSpring(JointSpring jointSpring, float spring, float damper, bool isHinge)
    {
        JointSpring JS = jointSpring;   //m_hingeJoint.spring
        JS.spring = spring;             //1000
        JS.damper = damper;             //10

        if (isHinge)
        {
            m_hingeJoint.spring = JS;
            m_hingeJoint.useSpring = true; 
        }
    }
}
