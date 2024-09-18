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

        m_springJoint.connectedBody = GameManager.instance.Pivot;
        m_springJoint.anchor = new Vector3(0, 10, 0);
        SetSpring(m_hingeJoint.spring, 1000, 10, true);
        m_springJoint.spring = 1000f;
        m_springJoint.damper = 0.1f;
        
        SetAxis();
    }

    private void AddJoint()
    {
        gameObject.AddComponent<HingeJoint>();
        gameObject.AddComponent<SpringJoint>();
        
        m_hingeJoint  = GetComponent<HingeJoint>();
        m_springJoint = GetComponent<SpringJoint>();
    }

    public void JointOff()
    {
        m_hingeJoint = GetComponent<HingeJoint>();
        Destroy(m_hingeJoint);
    }

    private void SetPivot()
    {
        m_hingeJoint.connectedBody = GameManager.instance.Pivot;
    }

    private void SetAxis()
    {
        //ここのX,Z上げて大げさにしても良い
        m_hingeJoint.axis = Vector3.Scale(GameManager.instance.Axis, new Vector3(1f, 0f, 1f));
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
