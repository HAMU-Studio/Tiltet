using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLine : MonoBehaviour
{
    [SerializeField] private LineRenderer m_lineRenderer;
    private Transform m_startPoint;
    private Transform m_endPoint;
    private RopeLine  m_ropeLine;
    private void Start()
    {
        m_ropeLine = this.gameObject.GetComponent<RopeLine>();
        m_startPoint = this.gameObject.transform;
        m_ropeLine.enabled = false;
      
    }

    void Update()
    {
        if (m_ropeLine.enabled == true)
        {
            var positions = new Vector3[] { m_startPoint.position, m_endPoint.position, };
            m_lineRenderer?.SetPositions(positions);
        }
    }

    public void SetEndPoint(Transform playerInstance)
    {
        m_ropeLine.enabled = true;
        m_endPoint = playerInstance;
       
    }

    public void ResetEndPoint()
    {
        m_ropeLine.enabled = false;
        m_endPoint = null;
    }
}
