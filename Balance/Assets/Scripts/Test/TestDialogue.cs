using Dialogue;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Test
{
    public class TestDialogue : MonoBehaviour
    {
        [SerializeField] private DisplayDialogue _displayDialogue;
        [SerializeField] private DialogueDatas[] dialogueDatas;
        private void Update()
        {

            for (int i = 0; i < dialogueDatas.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    foreach (var data in dialogueDatas[i].data)
                    {
                        _displayDialogue.Enqueue(data.Name);
                    }
                }
            }
        }
    }
}