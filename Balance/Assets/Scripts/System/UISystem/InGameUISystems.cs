using UnityEngine;
using System;
namespace System
{
   
    public class InGameUISystems : MonoBehaviour
    {
        public BlinkingSystem _blinkingSystem;
        public Score _score;
        public TimeCalc _timeCalc;

        public void HitObstacle()
        {
            if (GameManager.instance.Life <= 0) return;
            
            int i = 4 - GameManager.instance.Life;  // 体力を逆順にしないと動かないっぽい
            _blinkingSystem.StartCoroutine(_blinkingSystem.DamageIndication(i));
        }
        
        
    }
}