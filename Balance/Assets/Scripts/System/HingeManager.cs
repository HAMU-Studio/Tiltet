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
        m_hingeJoint = GetComponent<HingeJoint>();
        m_hingeJoint.connectedBody = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetPivot()
    {
        //m_hingeJoint.connectedBody = GameManager.instance.GetPivotRB();
    }

    private void SetAxis()
    {
        //m_hingeJoint.axis = GameManager.instance.Get
    }
}
