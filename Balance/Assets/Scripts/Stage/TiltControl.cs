using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltControl : MonoBehaviour
{
    // 初期Y位置を保存
    //private float initialYPosition;

    // 座標の固定
    private Vector3 FixedPosition;

    // Y軸の回転の固定
    private float FixedYRotation = 0f;

    // AddForceで使用する力の大きさ
    private Vector3 m_forceDirection = new Vector3(0, 10, 0);
    private ForceMode m_forceMode = ForceMode.Force;

    private Rigidbody m_rb;

    // 傾きを水平に保とうとする力(復元力)の強さ
    [SerializeField] float Resilience = 10f;

    // X軸とZ軸の回転制限角度
    [SerializeField] float MinXRotation = -30f;
    [SerializeField] float MaxXRotation = 30f;
    [SerializeField] float MinZRotation = -30f;
    [SerializeField] float MaxZRotation = 30f;

    // (※デバッグ用) オブジェクトの動作を一時停止するためのフラグ
    private bool isPaused = false;
    private Vector3 savedPosition;
    private Quaternion savedRotation;

    // 外部からアクセス可能な力の最大値
    public Vector3 MaxForcePoint { get; private set; }

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        m_rb = GetComponent<Rigidbody>();

        // nullチェック
        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }

        // 初期Y位置を保存
        //initialYPosition = transform.position.y;

        // オブジェクトの初期位置を保存
        FixedPosition = transform.position;

    }

    void Update()
    {
        // オブジェクトの位置を固定
        transform.position = FixedPosition;

        // Y軸の位置を初期位置に固定
        /*Vector3 currentPosition = transform.position;
        currentPosition.y = initialYPosition; // Y軸を初期位置に設定
        transform.position = currentPosition;*/

        // (※デバッグ用) Oキーが押されたときに一時停止を実行
        if (Input.GetKeyDown(KeyCode.O))
        {
            TogglePause();
        }

        // (※デバッグ用) 一時停止中は位置と回転を保存した値に固定
        if (isPaused)
        {
            transform.position = savedPosition;
            transform.rotation = savedRotation;
            return;
        }

        // Update内で力を加える（ForceModeがForce以外の場合）
        if (m_forceMode != ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
            MaxForcePoint = m_forceDirection; // 最大の力がかかるポイントを記録
        }

        // 現在の回転を取得
        Quaternion currentRotation = transform.rotation;

        // オイラー角に変換
        Vector3 euler = currentRotation.eulerAngles;

        // Y軸の回転を固定
        euler.y = FixedYRotation;

        // X軸とZ軸の回転を制限
        euler.x = ClampAngle(euler.x, MinXRotation, MaxXRotation);
        euler.z = ClampAngle(euler.z, MinZRotation, MaxZRotation);

        // 回転を更新
        transform.rotation = Quaternion.Euler(euler);
    }

    void FixedUpdate()
    {
        // (※デバッグ用) 一時停止中は処理を中断
        if (isPaused)
        {
            return;
        }

        // ForceModeがForceの場合はFixedUpdate内で力を加える
        if (m_forceMode == ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
            MaxForcePoint = m_forceDirection; // 最大の力がかかるポイントを記録
        }

        // 傾きを水平に保つ処理
        Vector3 currentRotation = transform.localEulerAngles;

        // X軸の傾きを水平に戻すためのトルク
        if (currentRotation.x > 180) currentRotation.x -= 360;
        Vector3 xCorrectionTorque = Vector3.right * -currentRotation.x * Resilience;

        // Z軸の傾きを水平に戻すためのトルク
        if (currentRotation.z > 180) currentRotation.z -= 360;
        Vector3 zCorrectionTorque = Vector3.forward * -currentRotation.z * Resilience;

        // Rigidbodyにトルクを加える
        m_rb.AddTorque(xCorrectionTorque + zCorrectionTorque);
    }

    // 力の方向と大きさを設定するメソッド
    public void SetForceDirection(Vector3 direction)
    {
        m_forceDirection = direction;
    }

    // 力の方向と大きさを取得するメソッド
    public Vector3 GetForceDirection()
    {
        return m_forceDirection;
    }

    // ForceModeを設定するメソッド
    public void SetForceMode(ForceMode mode)
    {
        m_forceMode = mode;
    }

    // ForceModeを取得するメソッド
    public ForceMode GetForceMode()
    {
        return m_forceMode;
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

    // (※デバッグ用) オブジェクトの動作を一時停止または再開するメソッド
    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            savedPosition = transform.position;
            savedRotation = transform.rotation;

            Debug.Log("処理を一時停止");
        }
        if (!isPaused)
        {
            Debug.Log("処理を再開");
        }
    }

    // StageMovement.csが向こうの場合、TiltControl.csを有効化
    public void StateBalanceRelease()
    {
        // StageMovementコンポーネントを取得
        StageMovement stageMovement = GetComponent<StageMovement>();

        // StageMovementが無効化されている場合の処理
        if (stageMovement != null && !stageMovement.enabled)
        {
            // Freeze RotationのXとZを解除
            m_rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            m_rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;

            // TiltControl.csを有効にする
            this.enabled = true;

            // デバッグログを出力
            Debug.Log("StageMovementがオフのため、Freeze RotationのXとZをfalseにしてTiltControlをオンにしました。");
        }
    }
}
