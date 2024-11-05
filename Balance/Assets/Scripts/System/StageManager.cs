using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StageManager : MonoBehaviour
{
   // [SerializeField] private GameObject[] walls;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    private void OnCollisionEnter(Collision other)
    {
      
        
        /*if (other.gameObject.CompareTag("Player"))
        {
            SetToStageChild(other.gameObject);
        }*/
    }

    [SerializeField] private float scaleSize;
 //   [SerializeField] private GameObject SetPos;
    public void SetToStageChild(GameObject obj)
    {
        //親子付けしてもステージの移動に置いて行かれるからサイズとスピード変更
        //子オブジェクトのtransform.parentに親にしたいオブジェクトのtransformを代入
        obj.transform.parent = this.transform;
    }

    /// <summary>
    /// 親のサイズによる影響を打ち消すスケール算出 -> なぜか必要なくなった
    /// </summary>
    /// <param name="child">自機の子にあたるPlayer</param>
    /// <returns>playerのlocalScaleに入れる</returns>
    public void CounterScaleCalc(GameObject child)
    {
        //lossyScaleはワールド空間上の親子関係による影響を加味した最終的なスケール(実際の見た目のサイズ)を取得できる
        Vector3 localScale = child.transform.localScale;
        Vector3 parentLossyScale = this.transform.lossyScale;

        child.transform.localScale 
            = new Vector3(
                localScale.x / parentLossyScale.x,
                localScale.y / parentLossyScale.y,
                localScale.z / parentLossyScale.z);
    }
}
