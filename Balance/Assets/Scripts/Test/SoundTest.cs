using System;
using UnityEngine;

namespace Test
{
 

    public class SoundTest : MonoBehaviour
    {
        [Header("再生したい音の名前")]
        [SerializeField] private string[] soundName;
        
        private void Update()
        {
            for (int i = 0; i < soundName.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    SoundManager.instance.Play(soundName[i]);
                    Debug.Log(i + "番目のサウンドを再生");
                }
            }
        }
    }
}