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
        float time, beforeFrameTime, floorTime;
        private bool isStopTimer;

        private void Awake()
        {
            timeArray = new int[4] { 0, 0, 0, 0 };
            time = 0f;
            isStopTimer = true;
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            Debug.Log("InitializeTimer");
            timeArray = GameManager.instance.SetTimeArray(timeArray);
        }

        private void SaveCurrentTime()
        {
            Debug.Log("SaveTimer");
            GameManager.instance.SaveCurrentTime(timeArray);
        }

        private void OnDestroy() => SaveCurrentTime();

        public bool IsStop
        {
            get { return isStopTimer; }
            set { isStopTimer = value; }
        }
        
        private void Update()
        {
            TimeDisplayUpdate();
        }

        private float elapsedTime;
        private void TimeDisplayUpdate()
        {
            if (isStopTimer) return;
            
            time += Time.deltaTime;

            floorTime = Mathf.Floor(time);   //切り捨て

            //Debug.Log("floarTime" + floarTime);

            elapsedTime = floorTime - beforeFrameTime;

            if (elapsedTime >= 1.0f) //1フレーム前の時間から変化していたら(1秒経過したら)繰り上げ処理
            {
                for (int i = 0; i < elapsedTime; i++)
                {
                    timeArray[0]++;
                    if (timeArray[0] >= 10)
                    {
                        // 10秒経過
                        // 繰り上げ処理
                        timeArray[1]++;      
                        timeArray[0] = 0;
                    }
                    if (timeArray[1] >= 6)
                    {
                        // 1分経過
                        timeArray[2]++;
                        
                        timeArray[0] = 0;
                        timeArray[1] = 0;
                    }

                    if (timeArray[2] >= 10)
                    {
                        // 10分経過
                        timeArray[3]++;
                        
                        timeArray[0] = 0;
                        timeArray[1] = 0;
                        timeArray[2] = 0;
                    }

                    if (timeArray[3] >= 6)
                    {
                        isStopTimer = true;
                        Debug.Log("一時間経過によりタイマー停止");
                    }
                }
            }

            for (int i = 0; i < timeImg.Length; i++)
            {
                timeImg[i].sprite = _numImage.numbers[timeArray[i]];

                timeImg[i].enabled = true;
            }
            
            beforeFrameTime = floorTime;
        }
    }
}