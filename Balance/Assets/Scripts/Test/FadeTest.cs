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
   //    private ThrowawayMethod method2;
        private void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                if (!once)
                {
                    Debug.Log("PressQ");
                    _transition.FadeStart();
                    GameManager.instance.CurrentState = GameState.EnemyBattle;
                    once = true;
                }

               // method.RunOnce( );
            }
            
            if (Input.GetKey(KeyCode.E))
            {
                if (!once)
                {
                    _transition.FadeStart();
                    Debug.Log("PressE");
                    GameManager.instance.CurrentState = GameState.Search;
                    once = true;
                }
            }
        }
    }
}