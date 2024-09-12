using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLine : MonoBehaviour
{
    private LineRenderer m_lineRenderer;
    private Transform m_startPoint;
    private Transform m_endPoint;
    private RopeLine  m_ropeLine;
    private void Start()
    {
        m_ropeLine = gameObject.GetComponent<RopeLine>();
        m_startPoint = gameObject.transform;
        m_ropeLine.enabled = false;
        m_lineRenderer = this.gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (m_ropeLine.enabled == true)
        {
            SetLinePos();
        }
    }

    private void SetLinePos()
    {
        if (m_lineRenderer.enabled == false)
            m_lineRenderer.enabled = true;
        
        var positions = new Vector3[] { m_startPoint.position, m_endPoint.position };
        m_lineRenderer?.SetPositions(positions);
    }
  
    //FallAreaで呼び出す。ロープの終点指定
    public void SetEndPoint(GameObject playerInstance)
    {
        m_endPoint = playerInstance.transform;
        m_ropeLine.enabled = true;
    }

    public void ResetRope()
    {
        m_lineRenderer.enabled = false;
        m_ropeLine.enabled = false;
        m_endPoint = null;
    }
}
