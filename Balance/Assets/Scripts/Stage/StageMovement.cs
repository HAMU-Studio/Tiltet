using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    public enum MovePattern
    {
        None,
        Forward,
        Backward,
        Left,
        Right
    }

    //移動パターン別で詳細数値変更可能
    [SerializeField] private MovePattern currentPattern = MovePattern.None;

    // 移動速度
    [Header("< 方向別移動速度 >")]
    [SerializeField] private float forward = 3f; 　//前方
    [SerializeField] private float backward = -3f; //後方
    [SerializeField] private float left = -3f; 　　//左
    [SerializeField] private float right = 3f; 　　//右

    // 前後の傾き範囲
    [Header("< 前後移動の傾き感知範囲 >")]
    [Header("例）Rotation.Xが5度以上15度以内(初期値)の傾きなら前方移動")]
    [SerializeField] private float forwardTiltMinX = 5f;　　// ５度以上(初期値)で前方移動を有効化
    [SerializeField] private float forwardTiltMaxX = 15f;   // 15度以上(初期値)は前方移動の有効範囲外
    [SerializeField] private float backwardTiltMinX = -15f; // -15度以上(初期値)は後方移動の有効範囲外
    [SerializeField] private float backwardTiltMaxX = -5f;  // -５度以上(初期値)で後方移動を有効化

    // 左右の傾き範囲
    [Header("< 左右移動の傾き感知範囲 >")]
    [Header("例）Rotation.Zが-5度以上-15度以内(初期値)の傾きなら右移動")]
    [SerializeField] private float leftTiltMinZ = 5f;    // ５度以上(初期値)で左移動を有効化
    [SerializeField] private float leftTiltMaxZ = 15f;　 // 15度以上(初期値)は左移動の有効範囲外
    [SerializeField] private float rightTiltMinZ = -15f; // -15度以上(初期値)は右移動の有効範囲外
    [SerializeField] private float rightTiltMaxZ = -5f;  // -５度以上(初期値)で右移動を有効化

    // 傾き範囲の振れ幅
    // 全ての移動パターンは-7.5～7.5度以内(初期値)が有効範囲    
    [Header("< 傾き感知範囲振れ幅 >")]
    [Header("例）Rotation.Xが5度以上(初期値)かつ、Rotation.Zが-7.5～7.5度以内(初期値)ならば前方の移動が有効化")]
    [SerializeField] private float tiltMinZ = -7.5f;
    [SerializeField] private float tiltMaxZ = 7.5f;

    private Rigidbody rb;
    private TiltControl tiltControl;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tiltControl = GetComponent<TiltControl>();

        if (rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }

        if (tiltControl == null)
        {
            Debug.LogError("TiltControlが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }
    }

    void Update()
    {
        UpdateMovePattern();
        ApplyMovement();
    }

    // 傾きに応じて移動パターンを更新
    private void UpdateMovePattern()
    {
        float tiltX = tiltControl.CurrentTiltX;
        float tiltZ = tiltControl.CurrentTiltZ;

        if (tiltX >= forwardTiltMinX && tiltX <= forwardTiltMaxX && tiltZ >= tiltMinZ && tiltZ <= tiltMaxZ)
        {
            currentPattern = MovePattern.Forward;
        }
        else if (tiltX >= backwardTiltMinX && tiltX <= backwardTiltMaxX && tiltZ >= tiltMinZ && tiltZ <= tiltMaxZ)
        {
            currentPattern = MovePattern.Backward;
        }
        else if (tiltX >= -7.5f && tiltX <= 7.5f && tiltZ >= leftTiltMinZ && tiltZ <= leftTiltMaxZ)
        {
            currentPattern = MovePattern.Left;
        }
        else if (tiltX >= -7.5f && tiltX <= 7.5f && tiltZ >= rightTiltMinZ && tiltZ <= rightTiltMaxZ)
        {
            currentPattern = MovePattern.Right;
        }
        else
        {
            currentPattern = MovePattern.None;
        }
    }

    // 現在の移動パターンに基づいてオブジェクトを動かす
    private void ApplyMovement()
    {
        switch (currentPattern)
        {
            case MovePattern.Forward:
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forward);
                break;
            case MovePattern.Backward:
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, backward);
                break;
            case MovePattern.Left:
                rb.velocity = new Vector3(left, rb.velocity.y, rb.velocity.z);
                break;
            case MovePattern.Right:
                rb.velocity = new Vector3(right, rb.velocity.y, rb.velocity.z);
                break;
            case MovePattern.None:
                rb.velocity = Vector3.zero; // 全ての軸の速度をゼロにして静止
                break;
        }
    }
}