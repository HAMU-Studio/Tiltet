using FadeSystem;
using System;
using UnityEngine;

namespace Test
{
    public class FadeTest : MonoBehaviour
    {
       [SerializeField] private FadeAndSceneTransition _transition;
        private void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                _transition.FadeStart();
            }
        }
    }
}