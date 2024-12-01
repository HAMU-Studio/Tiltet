using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsArrival : MonoBehaviour
{
    [SerializeField] private GameObject part; // サブパーツ
    
    [SerializeField] private float needStayTime = 5.0f; // 滞在時間
    private float m_stayTime; // 現在の滞在時間を保持
    private bool isStay;

    private void Start()
    {
        part.SetActive(false); // サブパーツを見えないようにする
        m_stayTime = 0f; // 現在の滞在時間を初期化
        isStay = false;
    }

    // コライダー内に3秒間滞在できればサブパーツが出現する
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isStay = true;
        }
    }
    // エリアから出たら滞在時間をリセット
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isStay = false;
            m_stayTime = 0f; // 滞在時間をリセット
        }
    }

    private void Update()
    {
        if (isStay)
        {
            m_stayTime += Time.deltaTime; // 滞在時間を減少させる
            if (m_stayTime >= needStayTime)
            {
                part.SetActive(true); // サブパーツを表示
                Destroy(gameObject); // エリアを削除
            }
        }
    }
}