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
        Stop
    }

    private State state = State.Moving; // 初期状態をMovingに設定

    // オブジェクトの移動速度のスケール
    [SerializeField] private float movementScale = 1f;

    // 移動量を管理するための変数
    private Vector3 m_beforePos;  // 前フレームの位置
    private Vector3 m_currentPos; // 現在の位置
    private Vector3 m_movementAmount; // 移動量

    // 移動量を外部から取得するためのプロパティ
    public Vector3 MovementAmount
    {
        get { return m_movementAmount; }
    }

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
        }

        CalculateMovementAmount();
    }

    // 状態を設定するメソッド
    public void SetState(State newState)
    {
        if (state == newState) return; // 同じ状態に変更しない

        state = newState; // 状態を更新

        if (state == State.Moving)
        {
            Debug.Log("State changed to Moving");
        }
        else if (state == State.Stop)
        {
            Debug.Log("State changed to Stop");
        }
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
        m_rb.velocity = Vector3.zero;
    }

    // 移動量を計算するメソッド
    private void CalculateMovementAmount()
    {
        m_currentPos = transform.position;
        m_movementAmount = m_currentPos - m_beforePos;
        m_beforePos = m_currentPos;
    }
}
