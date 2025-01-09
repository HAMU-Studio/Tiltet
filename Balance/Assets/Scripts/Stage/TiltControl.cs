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

    // 外部スクリプトから制御するフラグ
    //private bool forceZeroTilt = false;

    // 水平に戻るまでの時間（秒）
    //[SerializeField] private float levelingTime = 1f;

    // 水平に戻る速度
    //private float levelingSpeed;

    // RotationのXとZの値を公開
    public float RotationX { get; private set; }
    public float RotationZ { get; private set; }

    // 外部からフラグを設定するプロパティ
    /*public bool ForceZeroTilt
    {
        get => forceZeroTilt;
        set
        {
            forceZeroTilt = value;
            if (forceZeroTilt)
            {
                // 水平に戻る速度を計算 (1秒あたりの戻る割合)
                levelingSpeed = 1f / levelingTime;
            }
        }
    }*/

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
        currentPosition.y = initialYPosition; // オブジェクトのY座標を初期位置に固定
        transform.position = currentPosition;

        // Y軸の回転を固定
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y = initialYRotation; // Y軸の回転角度を初期値に固定

        /*if (forceZeroTilt)
        {
            // 傾きをゼロに戻す処理
            float tiltX = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
            float tiltZ = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;

            // 水平に戻る回転角度を徐々に計算
            tiltX = Mathf.MoveTowards(tiltX, 0f, levelingSpeed * Time.fixedDeltaTime * maxTiltAngleX);
            tiltZ = Mathf.MoveTowards(tiltZ, 0f, levelingSpeed * Time.fixedDeltaTime * maxTiltAngleZ);

            // 回転を更新
            currentRotation.x = tiltX;
            currentRotation.z = tiltZ;

            // RotationXとRotationZを更新
            RotationX = tiltX;
            RotationZ = tiltZ;

            // 回転を適用
            transform.rotation = Quaternion.Euler(currentRotation);

            // フラグが有効な間はこれ以降の処理をスキップ
            return;
        }*/

        // X軸とZ軸の回転を制限（最大角度を超えないようにする）
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

            // Rigidbodyにトルクを加える（傾きを適用）
            m_rb.AddTorque(xTiltTorque + zTiltTorque);
        }

        // 復元力を適用して傾きを戻す処理
        ApplyRestoringForce();
    }

    // 復元力を適用して傾きを安定させる
    private void ApplyRestoringForce()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // 現在のX軸、Z軸の傾きを計算
        float tiltX = currentRotation.x > 180 ? currentRotation.x - 360 : currentRotation.x;
        float tiltZ = currentRotation.z > 180 ? currentRotation.z - 360 : currentRotation.z;

        // 傾きに対する復元トルクを計算
        Vector3 restoringTorqueX = Vector3.right * -tiltX * restoringForce;
        Vector3 restoringTorqueZ = Vector3.forward * -tiltZ * restoringForce;

        // Rigidbodyに復元トルクを加える
        m_rb.AddTorque(restoringTorqueX + restoringTorqueZ);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突相手のRigidbodyを取得
        Rigidbody otherRb = collision.rigidbody;

        // 衝突相手がPlayerタグを持っている場合、質量を記録し接触状態を有効化
        if (otherRb != null && collision.gameObject.CompareTag("Player"))
        {
            contactMass = otherRb.mass;
            isContacting = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // 衝突相手のRigidbodyを取得
        Rigidbody otherRb = collision.rigidbody;

        // 衝突相手がPlayerタグを持っている場合、質量を更新
        if (otherRb != null && collision.gameObject.CompareTag("Player"))
        {
            contactMass = otherRb.mass;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 衝突相手がPlayerタグを持っている場合、質量をリセットし接触状態を無効化
        if (collision.gameObject.CompareTag("Player"))
        {
            contactMass = 0f;
            isContacting = false;
        }
    }
}
