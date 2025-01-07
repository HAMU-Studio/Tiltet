using UnityEngine;
using System;
namespace System
{
   
    public class InGameUISystems : MonoBehaviour
    {
        public BlinkingSystem _blinkingSystem;
        public Score _score;
        public TimeCalc _timeCalc;

        private int hitCount = 0;

        private void Awake()
        {
            hitCount = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                HitObstacle();
            }
        }

        public void HitObstacle()
        {
            if (GameManager.instance.Life <= 0) return;
            
            int i = 3 - GameManager.instance.Life;  // 体力を逆順にしないと動かないっぽい
            _blinkingSystem.StartCoroutine(_blinkingSystem.DamageIndication(i));
        }

        public void StartTimer()
        {
            _timeCalc.IsStop = false;
        }

        public void StopTimer()
        {
            _timeCalc.IsStop = true;
        }
        
        
    }
}