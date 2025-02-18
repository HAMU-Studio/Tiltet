using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Dialogue
{
    [Serializable]
    public class DialogueData
    {
        public string Name;        // 名前
        public Sprite Dialogue;    // セリフ(画像)
        public float  DisplayTime; // 表示時間
    }
    
    // 右クリックから作成できるように
    [CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData")]
    public class DialogueDatas : ScriptableObject
    {
        public DialogueData[] data; // ScriptableObjectの配列
    }
}