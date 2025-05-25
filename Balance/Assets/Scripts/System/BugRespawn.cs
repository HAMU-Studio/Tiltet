using UnityEngine;

/// <summary>
/// 物理挙動がおかしくなり、落ちるはずのない場面で自機から落ちた場合の処理
/// </summary>
public class BugRespawn : MonoBehaviour
{
   [SerializeField] private bool isRestart;

    private void OnCollisionEnter(Collision other)
    {
        Debug.LogAssertion("自機から落ちました");
        if (other.gameObject.CompareTag("Player")) 
        {
            if (isRestart)
            {
                GameManager.instance.SceneManager.StartTransition("MainStage");
                GameManager.instance.CurrentState = GameState.Restart;
            }
            else
            {
                GameManager.instance.RespawnPlayer(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogAssertion("自機から落ちました");

        if (other.gameObject.CompareTag("Player"))
        {
            if (isRestart)
            {
                GameManager.instance.SceneManager.StartTransition("MainStage");
                GameManager.instance.CurrentState = GameState.Restart;
            }
            else
            {
                Debug.Log("Call Respawn");
                GameManager.instance.SetPlayerPos();
            }
        }

        if (other.gameObject.CompareTag("SphereEnemy") ||
            other.gameObject.CompareTag("EllipseEnemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
