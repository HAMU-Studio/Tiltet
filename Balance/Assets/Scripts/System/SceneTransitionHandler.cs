using UnityEngine;
using UnityEngine.SceneManagement;

namespace System
{
    public class SceneTransitionHandler : MonoBehaviour
    {
        /// <summary>
        /// これをアタッチしたオブジェクトが有効化された時実行される
        /// </summary>
        private void OnEnable()
        {
            // イベントに登録 引数どうなってるのかよくわからん
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            // イベントから解除
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        /// <summary>
        /// シーンがアンロードされる直前の処理
        /// ※シーンが破棄されるタイミングで呼ばれるのでシーン内のオブジェクトが利用できない
        /// </summary>
        private string m_beforeSceneName;
        private void OnSceneUnloaded(Scene scene)
        {
            Debug.Log($"シーンが破棄されました : {scene.name}");
            if (scene.name == "Fight" || scene.name == "MainStage" || scene.name == "SnowFight" || scene.name == "VolcanoFight")
            {
                GameManager.instance.RespawnPlayer(true);
                ParticleManager.instance.RemoveAll();
               // GameManager.instance.PlayerUnLock();
            }

            if (GameManager.instance.CurrentState == GameState.Restart)
            {
                GameManager.instance.RestartAtSavePoint(true);
            }
            
            

            m_beforeSceneName = scene.name;
        }

        /// <summary>
        /// 新しいシーンがロードされた直後の処理  注意：Awakeより後、Startより前に呼ばれる
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"シーンがロードされました: {scene.name} | モード: {mode}");
            Debug.Log("BeforeScene = " + m_beforeSceneName);
            if (scene.name == "Fight" || scene.name == "MainStage" || scene.name == "SnowFight" || scene.name == "VolcanoFight")
            {
                if (GameManager.instance.isConnected)
                {
                    GameManager.instance.PlayerUnLock();
                    GameManager.instance.RespawnPlayer(false);
                    GameManager.instance.SetPlayerPos();
                }
            }
            
            // 戦闘->探索
            if (m_beforeSceneName == "Fight" || m_beforeSceneName == "VolcanoFight" || m_beforeSceneName == "SnowFight")
            {
                if ( scene.name == "MainStage")
                {
                    GameManager.instance.Back2Search();
                }
            }

            if (m_beforeSceneName == "Start")
            {
                if (scene.name == "MainStage")
                {
                    GameManager.instance.InitGame(false);
                }
            }
      
            if (GameManager.instance.CurrentState == GameState.Restart)
            {
                GameManager.instance.RestartAtSavePoint(false);
                GameManager.instance.CurrentState = GameState.Search;
            }
            
          
            m_beforeSceneName = null;

        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Debug.Log($"アクティブシーンが変更されました : {oldScene.name} -> {newScene.name}");
        }
    }
}