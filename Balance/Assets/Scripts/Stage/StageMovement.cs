using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    // 力の大きさを調整するための倍率
    [SerializeField] private float forceMultiplier = 1.0f;

    // TiltControlのインスタンスを保持
    private TiltControl tiltControl;

    // Rigidbodyコンポーネント
    private Rigidbody rb;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // TiltControlコンポーネントを取得
        tiltControl = GetComponent<TiltControl>();

        // nullチェック
        if (tiltControl == null)
        {
            Debug.LogError("TiltControlコンポーネントが見つかりません。");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbodyコンポーネントが見つかりません。");
        }
    }

    void FixedUpdate()
    {
        // 傾き方向を取得
        Vector3 tiltDirection = tiltControl.TiltDirection;

        // Y軸の位置を維持するため、Y軸の力をゼロにする
        tiltDirection.y = 0;

        // ワールド座標系で傾きの方向に基づいて力を加える
        Vector3 worldTiltDirection = transform.TransformDirection(tiltDirection);

        // 力を加える
        rb.AddForce(worldTiltDirection * forceMultiplier, ForceMode.Force);

        // Y軸の位置を固定
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0f; // 初期Y座標を固定値に設定 (必要に応じて調整可能)
        transform.position = currentPosition;
    }
}
