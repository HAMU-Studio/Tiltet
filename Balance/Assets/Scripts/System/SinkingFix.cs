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
        CheckSinkingLoosely();
    }

    private bool isSinking = false;
    private Collider[] m_overLapColliders;
    [SerializeField] private float divisor = default!;
    [SerializeField] private float stopFixHeight = default!;
    
    private void CheckSinking()
    {
        if (isSinkingLoosely == false)
        {
            return;
        }
        // point0: カプセルの一方の端点の位置
        // point1: カプセルのもう一方の端点の位置
        // radius: カプセルの半径
        
         m_overLapColliders = Physics.OverlapCapsule(
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y - m_cc.height / 2) + m_cc.radius,
                -m_cc.center.z),
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y + m_cc.height / 2) - m_cc.radius,
                -m_cc.center.z),
            m_cc.radius,
            LayerMask.GetMask("Floor") 
        );
         //LayerMaskは判定を取得するオブジェクトの選択に使う

        if (0 < m_overLapColliders.Length)
        {
            isSinking = true;
           // TempStopPhysics();
            FixSinking();
        }
        else if (isSinkingLoosely && transform.position.y >= stopFixHeight)  //ある程度の高さまで上らないとめり込み続けちゃう->一定の高さ超えたら修正ストップ
        {
            isSinking = false;
            isSinkingLoosely = false;
            Debug.Log("StopFix");
        }
    }
    
    private Collider[] m_overLapColliders_L;
    private bool isSinkingLoosely = false;
    private void CheckSinkingLoosely()
    {
        if (isSinkingLoosely)
        {
            //修正中は物理演算停止
            //  TempStopPhysics();
        }
        //コライダーを通常より小さく指定して、ゆるめのめりこみ検知
        
        m_overLapColliders_L = Physics.OverlapCapsule(
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y - m_cc.height / divisor) + m_cc.radius,
                -m_cc.center.z),
            transform.position + new Vector3(-m_cc.center.x, (m_cc.center.y + m_cc.height / divisor) - m_cc.radius,
                -m_cc.center.z),
            m_cc.radius,
            LayerMask.GetMask("Floor") 
        );
        //LayerMaskは判定を取得するオブジェクトの選択に使う
        //床とのめり込みさえ修正できればいいからenemyとplayerの指定いらないかも

        if (0 < m_overLapColliders_L.Length)
        {
            Debug.Log("Sinking!");
            isSinkingLoosely = true;
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
                    //direction = Vector3.Scale(direction, new Vector3(1, 7f, 1));
                    transform.position += direction * distance;
                    TempStopPhysics();  //移動後の動き抑制
                }
            }
        }
      
    }
}
