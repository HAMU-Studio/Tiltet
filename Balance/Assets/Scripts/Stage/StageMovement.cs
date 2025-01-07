using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    private Rigidbody m_rb;
    private TiltControl m_tiltControl;

    // 状態の列挙型
    public enum State
    {
        Moving,
        Stop,
        Neutral // Neutral状態を追加
    }

    private State state = State.Moving; // 初期状態をMovingに設定

    // デバック用の制御フラグ
    private bool debugStop = false;
    private bool debugNeutral = false;

    // 外部から制御可能なフラグ
    private bool isStopActive = false;
    private bool isNeutralActive = false;  

    public bool IsStopActive
    {
        get { return isStopActive; }
        set
        {
            isStopActive = value;
            UpdateState(); // フラグ変更時に状態を更新
        }
    }

    public bool IsNeutralActive
    {
        get { return isNeutralActive; }
        set
        {
            isNeutralActive = value;
            UpdateState(); // フラグ変更時に状態を更新
        }
    }

    // 移動量を管理するための変数
    private Vector3 m_beforePos;  // 前フレームの位置
    private Vector3 m_currentPos; // 現在の位置
    private Vector3 m_movementAmount; // 移動量

    // 移動量を外部から取得するためのプロパティ
    public Vector3 MovementAmount
    {
        get { return m_movementAmount; }
    }

    // オブジェクトの移動速度のスケール
    [SerializeField] private float movementScale = 1f;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
            return;
        }

        // TiltControlコンポーネントを取得
        m_tiltControl = FindObjectOfType<TiltControl>();
        if (m_tiltControl == null)
        {
            Debug.LogError("TiltControlが見つかりません。ステージのオブジェクトに正しくアタッチされているか確認してください。");
        }

        m_currentPos = transform.position;
        m_beforePos = transform.position;
    }

    private void Update()
    {
        // デバック用
        // UキーでStop状態をトグル
        if (Input.GetKeyDown(KeyCode.M))
        {
            debugStop = !debugStop;     // Stop状態のトグル
            debugNeutral = false;            // Neutral状態は解除
            UpdateState();                        // 状態を更新
            Debug.Log($"Stop状態が{(debugStop ? "有効" : "無効")}になりました。");
        }

        // デバック用
        // NキーでNeutral状態をトグル
        if (Input.GetKeyDown(KeyCode.N))
        {
            debugNeutral = !debugNeutral; // Neutral状態のトグル
            debugStop = false;                 // Stop状態は解除
            UpdateState();                          // 状態を更新
            Debug.Log($"Neutral状態が{(debugNeutral ? "有効" : "無効")}になりました。");
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Moving:
                ApplyMovement();
                break;

            case State.Stop:
                StopMovement();
                break;

            case State.Neutral:
                // Neutral状態では何もしない（外部スクリプトの力を優先）
                break;
        }
        CalculateMovementAmount();
    }

    // 状態を更新するメソッド
    private void UpdateState()
    {
        if (isStopActive || debugStop)
        {
            EnableStop();
        }
        else if (isNeutralActive || debugNeutral)
        {
            EnableNeutral();
        }
        else
        {
            EnableMoving();
        }
    }

    // Moving状態を有効にする処理
    private void EnableMoving()
    {
        if (state == State.Moving) return; // すでにMovingなら何もしない
        state = State.Moving;
        Debug.Log("State changed to Moving");
    }

    // Stop状態を有効にする処理
    private void EnableStop()
    {
        if (state == State.Stop) return; // すでにStopなら何もしない
        state = State.Stop;
        Debug.Log("State changed to Stop");
    }

    // Neutral状態を有効にする処理
    private void EnableNeutral()
    {
        if (state == State.Neutral) return; // すでにNeutralなら何もしない
        state = State.Neutral;
        m_rb.velocity = Vector3.zero;
        Debug.Log("State changed to Neutral");
    }

    // 移動を適用する処理
    private void ApplyMovement()
    {
        // TiltControlからRotationXとRotationZを取得
        float rotationX = m_tiltControl.RotationX;
        float rotationZ = m_tiltControl.RotationZ;

        // RotationXをvelocity.zに、RotationZをvelocity.xに適用
        Vector3 velocity = m_rb.velocity;
        velocity.z = rotationX * movementScale; // X軸の回転をZ方向の速度に適用
        velocity.x = -rotationZ * movementScale; // Z軸の回転をX方向の速度に適用
        m_rb.velocity = velocity;
    }

    // 移動を停止させる処理
    private void StopMovement()
    {
        m_rb.velocity = Vector3.zero; // 速度をゼロにする
    }

    // 移動量を計算するメソッド
    private void CalculateMovementAmount()
    {
        m_currentPos = transform.position;
        m_movementAmount = m_currentPos - m_beforePos;
        m_beforePos = m_currentPos;
    }
}
