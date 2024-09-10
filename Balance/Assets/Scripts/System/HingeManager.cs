using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HingeManager : MonoBehaviour
{
    
    private HingeJoint m_hingeJoint;
    private Rigidbody m_pivotRB;

    //private RopeLine m_ropeLine;
    void Start()
    {
        GameManager.instance.RescueState = RescueState.None;
    }
    
    void Update()
    {
     
   
    }

    //最初からHingejointがあるとエラーが出るため、落下してからjointを追加する
    public void SetJointAndLine()
    {
        Rigidbody m_RB = gameObject.GetComponent<Rigidbody>();
        m_RB.freezeRotation = false;
        
        this.gameObject.AddComponent<HingeJoint>();
        m_hingeJoint = GetComponent<HingeJoint>();
      
        SetPivot();
        
        //この値によって挙動が変わってしまう。要注意
        m_hingeJoint.anchor = new Vector3(0, 10, 0);

        SetSpring();
        SetAxis();
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
        m_hingeJoint.axis = Vector3.Scale(GameManager.instance.Axis, new Vector3(1f, 0f, 1f));
        //m_hingeJoint.axis.y = 
    }

    private void SetSpring()
    {
        JointSpring hingeSpring = m_hingeJoint.spring;
        hingeSpring.spring = 100;
        hingeSpring.damper = 10;
        m_hingeJoint.spring = hingeSpring;
        m_hingeJoint.useSpring = true;
    }
}
