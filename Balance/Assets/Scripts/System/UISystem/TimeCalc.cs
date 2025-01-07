using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class TimeCalc : MonoBehaviour
    {
        [Header("時間(3桁)")]
        [SerializeField] Image[] timeImg;

        [SerializeField] private NumImage _numImage;
        
        int[] timeArray;
        float time, beforeTime, floarTime;
        private bool isStopTimer;

        public bool IsStop
        {
            get { return isStopTimer; }
            set { isStopTimer = value; }
        }
        
        private void Update()
        {
            TimeDisplayUpdate();
        }

        private void TimeDisplayUpdate()
        {
            if (isStopTimer)
            {
                Debug.Log("TimerStop !");
                return;
            }
            
            time += Time.deltaTime;

            floarTime = Mathf.Floor(time);   //切り捨て

            //Debug.Log("floarTime" + floarTime);

            if (floarTime - beforeTime >= 1.0f) //1フレーム前の時間から変化していたら(1秒経過したら)繰り上げ処理
            {
                for (int i = 0; i < floarTime - beforeTime; i++)
                {
                    timeArray[0]++;
                    if (timeArray[0] >= 10)
                    {
                        timeArray[1]++;        //繰り上げ処理
                        timeArray[0] = 0;
                    }
                    if (timeArray[1] >= 10)
                    {
                        timeArray[2]++;
                        timeArray[1] = 0;
                    }
                }
            }

            for (int i = 0; i < timeImg.Length; i++)
            {
                timeImg[i].sprite = _numImage.numbers[timeArray[i]];

                timeImg[i].enabled = true;
            }
            
            beforeTime = floarTime;
        }
    }
}