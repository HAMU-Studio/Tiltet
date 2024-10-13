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
    public bool canActivated { get; private set; } = false; // 他のスクリプトで参照できるフラグ

    public Material redMaterial; // 赤色のマテリアル
    private Material originalMaterial; // 元のマテリアルを保存
    private Renderer objectRenderer; // オブジェクトのRenderer

    public bool IsSwitchOn // 他のスクリプトからスイッチの状態を取得できるプロパティ
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
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = true;
            canActivated = true; // Playerが接触している場合にフラグを立てる
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = false;
            canActivated = false; // Playerが離れた場合にフラグを下げる
        }
    }

    private void Update()
    {
        // 使用できるボタンが分かり次第変える。今は仮でFキーに設定している。
        if (isPlayerInContact && Input.GetKeyDown(KeyCode.F))
        {
            if (activeSwitch != null && activeSwitch != this) // 他のスイッチがアクティブな場合
            {
                activeSwitch.SetSwitchState(false); // 他のスイッチをオフにする
            }

            isSwitchOn = !isSwitchOn; // スイッチの状態を切り替える
            activeSwitch = isSwitchOn ? this : null; // アクティブなスイッチを更新
            Debug.Log("オン: " + switchType);

            // スイッチの状態に応じてマテリアルを切り替える
            objectRenderer.material = isSwitchOn ? redMaterial : originalMaterial;
        }
    }

    // スイッチの状態を設定するメソッド
    private void SetSwitchState(bool state)
    {
        isSwitchOn = state;
        Debug.Log($"{switchType} スイッチが {(state ? "オン" : "オフ")} になりました。");

        // スイッチの状態に応じてマテリアルを切り替える
        objectRenderer.material = state ? redMaterial : originalMaterial;

        if (!state) activeSwitch = null; // スイッチがオフになったらアクティブスイッチをリセット
    }
}
