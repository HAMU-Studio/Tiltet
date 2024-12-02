using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FadeSystem
{
    public class FadeAndSceneTransition : MonoBehaviour
    {
        [Header("フェード処理")] public FadeImage fadeHandler;
        [Header("移動するシーンの名前")] public string nextSceneName;

        private IFadeHandler m_fade;
        private bool isSceneTransitioning = false;

        private void Start()
        {
            //fadeHandlerがIFadeHandlerを実装していればIFadeHandler型に変換して代入
            m_fade = fadeHandler.GetComponent<IFadeHandler>(); 

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
            
            //リセットは戦闘と探索の切り替えのみ
          //  if (nextSceneName == "Fight" || nextSceneName == "GreenStage")
            GameManager.instance.ResetPlayer();
         
            m_fade.StartFadeOut();
            StartCoroutine(LoadNextSceneAsync());
        }

        /// <summary>
        /// 名前指定してシーン移動
        /// </summary>
        /// <param name="nextSceneName"></param>
        /*public void FadeStart(string nextSceneName)
        {
            this.nextSceneName = nextSceneName;
            FadeStart();
        }*/

        [SerializeField] private GameObject m_stage;
        private IEnumerator LoadNextSceneAsync()
        {
            while (m_fade.IsFadeOutComplete() == false)
            {
                yield return null;
            }
    
            yield return  SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

       //     yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
    }
}