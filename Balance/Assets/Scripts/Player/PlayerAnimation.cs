using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerCondition condition;
        [SerializeField] private Animator animator;
        [SerializeField] private AnimatorStateInfo stateInfo;
        private void Start()
        {
            condition.OnStateChange += OnStateChange;
        }

        private void OnStateChange(RescueEventArgs args)
        {
            if (args.CurrentState == State.SuperLand)
            {
                animator.Play("SuperLand");
            }
        }
    }
}