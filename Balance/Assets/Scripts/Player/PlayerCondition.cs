using Unity.Collections;
using UnityEngine;

namespace Player
{
    public enum State
    {
        //紐を無くすタイミングのためThrowingとFlyに分ける
        //飛ばす直前の位置に移動させるためMove追加
        None,
        Wait,
        OutsideMove,   
        Fly,
        SuperLand
    }

    public delegate void RescueEvent(RescueEventArgs args);

    public readonly struct RescueEventArgs
    {
        public readonly State PreviousState;
        public readonly State CurrentState;

        public RescueEventArgs(State previousState, State currentState)
        {
            PreviousState = previousState;
            CurrentState  = currentState;
        }
    }
    
    public class PlayerCondition : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool isPlayer1;
        [SerializeField, ReadOnly] private bool isFlying;
        [SerializeField, ReadOnly] private bool isDashing;
        [SerializeField, ReadOnly] private bool isFreezing;
        [SerializeField, ReadOnly] private bool canMove;
        [SerializeField, ReadOnly] private float moveSpeed;
        [SerializeField, ReadOnly] private State rescueState;
        public event RescueEvent OnStateChange;
        
        public bool IsPlayer1
        {
            get => isPlayer1;
            set => isPlayer1 = value;
        }
        
        public bool IsFlying
        {
            get => isFlying;
            set => isFlying = value;
        }
        public bool IsDashing
        {
            get => isDashing;
            set => isDashing = value;
        }
        
        public bool IsFreezing
        {
            get => isFreezing;
            set => isFreezing = value;
        }
        
        public bool CanMove
        {
            get => canMove;
            set => canMove = value;
        }
        
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }
        
        public State RescueState
        {
            set
            {
                RescueEventArgs args = new RescueEventArgs(rescueState, value);
                OnStateChange?.Invoke(args);
                rescueState = value;
            }
            get => rescueState; 
        }
    }
}