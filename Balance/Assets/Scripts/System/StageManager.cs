using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StageManager : MonoBehaviour
{
   private Vector3 m_beforePos;
   private Vector3 m_currentPos;

   private Vector3 m_movementAmount;

   private Rigidbody m_rb;

   private void Start()
   {
       m_currentPos = transform.position;
       m_beforePos = transform.position;
       m_rb = GetComponent<Rigidbody>();
   }

   
   public Vector3 MovementAmount
   {
       get { return m_movementAmount; }
   }

   private void FixedUpdate()
   {
       SetAircraftMovementAmount();
   }

   /// <summary>
   /// 自機の移動量計算
   /// </summary>
   private void SetAircraftMovementAmount()
   {
       m_currentPos = m_rb.position;

       m_movementAmount = (m_currentPos - m_beforePos);

       m_beforePos = m_rb.position;
   }

    public void SetToStageChild(GameObject obj)
    {
        obj.transform.parent = this.transform;
    }
}
