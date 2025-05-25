using System.FadeSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Test
{
    public class FadeTest : MonoBehaviour
    {
       [SerializeField] private FadeAndSceneTransition _transition;

        //   private ThrowawayMethod method = new ThrowawayMethod();

       private bool once;


        //    private ThrowawayMethod method2;
        private void Start() 
        {
            once = false;
            StartCoroutine(DelayStartOpning());
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.U))
            {
                if (!once)
                {
                    _transition.StartTransition();
                    GameManager.instance.CurrentState = GameState.EnemyBattle;
                    Debug.Log("書き換え");
                    once = true;
                }

            }

            OnFinishVideo();

        }

        private IEnumerator DelayStartOpning()
        {
            m_videoPlayer.Prepare();
            yield return new WaitForSeconds(0.8f);
            m_videoPlayer.Play();
        }

       [SerializeField] private VideoPlayer m_videoPlayer;
        private void OnFinishVideo()
        {
            if (m_videoPlayer.isPaused == true)
            {
                if (!once)
                {
                    _transition.StartTransition();
                    GameManager.instance.CurrentState = GameState.Search;
                    once = true;
                }
            }
                
        }
    }
}