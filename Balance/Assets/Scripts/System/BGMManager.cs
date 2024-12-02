using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace System
{
    /// <summary>
    /// Beta版のみ
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        [Header("シーン開始時のBGM")][SerializeField] String StartBGMName;

        [Header("BGMは〇秒遅延かけて再生")]
        [SerializeField] private float waitTime;
        /*[Header("戦闘BGM")][SerializeField] String fightBGMNName;
        [Header("SoundManagerを参照して自動遷移させる")][SerializeField] bool TransitionFightBGM = false;
        private bool fightBGMPlay = false;

        [Header("探索BGM")][SerializeField] String searchBGMName;
        [Header("SoundManagerを参照して自動遷移させる")][SerializeField] bool TransitionSearchBGM;
        private bool searchBGMPlay = false;

        [Header("リザルトBGM")][SerializeField] String ResultBGM;
        [Header("SoundManagerを参照して自動遷移させる")][SerializeField] bool TransitionResultBGM;
        private bool resultBGMPlay = false;*/
        
        void Start()
        {
            StartCoroutine(DelayPlayBGM());
        }
        private void Update()
        {
            /*if (GameManager.instance.CurrentState == GameState.EnemyBattle && TransitionFightBGM && !fightBGMPlay)
            {
                FightBGMPlay();
                fightBGMPlay = true;
            }
            if (GameManager.instance.CurrentState == GameState.Search && TransitionSearchBGM&&!searchBGMPlay)
            {
                SearchBGMPlay();
                searchBGMPlay=true;
            }
            if (GameManager.instance.CurrentState == GameState.Result && TransitionResultBGM&&!resultBGMPlay)
            {
                ResultBGMPlay();
                resultBGMPlay=true;
            }*/
        }
        /*void TitleBGMPlay()
        {
            SoundManager.instance.Play(fightBGMNName);
        }

        void MenuBGMPlay()
        {
            
        }*/

        /*void FightBGMPlay()
        {
            SoundManager.instance.Play(fightBGMNName);
        }

        void SearchBGMPlay()
        {
            SoundManager.instance.Play(searchBGMName);
        }

        void ResultBGMPlay()
        {
            SoundManager.instance.Play(ResultBGM);
        }*/
        private IEnumerator DelayPlayBGM()
        {
            yield return new WaitForSeconds(waitTime);
            SoundManager.instance.Play(StartBGMName);
          
        }
    }

 
}