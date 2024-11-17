using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StageManager : MonoBehaviour
{
   // [SerializeField] private GameObject[] walls;

   private Vector3 m_beforePos;
   private Vector3 m_currentPos;

   private Vector3 m_movementAmount;

   private void Start()
   {
       m_currentPos = transform.position;
       m_beforePos = transform.position;
   }

   public Vector3 MovementAmount
   {
       get { return m_movementAmount; }
   }

   private void FixedUpdate()
   {
       m_currentPos = transform.position;

       m_movementAmount = (m_currentPos - m_beforePos);

       m_beforePos = transform.position;
   }


   [SerializeField] private float scaleSize;
 //   [SerializeField] private GameObject SetPos;
    public void SetToStageChild(GameObject obj)
    {
       
     
        // Vector3.Scale(obj.transform.localScale, new Vector3(scaleSize, scaleSize, scaleSize));
        Debug.Log("callChange");

       
        //子オブジェクトのtransform.parentに親にしたいオブジェクトのtransformを代入
        obj.transform.parent = this.transform;
       
    }
}
