using Dialogue;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Test
{
    public class TestDialogue : MonoBehaviour
    {
        [SerializeField] private DialogueDatas dialogueDatas;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                foreach (var data in dialogueDatas.data)
                {
                    DisplayDialogue.instance.EnqueueDialogue(data.Name);
                }
            }
        }
    }
}