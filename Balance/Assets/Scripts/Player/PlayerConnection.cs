using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerConnection : MonoBehaviour
{
　　 [SerializeField] private PlayerInputManager playerInputManager = default!;
    [SerializeField] private PlayerController playerController = default!;
    [SerializeField] private Material defaultMaterial = default!;
 
    // プレイヤー入室時に受け取る通知
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が入室！");
        PlayerCondition condition =  playerInput.gameObject.GetComponent<PlayerCondition>();
        condition.PlayerNum = playerInput.user.index + 1;
       
        // 2Pならカラーを変える
        playerController.ChangePlayerColor(playerInput.user.index);
    }

    // プレイヤー退室時に受け取る通知
    public void OnPlayerLeft(PlayerInput playerInput)
    {
        print($"プレイヤー#{playerInput.user.index}が退室！");
        //注意 : 再接続した時Materialおかしくなる時がまれにあり
    }

    private void OnApplicationQuit()
    {
        // 終了処理として入れ替えたplayerのmaterialを元に戻す
        ResetPlayerMaterials();
    }

    private void ResetPlayerMaterials()
    {
        Renderer _renderer = playerInputManager.playerPrefab.gameObject.GetComponentInChildren<Renderer>();
        Material[] newMaterials = _renderer.sharedMaterials;
        newMaterials[1] = defaultMaterial;
        _renderer.sharedMaterials = newMaterials;
    }
}