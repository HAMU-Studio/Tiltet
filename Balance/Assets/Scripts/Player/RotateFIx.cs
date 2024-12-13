using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFIx : MonoBehaviour
{
    //プレイヤーをステージの子にすると傾きに応じてぐにゃるから無理やり直す, 今後要修正

    private Vector3 rot;

    void Update()
    {
        rot = transform.localEulerAngles;
    
        transform.localRotation =  Quaternion.Euler(0.0f, rot.y, 0.0f);
    }
}
