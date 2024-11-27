using System.Collections;
using UnityEngine;

namespace FadeSystem
{
    public class FadeImage : MonoBehaviour, IFadeHandler
    {
        private bool fadeComplete = false;

        public void StartFadeOut()
        {
            Debug.Log("フェードアウト開始");
            fadeComplete = false;

            StartCoroutine(FadeOut());
        }

        public bool IsFadeOutComplete()
        {
            return fadeComplete;
        }

        private IEnumerator FadeOut()
        {
            // 仮
            yield return new WaitForSeconds(2.0f);
            fadeComplete = true;
            Debug.Log("フェードアウト完了");
        }
    }
}