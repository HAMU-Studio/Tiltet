using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltController : MonoBehaviour
{

    //固定したいY軸の回転
    [SerializeField] float FixedYRotation = 0f;

    // AddForceで使用する力の大きさ
    private Vector3 m_forceDirection = new Vector3(0, 0, 10);
    private ForceMode m_forceMode = ForceMode.Force;

    private Rigidbody m_rb;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        m_rb = GetComponent<Rigidbody>();

        // nullチェック
        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }
    }

    void Update()
    {
        //外部で別のRigidbodyを参照したときに発生する問題を防止するために毎フレームNullチェックしている
        // Update内で力を加える（ForceModeがForce以外の場合）
        if (m_rb != null && m_forceMode != ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
        }

        // 現在の回転を取得
        Quaternion currentRotation = transform.rotation;

        // オイラー角に変換
        Vector3 euler = currentRotation.eulerAngles;

        // Y軸の回転を固定
        euler.y = FixedYRotation;

        // X軸とZ軸の回転を制限
        euler.x = ClampAngle(euler.x, -30f, 30f);
        euler.z = ClampAngle(euler.z, -30f, 30f);

        // 回転を更新
        transform.rotation = Quaternion.Euler(euler);
    }

    void FixedUpdate()
    {
        // ForceModeがForceの場合はFixedUpdate内で力を加える
        if (m_rb != null && m_forceMode == ForceMode.Force)
        {
            m_rb.AddForce(m_forceDirection, m_forceMode);
        }
    }
    
    //外部からm_forceDirectionとm_forceModeを取得する場合に使うアクセサメソッド（今は使用していない）
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

}