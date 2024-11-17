using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// SpawnSensorと合体させたい
/// </summary>
public class ReceiveNotificationExample : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager = default!;
    [SerializeField] private PlayerController _playerController = default!;
    [SerializeField] private Material m_defaultMaterial = default!;
 
    // プレイヤー入室時に受け取る通知
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が入室！");
       GameObject obj =  playerInput.gameObject.GetComponent<GameObject>();
       
        //2Pならカラーを変える
        _playerController.Change2PColor(playerInput.user.index);
    }

    // プレイヤー退室時に受け取る通知
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が退室！");
       
        //注意 : 再接続した時Materialおかしくなるかも
    }

    private void OnApplicationQuit()
    {
        //終了処理として入れ替えたplayerのmaterialを元に戻す
        ResetPlayerMaterials();
    }

    private void ResetPlayerMaterials()
    {
        Renderer _renderer = _playerInputManager.playerPrefab.gameObject.GetComponentInChildren<Renderer>();
        Material[] newMaterials = _renderer.sharedMaterials;
    //    newMaterials[0] = m_defaultMaterial;
        newMaterials[1] = m_defaultMaterial;
        _renderer.sharedMaterials = newMaterials;
    }
}