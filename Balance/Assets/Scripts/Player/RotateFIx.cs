using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFIx : MonoBehaviour
{
    //プレイヤーをステージの子にすると傾きに応じてぐにゃるから無理やり直す, 今後要修正

    private Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
         rot = default;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.rotation != rot)
        {
            transform.rotation = rot;
        }
    }
}
