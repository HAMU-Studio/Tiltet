using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private StageMovement m_stageMovement;
    private Rigidbody m_rb;

    // FallArea への参照を追加
    [SerializeField] private FallArea fallArea;

    [SerializeField] private float knockbackForce = 5f; // 横ノックバックの強さ
    [SerializeField] private float knockbackUpForce = 2f; // 縦ノックバックの強さ
    
    private void Awake()
    {
        // StageMovement コンポーネントを取得
        m_stageMovement = GetComponent<StageMovement>();
        if (m_stageMovement == null)
        {
            Debug.LogError("StageMovement コンポーネントが見つかりません。このスクリプトは同じオブジェクトにアタッチされる必要があります。");
        }

        // Rigidbody コンポーネントを取得
        m_rb = GetComponent<Rigidbody>();
        if (m_rb == null)
        {
            Debug.LogError("Rigidbody コンポーネントが見つかりません。このスクリプトは同じオブジェクトにアタッチされる必要があります。");
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
            m_stageMovement.IsStopActive = true; // Stop 状態を有効にする
            Debug.Log("StageMovement の Stop 状態を有効にしました。");

            // ノックバック方向の計算
            Vector3 direction = (transform.position - collision.gameObject.transform.position).normalized;
            direction.y = 0; // 水平方向のみに制限

            // 水平方向のノックバック
            m_rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

            // 上方向のノックバック
            m_rb.AddForce(transform.up * knockbackUpForce, ForceMode.Impulse);

            Debug.Log("ノックバック処理を適用しました。");
        }
    }
}
