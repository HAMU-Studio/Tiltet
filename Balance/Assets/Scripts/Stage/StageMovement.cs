using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    private Rigidbody m_rb; // Rigidbodyコンポーネントを格納
    private TiltControl m_tiltControl; // TiltControlコンポーネントを格納

    public enum State // ステージの動作状態を定義
    {
        Moving, // 動いている状態
        Stop,   // 停止状態
        Neutral // 中立状態（動作なし）
    }

    private State state = State.Moving; // 初期状態をMovingに設定

    private bool debugStop = false; // デバッグ用のStop状態フラグ
    private bool debugNeutral = false; // デバッグ用のNeutral状態フラグ

    private bool isStopActive = false; // Stop状態が有効かどうか
    private bool isNeutralActive = false; // Neutral状態が有効かどうか

    // Stop状態のゲッターとセッター
    public bool IsStopActive 
    {
        get { return isStopActive; }
        set
        {
            isStopActive = value;
            UpdateState(); // 状態更新
        }
    }

    // Neutral状態のゲッターとセッター
    public bool IsNeutralActive 
    {
        get { return isNeutralActive; }
        set
        {
            isNeutralActive = value;
            UpdateState(); // 状態更新
        }
    }

    private Vector3 m_beforePos; // 前回の位置を格納
    private Vector3 m_currentPos; // 現在の位置を格納
    private Vector3 m_movementAmount; // 移動量を格納

    // 移動量を取得するプロパティ
    public Vector3 MovementAmount 
    {
        get { return m_movementAmount; }
    }
    
    [Header("傾きが基準以下の時の加速度")]
    [SerializeField] private float movementScaleLow = 0.5f; // 低いスケール（速度調整）
    [Header("傾きが基準以上の時の加速度")]
    [SerializeField] private float movementScaleHigh = 1.5f; // 高いスケール（速度調整）
    [Header("高速と低速を分ける基準値(自機の傾きが最大15)")]
    [SerializeField] private float rotationThreshold = 10f; // スケールを分ける基準となる回転角度

    void Start()
    {
        m_rb = GetComponent<Rigidbody>(); // Rigidbodyコンポーネントを取得
        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
            return; // Rigidbodyが無い場合、処理を中止
        }

        m_tiltControl = FindObjectOfType<TiltControl>(); // TiltControlを探して取得
        if (m_tiltControl == null)
        {
            Debug.LogError("TiltControlが見つかりません。ステージのオブジェクトに正しくアタッチされているか確認してください。");
        }

        m_currentPos = transform.position; // 現在位置を初期化
        m_beforePos = transform.position; // 前回の位置を初期化
    }

    private void Update()
    {
        // デバッグ用：MキーでStop状態を切り替え
        if (Input.GetKeyDown(KeyCode.M))
        {
            debugStop = !debugStop;
            debugNeutral = false; // Neutral状態を無効化
            UpdateState(); // 状態更新
            Debug.Log($"Stop状態が{(debugStop ? "有効" : "無効")}になりました。");
        }

        // デバッグ用：NキーでNeutral状態を切り替え
        if (Input.GetKeyDown(KeyCode.N))
        {
            debugNeutral = !debugNeutral;
            debugStop = false; // Stop状態を無効化
            UpdateState(); // 状態更新
            Debug.Log($"Neutral状態が{(debugNeutral ? "有効" : "無効")}になりました。");
        }
    }

    void FixedUpdate()
    {
        // 状態に応じて処理を分岐
        switch (state)
        {
            case State.Moving:
                ApplyMovement(); // 動いている状態の処理
                break;

            case State.Stop:
                StopMovement(); // 停止状態の処理
                break;

            case State.Neutral:
                // 中立状態は特に処理しない
                break;
        }
        CalculateMovementAmount(); // 移動量の計算
    }

    // 状態を更新するメソッド
    private void UpdateState()
    {
        // Stop状態が有効ならStop状態に変更
        if (isStopActive || debugStop)
        {
            EnableStop();
        }
        // Neutral状態が有効ならNeutral状態に変更
        else if (isNeutralActive || debugNeutral)
        {
            EnableNeutral();
        }
        // それ以外はMoving状態に変更
        else
        {
            EnableMoving();
        }
    }

    // Moving状態
    private void EnableMoving()
    {
        // すでにMoving状態なら変更しない
        if (state == State.Moving) return;
        state = State.Moving; // Moving状態に変更
        Debug.Log("State changed to Moving");
    }

    // Stop状態
    private void EnableStop()
    {
        // すでにStop状態なら変更しない
        if (state == State.Stop) return;
        state = State.Stop; // Stop状態に変更
        Debug.Log("State changed to Stop");
    }

    // Neutral状態
    private void EnableNeutral()
    {
        // すでにNeutral状態なら変更しない
        if (state == State.Neutral) return;
        state = State.Neutral; // Neutral状態に変更
        m_rb.velocity = Vector3.zero; // 速度をゼロにして停止
        Debug.Log("State changed to Neutral");
    }

    // 自機に移動力を与えるメソッド
    private void ApplyMovement()
    {
        // 傾き情報を取得
        float rotationX = m_tiltControl.RotationX;
        float rotationZ = m_tiltControl.RotationZ;

        // 傾きが閾値を超えている場合、高い移動スケールを適用
        float currentMovementScale = (Mathf.Abs(rotationX) > rotationThreshold || Mathf.Abs(rotationZ) > rotationThreshold) 
            ? movementScaleHigh 
            : movementScaleLow;

        // 速度ベクトルを設定
        Vector3 velocity = m_rb.velocity;
        velocity.z = rotationX * currentMovementScale;
        velocity.x = -rotationZ * currentMovementScale;
        m_rb.velocity = velocity; // Rigidbodyに速度を適用
    }

    // 自機を停止させるメソッド
    private void StopMovement()
    {
        // 停止状態では速度をゼロに設定
        m_rb.velocity = Vector3.zero;
    }

    // 自機の移動量を計算メソッド
    private void CalculateMovementAmount()
    {
        // 現在位置と前回の位置の差分を計算
        m_currentPos = transform.position;
        m_movementAmount = m_currentPos - m_beforePos;
        m_beforePos = m_currentPos; // 前回位置を更新
    }
}
