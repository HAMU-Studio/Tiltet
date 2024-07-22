using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    public float forceMultiplier = 2.0f; // 力の倍率を設定する変数
    public float rotationAngle = 10.0f;  // 傾ける角度

    private Vector3 movementForce = Vector3.zero; // 現在の移動力を保持する変数
    private Vector3 targetRotation = Vector3.zero; // 目標回転角度

    void FixedUpdate()
    {
        // 現在の移動力に基づいて床を移動させる
        if (movementForce != Vector3.zero)
        {
            transform.Translate(movementForce * Time.fixedDeltaTime, Space.World);
            // 目標回転角度に向けて回転する
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetRotation), Time.fixedDeltaTime * 2.0f);
        }
        else
        {
            // 移動力がゼロの場合、回転をリセットする
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * 2.0f);
        }
    }

    public void AddForce(Vector3 force)
    {
        // 指定された方向に力を加える（倍率を適用）
        movementForce += force * forceMultiplier;

        // 移動方向に応じて回転角度を設定
        if (force == Vector3.forward)
        {
            targetRotation = new Vector3(rotationAngle, 0, 0);
        }
        else if (force == Vector3.back)
        {
            targetRotation = new Vector3(-rotationAngle, 0, 0);
        }
        else if (force == Vector3.left)
        {
            targetRotation = new Vector3(0, 0, rotationAngle);
        }
        else if (force == Vector3.right)
        {
            targetRotation = new Vector3(0, 0, -rotationAngle);
        }
        // 他の方向の場合は、適宜調整する
    }

    public void RemoveForce(Vector3 force)
    {
        // 指定された方向の力を削除（倍率を適用）
        movementForce -= force * forceMultiplier;

        // 力がゼロになる場合、回転角度をリセット
        if (movementForce == Vector3.zero)
        {
            targetRotation = Vector3.zero;
        }
    }

    public void StopMoving()
    {
        // 力をゼロにして停止する
        movementForce = Vector3.zero;
        targetRotation = Vector3.zero;
    }
}
