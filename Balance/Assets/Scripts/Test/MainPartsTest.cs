using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPartsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        getPart = new ThrowawayMethod();
        if (gameObject.activeSelf)
        {
            once = true;
        }
    }

    private bool once;
    void Update()
    {
        if (gameObject.activeSelf && once == false)
        {
            DisplayDialogue.dialogue.EnqueueDialogue("ActiveMainPart");
            once = true;
        }
    }

    private ThrowawayMethod getPart;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            getPart.RunOnce(GameManager.instance.AddMainPartsNum); 
            Destroy(this.gameObject);
            SoundManager.instance.Play("GetMainPart");
        }
    }
}
