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

    // 傾きの最大角度（インスペクターで調整可能）
    [SerializeField] private float maxTiltAngleX = 30f;
    [SerializeField] private float maxTiltAngleZ = 30f;

    // 復元力の大きさ
    [SerializeField] private float restoringForce = 10f;

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
        // Y軸方向の移動を固定
        Vector3 currentPosition = transform.position;
        currentPosition.y = initialYPosition; // Y軸の位置を初期位置に固定
        transform.position = currentPosition;

        // Y軸の回転を固定
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = initialYRotation; // Y軸の回転を初期回転角度に固定

        // X軸とZ軸の回転を±maxTiltAngleX、±maxTiltAngleZに制限
        currentRotation.x = Mathf.Clamp(currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x, -maxTiltAngleX, maxTiltAngleX);
        currentRotation.z = Mathf.Clamp(currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z, -maxTiltAngleZ, maxTiltAngleZ);

        // 制限後の回転を適用
        transform.rotation = Quaternion.Euler(currentRotation);

        // 接触中のオブジェクトがある場合、その質量に応じて傾きを加える
        if (isContacting && contactMass > 0f)
        {
            // 質量に応じた傾きの強さを計算
            float tiltAmount = Mathf.Clamp(contactMass, 1f, 10f); // 1～10の範囲で傾きを制限
            // X軸とZ軸の傾きを計算（質量に応じて傾く強さを変更）
            Vector3 xTiltTorque = Vector3.right * -tiltAmount;
            Vector3 zTiltTorque = Vector3.forward * -tiltAmount;

            // Rigidbodyにトルクを加える（傾きを適用）
            m_rb.AddTorque(xTiltTorque + zTiltTorque);
        }

        // 復元力を適用して傾きを戻す処理
        ApplyRestoringForce();
    }

    // 復元力を適用して傾きを安定させる
    private void ApplyRestoringForce()
    {
        // 現在の回転角度を取得
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // 回転角度を[-180, 180]の範囲に正規化
        float tiltX = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        float tiltZ = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;

        // X軸とZ軸の傾きに対する復元力を計算
        Vector3 restoringTorqueX = Vector3.right * -tiltX * restoringForce;
        Vector3 restoringTorqueZ = Vector3.forward * -tiltZ * restoringForce;

        // 復元力をRigidbodyに適用
        m_rb.AddTorque(restoringTorqueX + restoringTorqueZ);
    }

    // オブジェクトが接触を開始したとき
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;

        if (otherRb != null)
        {
            // 接触したオブジェクトの質量を取得
            contactMass = otherRb.mass;
            isContacting = true; // 接触状態を記録
            Debug.Log("接触開始: 質量 " + contactMass);
        }
    }

    // オブジェクトが接触している間
    void OnCollisionStay(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;

        if (otherRb != null)
        {
            // 接触している間は質量を更新
            contactMass = otherRb.mass;
        }
    }

    // オブジェクトが接触を終了したとき
    void OnCollisionExit(Collision collision)
    {
        // 接触が終わったので質量をリセット
        contactMass = 0f;
        isContacting = false;
        Debug.Log("接触終了");
    }
}
