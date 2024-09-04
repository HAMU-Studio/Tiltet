using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLine : MonoBehaviour
{
    [SerializeField] private LineRenderer m_lineRenderer;
    [SerializeField] Transform m_startPoint;
    [SerializeField] Transform m_endPoint;
    
    // Update is called once per frame
    void Update()
    {
        var positions = new Vector3[] { m_startPoint.position, m_endPoint.position, };
        m_lineRenderer?.SetPositions(positions);
    }
}
