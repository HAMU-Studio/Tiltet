using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLine : MonoBehaviour
{
    private LineRenderer m_lineRenderer;
    private Transform m_startPoint;
    private Transform m_endPoint;
  
    private void Start()
    {
        m_startPoint = gameObject.transform;
        m_lineRenderer = this.gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if ( m_endPoint != null)
        {
            SetLinePos(m_startPoint.position, m_endPoint.position);
        }
    }

    private void SetLinePos(Vector3 startPos, Vector3 endPos)
    {
        var positions = new Vector3[] { startPos, endPos };
        m_lineRenderer?.SetPositions(positions);
    }
  
    //FallAreaで呼び出す。ロープの終点指定
    public void SetEndPoint(GameObject playerInstance)
    {
        m_endPoint = playerInstance.transform;
    }

    public void ResetRope()
    {
        SetLinePos ( Vector3.zero, Vector3.zero );
        m_endPoint = null;
    }
}
