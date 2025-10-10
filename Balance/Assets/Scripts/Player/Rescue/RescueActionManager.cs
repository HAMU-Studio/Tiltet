using UnityEngine;
using System;
using Dialogue;

namespace Player.Rescue
{

    public class RescueActionManager : MonoBehaviour
    {
       [SerializeField] private PlayerCondition condition;

        Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            condition.OnStateChange += OnStateChange;
            if (GameManager.instance == null)
            {
                Debug.LogError("GameManager is null");
            }
        }

        private void OnDestroy() => condition.OnStateChange -= OnStateChange;

        private void OnStateChange(RescueEventArgs args)
        {
            if (args.PreviousState != State.Wait && args.CurrentState == State.Wait)
            {
                //落ちたら救出開始
                if (GameManager.instance.IsRescue == false)
                {
                    GameManager.instance.IsRescue = true;
                    PlayStruggle();
                }
            }

            // Wait to Move or Wait to Fly
            if (args.PreviousState == State.Wait && args.CurrentState == State.OutsideMove
                || args.PreviousState == State.Wait && args.CurrentState == State.Fly)
            {
                SoundManager.instance.StopPlay("Struggle");
                animator.Play("Walk_01");
                SuperLandDialogue();
                if (args.CurrentState == State.OutsideMove)
                {
                    // 外側に飛ばす音
                }
                else
                {
                    SoundManager.instance.Play("Fly");
                }
            }

            // Move to Fly
            if (args.PreviousState == State.OutsideMove && args.CurrentState == State.Fly)
            {
                SoundManager.instance.Play("Fly");
                animator.Play("Walk_01");
            }

            if (args.PreviousState == State.Fly || args.PreviousState == State.SuperLand)
            {
                //着地したら救出終了
                if (args.CurrentState == State.None)
                {
                    GameManager.instance.IsRescue = false;

                    if (args.PreviousState == State.Fly) //通常着地
                    {
                        ParticleManager.instance.GenerateAndPlay("Landing", this.transform);
                        SoundManager.instance.Play("NormalLanding");
                        animator.SetTrigger("toLand");
                        Debug.Log("NormalLanding");
                    }
                    else
                    {
                        // スーパー着地
                        animator.SetTrigger("toLand");
                        ParticleManager.instance.GenerateAndPlay("Landing", this.transform);
                        SoundManager.instance.Play("SuperLanding");
                    }
                }
            }
        }
        
        private void SuperLandDialogue()
        {
            DisplayDialogue.dialogue.Enqueue("SuperLand");
        }

        public void PlayStruggle()
        {
            animator.SetTrigger("toStruggle");
        }

    }
}