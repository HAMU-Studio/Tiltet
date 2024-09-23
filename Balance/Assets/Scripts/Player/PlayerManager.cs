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
    Throwing,
    Fly
}

public class PlayerManager : MonoBehaviour
{
    private RescueState currentRescueState;
    
    public RescueState RescueState
    {
        set { currentRescueState = value; }
        get { return currentRescueState; }
    }
 
}
