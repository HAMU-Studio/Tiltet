using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace System
{
    public class SoundManager : MonoBehaviour
    {
        //別名(name)をキーとした管理用Dictionary
        private Dictionary<string, BGMData> BGMDictionary = new Dictionary<string, BGMData>();
        private Dictionary<string, SEData> SEDictionary = new Dictionary<string, SEData>();
        
        private static SoundManager instance = null;
        
        //AudioSource（スピーカー）を同時に鳴らしたい音の数だけ用意
        private AudioSource[] audioSourceList = new AudioSource[20];

        [Serializable]
        public class BGMData
        {
            public string    name; 
            public AudioClip audioClip;
        }

        [Serializable]
        public class SEData
        {
            public string    name;
            public AudioClip audioClip;
        }

        [SerializeField] private BGMData[] bgmDatas;
        [SerializeField] private SEData[]  SEDatas;
        private void SetInstance()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Awake()
        {
            for (int i = 0; i < audioSourceList.Length; i++)
            {
                audioSourceList[i] = gameObject.AddComponent<AudioSource>();
            }
            
            //それぞれDictionaryにセット
            foreach (BGMData bgmData in bgmDatas)
            {
                BGMDictionary.Add(bgmData.name, bgmData);
            }

            foreach (SEData SEData in SEDatas)
            {
                SEDictionary.Add(SEData.name, SEData);
            }
            
            SetInstance();
        }

        /// <summary>
        /// 未使用のAudioSourceの取得 全て使用中ならnull
        /// </summary>
        private AudioSource GetUnusedAudioSource()
        {
            for (int i = 0; i < audioSourceList.Length; i++)
            {
                if (audioSourceList[i].isPlaying == false)
                    return audioSourceList[i];
            }
            return null; 
        }
        
        public void PlayBGM(AudioClip bgm)
        {
            // 同時に鳴らす場合を考慮して毎回変数生成する(必要ないかも)
            AudioSource audioSource = GetUnusedAudioSource();

            if (audioSource == null)
            {
                Debug.Log("BGM play failed");
                return;
            }
            audioSource.clip = bgm;
            audioSource.Play();
        }

        public void PlaySE(AudioClip clip)
        {
            // 同時に鳴らす場合を考慮して毎回変数生成する(必要ないかも)
            AudioSource audioSource = GetUnusedAudioSource();

            if (audioSource == null)
            {
                Debug.Log("BGM play failed");
                return;
            }

            audioSource.clip = clip;
            audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// これを再生したいタイミングで呼び出す。名前を指定してBGM、SE一覧から検索、再生。
        /// </summary>
        /// <param name="name">AudioClipの名前でなく登録した別名</param>
        public void Play(string name)
        {
            // それぞれの管理用Dictionaryから別名で検索、一致したら再生
            if (BGMDictionary.TryGetValue(name, out BGMData bgmData))
            {
                PlayBGM(bgmData.audioClip);
            }
            else if (SEDictionary.TryGetValue(name, out SEData seData))
            {
                PlaySE(seData.audioClip);
            }
            else
            {
                Debug.LogWarning("その別名は登録されていません:{name}");
            }
        }

    }
}