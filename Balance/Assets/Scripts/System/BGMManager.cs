using UnityEngine;
using System.Collections;

namespace System
{
    /// <summary>
    /// Beta版のみ
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        [Header("シーン開始時のBGM")][SerializeField] String startBGMName;

        [Header("BGMは〇秒遅延かけて再生")]
        [SerializeField] private float waitTime;
        
        void Start()
        {
            StartCoroutine(DelayPlayBGM());
        }
      
        private IEnumerator DelayPlayBGM()
        {
            yield return new WaitForSeconds(waitTime);
            SoundManager.instance.Play(startBGMName);
        }
    }
}