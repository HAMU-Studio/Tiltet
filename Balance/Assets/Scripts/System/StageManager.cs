using System;
using System.Collections;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    //private TiltControl m_tillControl;
    private StageMovement m_stageMovement;
    private Rigidbody m_rb;

    // FallArea への参照を追加
    [SerializeField] private FallArea fallArea;

    private void Start()
    {
        // TiltControl コンポーネントを取得
        /*m_tillControl = GetComponent<TiltControl>();
        if (m_tillControl == null)
        {
            Debug.LogError("TiltControl コンポーネントが見つかりません。");
        }*/

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

        GameManager.instance.SaveAircraftInstance(gameObject);
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

            //m_tillControl.ForceZeroTilt = true;

            // Neutral 状態を数秒後に無効にする処理を開始
            StartCoroutine(DisableNeutralStateAfterDelay(1.5f));
        }
    }

    // Neutral 状態を遅延して無効にするコルーチン
    private IEnumerator DisableNeutralStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        //m_tillControl.ForceZeroTilt = false;

        m_stageMovement.IsNeutralActive = false; // Neutral 状態を無効にする
        Debug.Log("StageMovement の Neutral 状態を無効にしました。");
    }
}
