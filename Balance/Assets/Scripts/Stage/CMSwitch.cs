using Player;
using System.Collections;
using System.Collections.Generic;
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
    private bool isSwitchPressed = false; // Switchを押したかを通知するフラグ
    private bool isSwitchOn = false; // スイッチの状態

    public Material redMaterial; // 赤色のマテリアル
    private Material originalMaterial; // 元のマテリアルを保存
    private Renderer objectRenderer; // オブジェクトのRenderer

    private int playerNumOnStanding;
    private bool isStandingBoth;

    // 他のスクリプトからプレイヤーの接触の状態を取得できるプロパティ
    public bool IsPlayerInContact()
    {
        return isPlayerInContact;
    }

    // 他のスクリプトからスイッチの状態を取得できるプロパティ
    public bool IsSwitchOn 
    {
        get { return isSwitchOn; }
    }

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>(); // Rendererを取得
        originalMaterial = objectRenderer.material; // 元のマテリアルを保存

        // Frontタイプのスイッチは初期状態でオンに設定
        if (switchType == SwitchType.Front)
        {
            SetSwitchState(true);
            activeSwitch = this; // Frontスイッチを初期アクティブスイッチに設定
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == false)
            return;

        if (isPlayerInContact)      // 同時乗り対策
            isStandingBoth = true;
            
        isPlayerInContact = true;
        PlayerCondition condition = other.gameObject.GetComponent<PlayerCondition>();
        playerNumOnStanding = condition.PlayerNum;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == false)
            return;
        
        isPlayerInContact = false;
        isStandingBoth = false;
    }

    //スイッチの状態を切り替えるメソッド
    private void SwitchPressed()
    {
        if (isSwitchPressed)
        {
            if (activeSwitch != null && activeSwitch != this) // 他のスイッチがアクティブな場合
            {
                activeSwitch.SetSwitchState(false);  // 他のスイッチをオフにする
            }

            isSwitchOn = !isSwitchOn;  // スイッチの状態を切り替える
            activeSwitch = isSwitchOn ? this : null;  // アクティブなスイッチを更新
            Debug.Log("オン: " + switchType);

            // スイッチの状態に応じてマテリアルを切り替える
            objectRenderer.material = isSwitchOn ? redMaterial : originalMaterial;
            isSwitchPressed = false;
        }
    }

    // isSwitchPressedがtrueになった時にSwitchPressedを呼ぶ
    public bool SetSwitchPressed(int playerNum)
    {
        if (playerNum == playerNumOnStanding)
        {
            isSwitchPressed = true;
            SwitchPressed(); // フラグがtrueになったタイミングでSwitchPressedメソッドを呼ぶ
            return true;
        }

        Debug.Log("ボタン上のプレイヤーとボタンを押したプレイヤーが違います。");
        return false;
    }

    // オン状態のスイッチをオフに更新するメソッド
    private void SetSwitchState(bool state)
    {
        isSwitchOn = state;
        Debug.Log($"{switchType} スイッチが {(state ? "オン" : "オフ")} になりました。");

        // スイッチの状態に応じてマテリアルを切り替える
        objectRenderer.material = state ? redMaterial : originalMaterial;

        if (!state) activeSwitch = null; // スイッチがオフになったらアクティブスイッチをリセット
    }
}
