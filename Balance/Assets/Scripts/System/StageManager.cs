using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private TiltControl m_tiltControl; // TiltControl の参照
    private StageMovement m_stageMovement; // StageMovement の参照
    private Rigidbody m_rb; // Rigidbody の参照

    // FallArea への参照を追加
    [SerializeField] private FallArea fallArea; // 落下エリアを管理するクラス

    private void Start()
    {
        // TiltControl コンポーネントを取得
        m_tiltControl = GetComponent<TiltControl>();
        if (m_tiltControl == null)
        {
            Debug.LogError("TiltControl コンポーネントが見つかりません。");
        }

        // StageMovement コンポーネントを取得
        m_stageMovement = GetComponent<StageMovement>();
        if (m_stageMovement == null)
        {
            Debug.LogError("StageMovement コンポーネントが見つかりません。");
        }

        // Rigidbody コンポーネントを取得
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
        {
            Debug.LogError("Rigidbody コンポーネントが見つかりません。");
        }

        // FallArea コンポーネントを取得
        if (fallArea == null)
        {
            Debug.LogError("FallArea コンポーネントが見つかりません。");
        }

        // ゲーム管理システムに航空機インスタンスを保存
        GameManager.instance.SaveAircraftInstance(gameObject);

        // スタート時の位置を設定
        if (GameManager.instance.CurrentState == GameState.Search)
            transform.position = new Vector3(transform.position.x, 50f, transform.position.z);
    }

    private void Update()
    {
        // FallArea の waitRescue が true なら StageMovement の Stop 状態を有効にし、false なら無効にする
        m_stageMovement.IsStopActive = fallArea.WaitRescue;
    }

    // 衝突時の処理
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突相手のタグが "StageObject" の場合
        if (collision.gameObject.CompareTag("StageObject"))
        {
            m_stageMovement.IsNeutralActive = true; // Neutral 状態を有効にする
            Debug.Log("StageMovement の Neutral 状態を有効にしました。");

            // 衝突点からオブジェクトの中心方向を計算
            Vector3 contactPoint = collision.contacts[0].point; // 衝突点
            Vector3 forceDirection = (transform.position - contactPoint).normalized; // オブジェクトの中心方向を計算

            float forceMagnitude = 100f; // 力の大きさ
            Vector3 force = forceDirection * forceMagnitude; // 力のベクトルを生成

            m_rb.AddForce(force, ForceMode.Impulse); // 力を瞬間的に加える

            m_tiltControl.SetTiltResetState(true); // 傾きをリセットする状態に切り替え
            Debug.Log("TiltReset を有効にしました。");

            // 衝突時にUIや音を処理
            InGameUISystems.instance.HitObstacle();
            SoundManager.instance.Play("AircraftHit");

            // Neutral 状態を数秒後に無効にする処理を開始
            StartCoroutine(DisableNeutralStateAfterDelay(2f));
        }
    }

    // Neutral 状態を遅延して無効にするコルーチン
    private IEnumerator DisableNeutralStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Neutral 状態を無効にする
        m_stageMovement.IsNeutralActive = false;
        Debug.Log("StageMovement の Neutral 状態を無効にしました。");

        m_tiltControl.SetTiltResetState(false); // 傾きリセットを無効にする
        Debug.Log("TiltReset を無効にしました。");
    }
}
