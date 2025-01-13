using FadeSystem;
using System;
using UnityEngine;

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
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.U))
            {
                if (!once)
                {
                    _transition.FadeStart();
                    GameManager.instance.CurrentState = GameState.EnemyBattle;
                    Debug.Log("書き換え");
                    once = true;
                }

            }
            
            if (Input.GetKey(KeyCode.Q))
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