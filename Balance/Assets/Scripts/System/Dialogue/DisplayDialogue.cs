using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Dialogue
{
    public class DisplayDialogue : MonoBehaviour
    {
        [SerializeField] private bool isDialogue;
        public static DisplayDialogue dialogue;
        public static DisplayDialogue system;
        
        Queue<DialogueData> _task = new Queue<DialogueData>();

        [SerializeField] private DialogueDatas[] dialogueDatas;
        private Dictionary<string, DialogueData> dataDictionary = new();

        [SerializeField] private Image image;
        
        private void Awake()
        {
            // セリフ表示用とシステムメッセージ表示用で分ける
            if (isDialogue)
            {
                dialogue = this;
            }
            else
            {
                system = this;
            }
            InitializeDataDictionary();
            image.enabled = false;
        }

        private void InitializeDataDictionary()
        {
            for (int i = 0; i < dialogueDatas.Length; i++)
            {
                for (int j = 0; j < dialogueDatas[i].data.Length; j++)
                {
                    var dialogueData = dialogueDatas[i].data[j];
                    if (dialogueData != null && !string.IsNullOrEmpty(dialogueData.Name))
                    {
                        dataDictionary.TryAdd(dialogueData.Name, dialogueData);
                    }
                }
            }
        }

        private float elapsedTime;
        private bool isShowing; 
        private DialogueData _data;
        private void Update()
        {
            if (!isShowing && _task.Count > 0)
            {
                 _data = _task.Dequeue();
                Display(_data.Dialogue);
                isShowing = true;
            }
            
            
            if (isShowing)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime > _data.DisplayTime / 2f)
                {
                    // 表示時間の半分でしゃべり停止
                    SoundManager.instance.StopPlay("Speak");
                }

                if (elapsedTime > _data.DisplayTime)
                {
                    CheckHide();
                    isShowing = false;
                    elapsedTime = 0f;
                }
            }
        }

        private void Display(Sprite dialogue)
        {
            image.sprite = dialogue;
            if (!image.enabled) image.enabled = true;
            
            if (isDialogue)
             SoundManager.instance.Play("Speak");   // しゃべるのはセリフの時だけ
        
        }

        private void CheckHide()
        {
            if (image.sprite == null)
                return;
            
            // 一連のセリフ表示の最後ならimageの表示off
            if (_task.Count == 0)
             image.enabled = false;
          
           　// image.sprite = null;
        }
        
        public void Enqueue(string name)
        {
            DialogueData _data = GetDialogueData(name);
            if (_data == null) return;

            if (_task.Count != 0)   // 他のセリフが表示待機
            {
                if (DamageDialogueCheck(name) == false || CoinDialogueCheck(name) == false)
                    return;
                
            }
            _task.Enqueue(_data);
        }

        private bool DamageDialogueCheck(string name)
        {
            if (name == "hyo" || name == "wa")
            {
                // 他のセリフ表示中に初ダメージ食らったら入れない
                if (GameManager.instance.Life == 11)
                {
                    Debug.Log("Damage dialogue is Skipped");
                    return false;
                }
            }

            return true;
        }

        private bool CoinDialogueCheck(string name)
        {
            if (name == "FindCoinGimmick"　|| name == "SubPartGimmick")
                return false;
            
            return true;
        }

        private DialogueData GetDialogueData(string name)
        {
            if (dataDictionary.TryGetValue(name, out DialogueData data))
            {
                return data;
            }
            else
            {
                Debug.Log("Failed to get data");
                return null;
            }
        }

        public IEnumerator DelayEnqueue(string name, float delayTime)
        {
            Debug.Log("Call delayEnqueue");
            yield return new WaitForSeconds(delayTime);
            Enqueue(name);
            yield return null;
        }
    }
}