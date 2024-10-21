using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    public float forceMultiplier = 2.0f; // 力の倍率を設定する変数
    private Vector3 movementForce = Vector3.zero; // 現在の移動力を保持する変数

    void FixedUpdate()
    {
        // 現在の移動力に基づいて床を移動させる
        if (movementForce != Vector3.zero)
        {
            transform.Translate(movementForce * Time.fixedDeltaTime, Space.World);
        }
    }

    public void AddForce(Vector3 force)
    {
        // 指定された方向に力を加える（倍率を適用）
        movementForce += force * forceMultiplier;
    }

    public void RemoveForce(Vector3 force)
    {
        // 指定された方向の力を削除（倍率を適用）
        movementForce -= force * forceMultiplier;
    }

    public void StopMoving()
    {
        // 力をゼロにして停止する
        movementForce = Vector3.zero;
    }

    public void StopMovement()
    {
        // StageMovement と GravitySensor の機能を停止する処理
        StopAllCoroutines();
        enabled = false;

        // GravitySensor コンポーネントを取得して停止する
        GravitySensor gravitySensor = GetComponent<GravitySensor>();
        if (gravitySensor != null)
        {
            gravitySensor.enabled = false;
        }

        // RigidbodyのFreeze Positionを全てtrueにする
        // RigidbodyのFreeze Rotationを全てtrueにする
        /*Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                              RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }*/
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EncountArea"))
        {
            StopMovement();
        }
    }*/
}
