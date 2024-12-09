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
                    GameManager.instance.NextState = GameState.EnemyBattle;
                    once = true;
                }

            }
            
            if (Input.GetKey(KeyCode.I))
            {
                if (!once)
                {
                    _transition.FadeStart();
                    GameManager.instance.NextState = GameState.Search;
                    once = true;
                }
            }

        }
    }
}