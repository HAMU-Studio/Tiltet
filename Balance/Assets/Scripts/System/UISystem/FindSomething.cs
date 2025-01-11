using Dialogue;
using System.Security.Cryptography;
using UnityEngine;

namespace System
{
    public class FindSomething : MonoBehaviour
    {
        [SerializeField] private string[] dialogueNames;

        private void Start()
        {
            isCalled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }

        private bool isCalled = false;
        private void OnTriggerEnter(Collider other)
        {
            if (isCalled) return;
            
            if (other.gameObject.CompareTag("Ground"))
            {
                isCalled = true;
                foreach (var name in dialogueNames)
                {
                    DisplayDialogue.dialogue.EnqueueDialogue(name);
                }
                Destroy(gameObject);
            }
        }
    }
}