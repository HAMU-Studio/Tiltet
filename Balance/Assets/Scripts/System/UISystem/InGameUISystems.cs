using UnityEngine;
using System;
using UnityEngine.UI;

namespace System
{
   
    public class InGameUISystems : MonoBehaviour
    {
        public static InGameUISystems instance = null;
        
        public BlinkingSystem _blinkingSystem;
        public Score _score;
        public TimeCalc _timeCalc;

        //[SerializeField] private GameObject[] systemMessege;

        private int hitCount = 0;

        private void Awake()
        {
            hitCount = 0;
            if (instance == null)
            {
                transform.parent = null;
                instance = this;
            }
        }

        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.P))
            {
                HitObstacle();
            }*/
        }

        public void HitObstacle()
        {
            if (GameManager.instance.Life <= 0) return;
            
            int i = GameManager.instance.Life-1;  // 体力を逆順にしないと動かないっぽい
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