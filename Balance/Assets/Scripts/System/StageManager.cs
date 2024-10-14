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
  
    /*private void OnCollisionEnter(Collision other)
    {
      
        
        if (other.gameObject.CompareTag("Player"))
        {
            SetToStageChild(other.gameObject);
        }
    }*/

    [SerializeField] private float scaleSize;
 //   [SerializeField] private GameObject SetPos;
    public void SetToStageChild(GameObject obj)
    {
       
        //親子付けしてもステージの移動に置いて行かれるからサイズとスピード変更
        // Vector3.Scale(obj.transform.localScale, new Vector3(scaleSize, scaleSize, scaleSize));
        Debug.Log("callChange");
        obj.transform.localScale=  new Vector3(scaleSize, scaleSize, scaleSize);
       // obj.GetComponent<PlayerController>().walkSpeed *= 2f;
        //obj.GetComponent<PlayerController>().dashSpeed *= 2f;
        
       // obj.transform.position = SetPos.transform.position;
        //return;
        
        //子オブジェクトのtransform.parentに親にしたいオブジェクトのtransformを代入
        obj.transform.parent = this.transform;
        //new Vector3(0.07f, 4.23f, 0.07f);
       
    }
}
