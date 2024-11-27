using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FadeSystem
{
    public class FadeAndSceneTransition : MonoBehaviour
    {
        [Header("フェード処理")] public MonoBehaviour fadeHandler;
        [Header("移動するシーンの名前")] public string nextSceneName;

        private IFadeHandler m_fade;
        private bool isSceneTransitioning = false;

        private void Start()
        {
            //fadeHandlerがIFadeHandlerを実装していればIFadeHandler型に変換して代入
            m_fade = fadeHandler as IFadeHandler;

            if (m_fade == null)
            {
                Debug.LogError("No IFadeHandler attached to fadeHandler");
            }
        }

        public void FadeStart()
        {
            if (isSceneTransitioning || m_fade == null)
                return;

            isSceneTransitioning = true;
            m_fade.StartFadeOut();
            StartCoroutine(LoadNextSceneAsync());

        }

        private IEnumerator LoadNextSceneAsync()
        {
            while (m_fade.IsFadeOutComplete() == false)
            {
                yield return null;
            }

            yield return SceneManager.LoadSceneAsync(nextSceneName);
        }
    }
}