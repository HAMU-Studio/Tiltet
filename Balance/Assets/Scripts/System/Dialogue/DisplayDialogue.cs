using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DisplayDialogue : MonoBehaviour
    {
        public static DisplayDialogue instance;
        
        Queue<DialogueData> _task = new Queue<DialogueData>();

        [SerializeField] private DialogueDatas[] dialogueDatas;
        private Dictionary<string, DialogueData> dataDictionary = new();

        [SerializeField] private Image image;
        
        [Header("次のセリフ表示までのディレイ")]
        [SerializeField] private float delay;

        private void Awake()
        {
            instance = this;
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

                if (elapsedTime > _data.DisplayTime)
                {
                    Hidden();
                }

                if (elapsedTime > _data.DisplayTime + delay)
                {
                    isShowing = false;
                    elapsedTime = 0f;
                }
            }
        }

        private void Display(Sprite dialogue)
        {
            image.sprite = dialogue;
            image.enabled = true;
        }

        private void Hidden()
        {
            if (image.sprite == null)
                return;
            
            image.enabled = false;
            image.sprite = null;
        }
        
        public void EnqueueDialogue(string name)
        {
            DialogueData _data = GetDialogueData(name);
            if (_data == null) return;
            
            _task.Enqueue(_data);
        }

        private DialogueData GetDialogueData(string name)
        {
            if (instance.dataDictionary.TryGetValue(name, out DialogueData data))
            {
                return data;
            }
            else
            {
                Debug.Log("Failed to get data");
                return null;
            }
        }
    }
}