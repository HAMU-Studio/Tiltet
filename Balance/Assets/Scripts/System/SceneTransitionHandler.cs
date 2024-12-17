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
            if (scene.name == "Fight" || scene.name == "GreenStage")
            {
                GameManager.instance.RespawnPlayer(true);
            }

            if (GameManager.instance.CurrentState == GameState.Restart)
            {
                GameManager.instance.RestartAtSavePoint(true);
                Debug.Log("call A");
            }

            m_beforeSceneName = scene.name;
        }

        /// <summary>
        /// 新しいシーンがロードされた直後の処理
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"シーンがロードされました: {scene.name} | モード: {mode}");
           
            if (scene.name == "Fight" || scene.name == "GreenStage")
            {
                GameManager.instance.RespawnPlayer(false);
            }
            
            // 戦闘->探索
            if (m_beforeSceneName == "Fight" && scene.name == "GreenStage")
            {
                GameManager.instance.RespawnPlayer(false);
                GameManager.instance.Back2Search();
            }
            if (GameManager.instance.CurrentState == GameState.Restart)
            {
                Debug.Log("call B");
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