using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SimpleRotate : MonoBehaviour
{
    public Transform targetObject; // 回転させたいオブジェクト

    void Update()
    {
        if (targetObject != null)
        {
            targetObject.Rotate(Vector3.back, 10f * Time.deltaTime); // Y軸周りに回転
        }
    }
}
