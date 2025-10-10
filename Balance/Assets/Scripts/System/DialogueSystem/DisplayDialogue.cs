using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Dialogue
{
    public class DisplayDialogue : MonoBehaviour
    {
        [SerializeField] private bool isDialogue;
        public static DisplayDialogue dialogue;
        public static DisplayDialogue system;
        
        Queue<DialogueData> m_task = new Queue<DialogueData>();

        [SerializeField] private DialogueDatas[] dialogueDatas;
        private Dictionary<string, DialogueData> m_dataDictionary = new();

        [SerializeField] private Image image;

        [SerializeField] private Animator dialogueAnim;
        [SerializeField] private bool systemMessage;

        public event Action OnDialogueSkipped;
        
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
                        m_dataDictionary.TryAdd(dialogueData.Name, dialogueData);
                    }
                }
            }
        }

        private float m_elapsedTime;
        private bool isShowing; 
        private DialogueData _data;
        private void Update()
        {
            if (!isShowing && m_task.Count > 0)
            {
                 _data = m_task.Dequeue();
                Display(_data.Dialogue);
                isShowing = true;
            }
            
            
            if (isShowing)
            {
                m_elapsedTime += Time.deltaTime;

                if (m_elapsedTime > _data.DisplayTime / 2f)
                {
                    // 表示時間の半分でしゃべり停止
                    SoundManager.instance.StopPlay("Speak");
                }

                if (m_elapsedTime > _data.DisplayTime)
                {
                    CheckHide();
                    isShowing = false;
                    m_elapsedTime = 0f;
                }
            }
        }

        private void Display(Sprite dialogue)
        {
            image.sprite = dialogue;
            if (!image.enabled)
            {
                if (!systemMessage)
                {
                    dialogueAnim.SetBool("Open", true);
                    SoundManager.instance.DelayPlay("Speak", 0.4f);   // アニメーションに合わせてセリフのサウンドも遅延かける
                }
                
                image.enabled = true;
            }
            else if (isDialogue)
            {
                SoundManager.instance.Play("Speak");   // しゃべるのはセリフの時だけ
            }
        }

        private void CheckHide()
        {
            if (image.sprite == null)
                return;
            
            // 一連のセリフ表示の最後ならimageの表示off
            if (m_task.Count == 0)
            {
                if (!systemMessage)
                 dialogueAnim.SetBool("Open", false);
                
                StartCoroutine(DelayHide());
            }
        }
        
        public void Enqueue(string name)
        {
            DialogueData _data = GetDialogueData(name);
            if (_data == null) return;
            
            if (BattleDialogueCheck(name) == false)
                return;

            if (m_task.Count != 0)   // 他のセリフが表示待機
            {
                if (DamageDialogueCheck(name) == false || CoinDialogueCheck(name) == false)
                    return;
            }
            m_task.Enqueue(_data);
        }

        public void ClearQueue()
        {
            if (isShowing)
            {
                SoundManager.instance.StopPlay("Speak");
                if (!systemMessage)
                    dialogueAnim.SetBool("Open", false);
                
                StartCoroutine(DelayHide());
                isShowing = false;
                m_elapsedTime = 0f;
            }
            
            m_task.Clear();
            OnDialogueSkipped?.Invoke();
            
            Debug.Log("セリフがスキップされました。");
         
        }

        private bool DamageDialogueCheck(string name)
        {
            if (name == "hyo" || name == "wa")
            {
                // 他のセリフ表示中に初ダメージ食らったら入れない
                if (GameManager.instance.Life == 11)
                    return false;
            }

            return true;
        }

        private bool CoinDialogueCheck(string name)
        {
            if (name == "FindCoinGimmick"　|| name == "SubPartGimmick")
                return false;
            
            return true;
        }
        
        private bool BattleDialogueCheck(string name)
        {
            if (name == "Battle01"　|| name == "Battle02")
            {
                if (GameManager.instance.CurrentState != GameState.EnemyBattle)
                {
                    return false;
                }
            }
            return true;
        }

        private DialogueData GetDialogueData(string name)
        {
            if (m_dataDictionary.TryGetValue(name, out DialogueData data))
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
            yield return new WaitForSeconds(delayTime);
            Enqueue(name);
            yield return null;
        }

        private IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(0.8f);
            image.enabled = false;
        }
    }
}