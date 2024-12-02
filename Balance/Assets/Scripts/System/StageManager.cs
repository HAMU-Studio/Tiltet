using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StageManager : MonoBehaviour
{
    // 初期のY座標を保持する変数
    private float m_initialY;

    private void Start()
    {
        GameManager.instance.SaveAircraftInstance(gameObject);
    }

    void Update()
    {
        // オブジェクトのY座標を固定する処理を呼び出す
        //LockPositionY();
    }

    // Y座標を初期位置から下がらないように固定する処理
    private void LockPositionY()
    {
        Vector3 position = transform.position;

        // Y座標が初期値より小さい場合、初期値に戻す
        if (position.y < m_initialY)
        {
            position.y = m_initialY;
            transform.position = position;
        }
    }

    // 衝突時の処理を追加
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突相手のタグが "StageObject" の場合
        if (collision.gameObject.CompareTag("StageObject"))
        {
            Debug.Log("衝突");
        }
    }
}
