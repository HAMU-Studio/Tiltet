using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HingeManager : MonoBehaviour
{
    
    private HingeJoint m_hingeJoint;
    private Rigidbody m_pivotRB;

    private RopeLine m_ropeLine;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (GameManager.instance.RescueState == RescueState.Throwing)
        {
            
        }
   
    }

    //最初からHingejointがあるとエラーが出るため、落下してからjointを追加する
    public void SetJointAndLine()
    {
        Rigidbody m_RB = gameObject.GetComponent<Rigidbody>();
        m_RB.freezeRotation = false;
        
        this.gameObject.AddComponent<HingeJoint>();
        m_hingeJoint = GetComponent<HingeJoint>();
      
        SetPivot();
        
        m_hingeJoint.axis = new Vector3(0f, 0f, 10f);
        m_hingeJoint.anchor = new Vector3(0f, 3f, 0f);
        

       // m_hingeJoint.useSpring = true;
        SetAxis();
        
        //ロープラインの終点指定
        m_ropeLine = GameManager.instance.Pivot.GetComponent<RopeLine>();
        m_ropeLine.SetEndPoint(GameManager.instance.Pivot.transform);
        
        
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
        m_hingeJoint.axis = GameManager.instance.Axis;
    }
}
