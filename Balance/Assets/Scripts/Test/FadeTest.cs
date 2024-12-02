using FadeSystem;
using System;
using UnityEngine;

namespace Test
{
    public class FadeTest : MonoBehaviour
    {
       [SerializeField] private FadeAndSceneTransition _transition;

       private ThrowawayMethod method = new ThrowawayMethod();

       private bool once;
        private bool getIsEncount;

        EncountManager encountmanager;

        //    private ThrowawayMethod method2;
        private void Start() 
        {
            once = false;
            getIsEncount = false;

            //EncountManagerにアタッチしてあるEncountManagerのスクリプトから
            GameObject enemyManager = GameObject.Find("EncountManager");
            encountmanager = enemyManager.GetComponent<EncountManager>();
        }

        private void Update()
        {
            //追加しました
            getIsEncount = encountmanager.isEncount;
            if (getIsEncount)
            {
                if (!once)
                {
                    _transition.FadeStart();
                    GameManager.instance.CurrentState = GameState.EnemyBattle;
                    once = true;
                }
            }
            //追加しました

            if (Input.GetKey(KeyCode.U))
            {
                if (!once)
                {
                    _transition.FadeStart();
                    GameManager.instance.CurrentState = GameState.EnemyBattle;
                    once = true;
                }

            }
            
            if (Input.GetKey(KeyCode.I))
            {
                if (!once)
                {
                    _transition.FadeStart();
                    GameManager.instance.CurrentState = GameState.Search;
                    once = true;
                }
            }
        }
    }
}