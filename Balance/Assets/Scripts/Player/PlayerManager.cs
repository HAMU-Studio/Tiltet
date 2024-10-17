using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 元はGamaManagerにstateを置いていたが、インスタンスが一つなのでエラーが多発したので移動。
/// ついでにアニメーション実装を楽にするためにPlayerControllerのフラグを減らしてこっちに構造体として移植したい。
/// HingeManagerの移植もありかも
/// </summary>

public enum RescueState
{
    //紐を無くすタイミングのためThrowingとFlyに分ける
    //飛ばす直前の位置に移動させるためMove追加
    None,
    Wait,
    Move,
    Fly,
    SuperLand
}

public class PlayerManager : MonoBehaviour
{
    private RescueState currentState;
    
    public RescueState State
    {
        set { currentState = value; }
        get { return currentState; }
    }

    private void Start()
    {
        m_beforeState = currentState;
    }

    private void FixedUpdate()
    {
        if (currentState != m_beforeState)
        {
            OnStateChange();
        }
    }

    private RescueState m_beforeState;
    private void OnStateChange()
    {
       // Debug.Log("state change " + m_beforeState + "->" + currentState);
        
        if (m_beforeState == RescueState.None && currentState == RescueState.Wait)
        {
            //落ちたら救出開始
            GameManager.instance.Rescue = true;
        }

        if (m_beforeState == RescueState.Fly || m_beforeState == RescueState.SuperLand)
        {
            //着地したら救出終了
            if (currentState == RescueState.None)
            {
                GameManager.instance.Rescue = false;
            }
        }

        if (m_beforeState == RescueState.Fly && currentState == RescueState.SuperLand)
        {
            if (gameObject.layer ==  LayerMask.NameToLayer("Fly"))
            {
                gameObject.layer = LayerMask.NameToLayer("Player");
                Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
            }
        }

        if (m_beforeState == RescueState.Move && currentState == RescueState.Fly)
        {
            //飛んだらレイヤー管理
            StartCoroutine("LayerManagement");
            //LayerManagement();
        }
        m_beforeState = currentState;
    }

    [Header("飛び始めてから当たり判定が元に戻るまでの時間")] 
    [SerializeField] private float waitTime = 2f;
    
    /// <summary>
    /// 救出アクション中進行不可能にならないようにLayerを変更して念のため貫通するように
    /// </summary>
    private IEnumerator LayerManagement()
    {
        //NameToLayerは名前から数値への変換。本来Layerは数字
        gameObject.layer = LayerMask.NameToLayer("Fly");
        Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
        
        yield return new WaitForSeconds(waitTime);
        
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log("Layer Change to " + LayerMask.LayerToName(gameObject.layer));
    }
}
