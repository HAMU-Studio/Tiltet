using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        //終了処理として入れ替えたplayerのmaterialを元に戻す
        // _playerInputManager.playerPrefab.gameObject.GetComponent<Renderer>().material = m_defaultMaterial;
     
    }

    private void OnApplicationQuit()
    {
        _playerInputManager.playerPrefab.gameObject.GetComponent<Renderer>().material = m_defaultMaterial;
    }
}