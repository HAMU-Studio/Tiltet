using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltControl : MonoBehaviour
{
    private Rigidbody m_rb;

    // 接触中のオブジェクトの質量を記録する変数
    private float contactMass = 0f;

    // オブジェクトが接触しているかどうかのフラグ
    private bool isContacting = false;

    // 初期Y位置とY軸の回転角度を記録する変数
    private float initialYPosition;
    private float initialYRotation;

    // 傾きの最大角度
    [SerializeField] private float maxTiltAngleX = 30f;
    [SerializeField] private float maxTiltAngleZ = 30f;

    // 復元力の大きさ
    [SerializeField] private float restoringForce = 10f;

    // RotationのXとZの値を公開
    public float RotationX { get; private set; }
    public float RotationZ { get; private set; }

    // 列挙型で状態を定義
    private enum TiltState
    {
        TiltAdapting, // 傾きを適用する状態
        TiltReset     // 傾きをリセットする状態
    }

    // 現在の状態を保持
    private TiltState currentState = TiltState.TiltAdapting;

    // TiltResetの有効性を管理
    private bool isTiltResetActive = false;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        m_rb = GetComponent<Rigidbody>();

        // nullチェック
        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }

        // 初期Y位置とY軸の回転角度を保存
        initialYPosition = transform.position.y;
        initialYRotation = transform.rotation.eulerAngles.y;
    }

    void FixedUpdate()
    {
        // 状態による処理の分岐
        switch (currentState)
        {
            case TiltState.TiltAdapting:
                ApplyTiltAdapting(); // 傾きを適用
                break;

            case TiltState.TiltReset:
                ApplyTiltReset(); // 傾きをリセット
                break;
        }

        // 状態切り替えの監視
        MonitorStateSwitch();
    }

    // 傾きを適用する処理
    private void ApplyTiltAdapting()
    {
        // Y軸方向の移動を固定
        Vector3 currentPosition = transform.position;
        currentPosition.y = initialYPosition;
        transform.position = currentPosition;

        // Y軸の回転を固定
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = initialYRotation;

        // X軸とZ軸の回転を制限
        currentRotation.x = Mathf.Clamp(currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x, -maxTiltAngleX, maxTiltAngleX);
        currentRotation.z = Mathf.Clamp(currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z, -maxTiltAngleZ, maxTiltAngleZ);

        // RotationXとRotationZに現在の回転を代入
        RotationX = currentRotation.x;
        RotationZ = currentRotation.z;

        // 制限後の回転を適用
        transform.rotation = Quaternion.Euler(currentRotation);

        // 接触中のオブジェクトがある場合、その質量に応じて傾きを加える
        if (isContacting && contactMass > 0f)
        {
            float tiltAmount = Mathf.Clamp(contactMass, 1f, 10f); // 質量に応じた傾きの量を計算
            Vector3 xTiltTorque = Vector3.right * -tiltAmount;    // X軸方向のトルク
            Vector3 zTiltTorque = Vector3.forward * -tiltAmount; // Z軸方向のトルク

            // Rigidbodyにトルクを加える
            m_rb.AddTorque(xTiltTorque + zTiltTorque);
        }

        // 復元力を適用して傾きを戻す
        ApplyRestoringForce();
    }

    // 傾きをリセットする処理
    private void ApplyTiltReset()
    {
        // 角速度をリセット
        m_rb.angularVelocity = Vector3.zero; // 角速度をリセット
        
        // 現在のX軸とZ軸の傾きを取得
        (float currentTiltX, float currentTiltZ) = GetCurrentTiltAngles();

        // 徐々に初期状態に戻す
        float newTiltX = Mathf.MoveTowards(currentTiltX, 0, restoringForce * Time.fixedDeltaTime);
        float newTiltZ = Mathf.MoveTowards(currentTiltZ, 0, restoringForce * Time.fixedDeltaTime);

        // 回転を適用
        transform.rotation = Quaternion.Euler(newTiltX, initialYRotation, newTiltZ);

        // RotationXとRotationZを更新
        RotationX = newTiltX;
        RotationZ = newTiltZ;
    }
    
    // 復元力を適用して傾きを安定させる
    private void ApplyRestoringForce()
    {
        // 現在のX軸とZ軸の傾きを取得
        (float tiltX, float tiltZ) = GetCurrentTiltAngles();

        // 傾きに対する復元トルクを計算
        Vector3 restoringTorqueX = Vector3.right * -tiltX * restoringForce;
        Vector3 restoringTorqueZ = Vector3.forward * -tiltZ * restoringForce;

        // Rigidbodyに復元トルクを加える
        m_rb.AddTorque(restoringTorqueX + restoringTorqueZ);
    }
    
    // 現在の傾き角度を取得するメソッド
    private (float, float) GetCurrentTiltAngles()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float tiltX = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        float tiltZ = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;
        return (tiltX, tiltZ);
    }

    // 状態切り替えを監視
    private void MonitorStateSwitch()
    {
        // TiltResetが無効化された場合、自動でTiltAdaptingに切り替え
        if (!isTiltResetActive && currentState == TiltState.TiltReset)
        {
            currentState = TiltState.TiltAdapting;
        }
    }

    // 外部スクリプトからTiltResetの状態を設定
    public void SetTiltResetState(bool isActive)
    {
        isTiltResetActive = isActive;

        if (isActive)
        {
            currentState = TiltState.TiltReset;
        }
        else
        {
            currentState = TiltState.TiltAdapting;
        }
    }

    // プレイヤーとの接触時に呼ばれる
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;

        if (otherRb != null && collision.gameObject.CompareTag("Player"))
        {
            contactMass = otherRb.mass; // プレイヤーの質量を記録
            isContacting = true; // 接触中フラグを立てる
        }
    }

    // 接触中に呼ばれる
    void OnCollisionStay(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;

        if (otherRb != null && collision.gameObject.CompareTag("Player"))
        {
            contactMass = otherRb.mass; // プレイヤーの質量を更新
        }
    }

    // 接触が終了した時に呼ばれる
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            contactMass = 0f; // 接触が終了したので質量をリセット
            isContacting = false; // 接触中フラグを解除
        }
    }
}
