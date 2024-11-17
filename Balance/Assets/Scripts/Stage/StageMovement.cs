using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    private enum MovePattern
    {
        None,
        Forward,    //前方
        Backward,   //後方
        Left,       //左
        FrontLeft,  //左前方
        RearLeft,   //左後方
        Right,      //右
        ForntRight, //右前方
        RearRight   //右後方
    }

    //移動パターン別で詳細数値変更可能
    [SerializeField] private MovePattern currentPattern = MovePattern.None;

    // 移動速度
    [Header("< 方向別移動速度 >")]
    [SerializeField] private float forward = 3f; 　
    [SerializeField] private float backward = -3f; 
    [SerializeField] private float left = -3f; 　　
    [SerializeField] private float right = 3f; 　　

    // 前後・左右移動の有効条件
    // ここで設定している数値は、探査機を動かす基準角度。つまりforwardTiltMinXを５にした場合、探査機の傾きがX=５度以上になると前方移動を有効化する仕組み。
    [Header("< 前後・左右移動の有効条件角度 >\n例）forwardTiltMinXが5以上で前方移動")]
    [SerializeField] private float forwardTiltX = 5f;　　// 前方移動の有効条件数値
    [SerializeField] private float backwardTiltX = -5f;  // 後方移動
    [SerializeField] private float leftTiltZ = 5f;       // 左移動
    [SerializeField] private float rightTiltZ = -5f;     // 右移動

    // 左前方・左後方移動の有効条件
    [Header("< 左前方・左後方の有効条件角度 >\n例）frontLeftTiltMinが5以上かつ、referenceLeftTiltMinが5以上なら左前移動")]
    [SerializeField] private float frontLeftTilt = 5f;      // 左前方移動の有効条件数値
    [SerializeField] private float rearLeftTilt = -5f;　　　// 左後方移動
    [SerializeField] private float standardLeftTilt = 5f;   // 左前方・左後方移動における左移動の基準有効条件数値

    // 右前方・右後方移動の有効条件
    [Header("< 右前方・右後方の有効条件角度 >\n例）frontRightTiltMinが5以上かつ、referenceRightTiltMinが-5以下なら左前移動")]
    [SerializeField] private float frontRightTilt = 5f;      // 右前方移動の有効条件数値
    [SerializeField] private float rearRightTilt = -5f;      // 右後方移動
    [SerializeField] private float standardRightTilt = -5f;  // 右前方・右後方移動における右移動の基準有効条件数値

    // 傾き範囲の振れ幅
    // 全ての移動パターンは-7.5～7.5度以内(初期値)が有効範囲    
    [Header("< 傾き有効範囲振れ幅 >\n例）左右移動を維持できる傾き角度の振れ幅はtiltMinX(-7.5)～tiltMaxX(7.5)以内")]
    [SerializeField] private float tiltMinX = -7.5f; // X軸の感知する角度の最小値
    [SerializeField] private float tiltMaxX = 7.5f;　// X軸の感知する角度の最大値
    [SerializeField] private float tiltMinZ = -7.5f; // Z軸の感知する角度の最小値
    [SerializeField] private float tiltMaxZ = 7.5f;  // Z軸の感知する角度の最大値

    private Rigidbody m_rb;
    private TiltControl m_tiltControl;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_tiltControl = GetComponent<TiltControl>();

        if (m_rb == null)
        {
            Debug.LogError("Rigidbodyが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }

        if (m_tiltControl == null)
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
        float tiltX = m_tiltControl.CurrentTiltX;
        float tiltZ = m_tiltControl.CurrentTiltZ;

        // 前方移動の有効条件
        if (tiltX >= forwardTiltX  && tiltZ > tiltMinZ && tiltZ < tiltMaxZ)
        {
            currentPattern = MovePattern.Forward;// 移動パターンを有効化
        }
        // 後方移動の有効条件
        else if (tiltX <= backwardTiltX  && tiltZ > tiltMinZ && tiltZ < tiltMaxZ)
        {
            currentPattern = MovePattern.Backward;
        }
        // 左移動の有効条件
        else if (tiltZ >= leftTiltZ  && tiltX > tiltMinX && tiltX < tiltMaxX)
        {
            currentPattern = MovePattern.Left;
        }
        // 左前方移動の有効条件
        else if (tiltX >= frontLeftTilt && tiltZ >= standardLeftTilt) 
        {
            currentPattern = MovePattern.FrontLeft;
        }
        // 左後方移動の有効条件
        else if(tiltX <= rearLeftTilt && tiltZ >= standardLeftTilt)
        {
            currentPattern = MovePattern.RearLeft;
        }
        // 右移動の有効条件
        else if (tiltZ <= rightTiltZ  && tiltX >= tiltMinX && tiltX <= tiltMaxX)
        {
            currentPattern = MovePattern.Right;
        }
        // 右前方移動の有効条件
        else if (tiltX >= frontRightTilt && tiltZ <= standardRightTilt)
        {
            currentPattern = MovePattern.ForntRight;
        }
        // 右後方移動の有効条件
        else if (tiltX <= rearRightTilt && tiltZ <= standardRightTilt)
        {
            currentPattern = MovePattern.RearRight;
        }

        else
        {
            currentPattern = MovePattern.None;
        }
    }

    // 各MovePatternの動作内容
    private void ApplyMovement()
    {
        // 現在の移動パターンに基づいて探査機を動かす
        switch (currentPattern)
        {
            case MovePattern.Forward:
                m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, forward); // 前方に移動
                break;
            case MovePattern.Backward:
                m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, backward); // 後方に移動
                break;
            case MovePattern.Left:
                m_rb.velocity = new Vector3(left, m_rb.velocity.y, m_rb.velocity.z); // 左に移動
                break;
            case MovePattern.FrontLeft:
                m_rb.velocity = new Vector3(left, m_rb.velocity.y, forward); // 左前方に移動
                break;
            case MovePattern.RearLeft:
                m_rb.velocity = new Vector3(left, m_rb.velocity.y, backward); // 左後方に移動
                break;
            case MovePattern.Right:
                m_rb.velocity = new Vector3(right, m_rb.velocity.y, m_rb.velocity.z); // 右に移動
                break;
            case MovePattern.ForntRight:
                m_rb.velocity = new Vector3(right, m_rb.velocity.y, forward); // 右前方に移動
                break;
            case MovePattern.RearRight:
                m_rb.velocity = new Vector3(right, m_rb.velocity.y, backward); // 右後方に移動
                break;
            case MovePattern.None:
                m_rb.velocity = Vector3.zero; // 全ての軸の速度をゼロにして静止
                break;
        }
    }
}