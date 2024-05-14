using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // 固定したいY軸の回転角度
    public float fixedYRotation = 0f;

    void Update()
    {
        // 現在の回転を取得
        Quaternion currentRotation = transform.rotation;

        // オイラー角に変換
        Vector3 euler = currentRotation.eulerAngles;

        // Y軸の回転を固定
        euler.y = fixedYRotation;

        // X軸とZ軸の回転を制限
        euler.x = ClampAngle(euler.x, -30f, 30f);
        euler.z = ClampAngle(euler.z, -30f, 30f);

        // 回転を更新
        transform.rotation = Quaternion.Euler(euler);
    }

    // 角度をクランプするヘルパーメソッド
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 180f)
        {
            angle = Mathf.Clamp(angle, min, max);
        }
        else
        {
            angle = Mathf.Clamp(angle, 360f + min, 360f + max);
        }
        return angle;
    }
}
