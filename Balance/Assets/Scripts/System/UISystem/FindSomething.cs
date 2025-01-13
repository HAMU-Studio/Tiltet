using Dialogue;
using System.Security.Cryptography;
using UnityEngine;

namespace System
{
    public class FindSomething : MonoBehaviour
    {
        [SerializeField] private string[] dialogueNames;
        [SerializeField] private bool aircraft;

        private void Start()
        {
            isCalled = false;
            GetComponent<MeshRenderer>().enabled = false;
            
            // すでにセリフを表示済みなら消す
            if (FieldItemsManager.instance.GetIsAcquired(gameObject.name) == true)
            {
                Destroy(gameObject);
            }
        }

        private bool isCalled = false;
        private void OnTriggerEnter(Collider other)
        {
            if (isCalled) return;
            
            if (aircraft == true)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    GetAndDisplayDialogue();
                }
            }
            else
            {
                if (other.gameObject.CompareTag("Ground"))
                {
                    GetAndDisplayDialogue();
                }
            }
        }

        private void GetAndDisplayDialogue()
        {
            isCalled = true;
            foreach (var name in dialogueNames)
            {
                DisplayDialogue.dialogue.Enqueue(name);
            }
                
            if (FieldItemsManager.instance.GetIsAcquired(gameObject.name) == false)
            {
                FieldItemsManager.instance.GetItem(gameObject.name);
            }
                
            Destroy(gameObject);
        }
    }
}