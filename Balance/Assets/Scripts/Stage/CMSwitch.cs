using UnityEngine;

public class CMSwitch : MonoBehaviour
{
    public enum SwitchType
    {
        Front,
        Back,
        Left,
        Right
    }

    public SwitchType switchType; // オブジェクトのタイプを設定する

    public static CMSwitch activeSwitch = null; // 現在アクティブなスイッチを追跡する

    private bool isPlayerInContact = false; // Playerと接触しているかを確認するフラグ
    private bool isSwitchOn = false; // スイッチの状態

    public bool IsSwitchOn // 他のスクリプトからスイッチの状態を取得できるプロパティ
    {
        get { return isSwitchOn; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = false;
        }
    }

    public void Update()
    {
        if (isPlayerInContact && Input.GetKeyDown(KeyCode.F))
        {
            if (activeSwitch != null && activeSwitch != this) // 他のスイッチがアクティブな場合
            {
                activeSwitch.SetSwitchState(false); // 他のスイッチをオフにする
            }

            isSwitchOn = !isSwitchOn; // スイッチの状態を切り替える
            activeSwitch = isSwitchOn ? this : null; // アクティブなスイッチを更新
            Debug.Log("オン: " + switchType); 
        }
    }

    // スイッチの状態を設定するメソッド
    public void SetSwitchState(bool state)
    {
        isSwitchOn = state;
        Debug.Log($"{switchType} スイッチが {(state ? "オン" : "オフ")} になりました。");
        if (!state) activeSwitch = null; // スイッチがオフになったらアクティブスイッチをリセット
    }
}
