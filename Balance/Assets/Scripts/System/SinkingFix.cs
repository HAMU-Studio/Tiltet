using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SinkingFix : MonoBehaviour
{
    private CapsuleCollider m_cc;
    private Rigidbody m_rb;
    // Start is called before the first frame update
    void Start()
    {
        isSinking = false;
        m_cc = this.GetComponent<CapsuleCollider>();
        m_rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSinking();
    }

    private bool isSinking = false;
    private Collider[] m_overLapColliders;
    [SerializeField] private float divisor = default!; 
    
    private void CheckSinking()
    {
        if (isSinking)
        {
            //修正中は物理演算停止
          //  TempStopPhysics();
        }
         
        
        // point0: カプセルの一方の端点の位置
        // point1: カプセルのもう一方の端点の位置
        // radius: カプセルの半径
        
        //コライダーを通常より小さく指定すればがっつりめり込んだ時のみ修正できるかも？
         m_overLapColliders = Physics.OverlapCapsule(
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y - m_cc.height / 2) + m_cc.radius,
                -m_cc.center.z),
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y + m_cc.height / 2) - m_cc.radius,
                -m_cc.center.z),
            m_cc.radius,
            LayerMask.GetMask("Floor") 
        );
         //LayerMaskは判定を取得するオブジェクトの選択に使う
         //床とのめり込みさえ修正できればいいからenemyとplayerの指定いらないかも

        if (0 < m_overLapColliders.Length)
        {
            foreach (Collider c in m_overLapColliders)
            {
                Debug.Log(c.gameObject);
            }

            isSinking = true;
            TempStopPhysics();
            FixSinking();
        }
        else
        {
            isSinking = false;
        }
    }
 
    private void TempStopPhysics()
    {
        m_rb.velocity = Vector3.zero;
        m_rb.angularVelocity = Vector3.zero;
    }

    private Vector3 direction;
    private float distance;
    private void FixSinking()
    {
        if (isSinking)
        {
            foreach (Collider c in m_overLapColliders)
            {
                bool penetrat = Physics.ComputePenetration(
                    m_cc, transform.position, transform.rotation,
                    c.GetComponent<Collider>(), c.transform.position, c.transform.rotation,
                        out direction, out distance);

                if (penetrat)
                {
                  //  direction = Vector3.Scale(direction, new Vector3(1, 2f, 1));
                    transform.position = transform.position + (direction * distance);
                    TempStopPhysics();  //移動後の動き抑制
                }
            }
        }
      
    }
}
