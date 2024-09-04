using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HingeManager : MonoBehaviour
{
    // Start is called before the first frame update
    private HingeJoint m_hingeJoint;
    private Rigidbody m_pivotRB;
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.RescueState == RescueState.Throwing)
        {
            
        }
    }

    public void SetJoint()
    {
        this.gameObject.AddComponent<HingeJoint>();
        m_hingeJoint = GetComponent<HingeJoint>();
      
        m_hingeJoint.anchor = new Vector3(0f, 3f, 0f);
        SetPivot();
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
        m_hingeJoint.axis = GameManager.instance.Axis;
    }
}
