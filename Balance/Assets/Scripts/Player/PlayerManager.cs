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
        DebugStateChange();
    }

    private RescueState m_beforeState;
    private void DebugStateChange()
    {
        if (currentState != m_beforeState)
        {
            Debug.Log("state change " + m_beforeState + "->" + currentState);
        }
        
        m_beforeState = currentState;
    }
}
