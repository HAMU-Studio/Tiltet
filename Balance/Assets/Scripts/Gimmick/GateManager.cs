using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    [Header("ゲートの数")]
    [SerializeField] private int clearNum;
    [Header("出てくるパーツ")]
    [SerializeField] private GameObject parts;

    public int GateNumber { get; set; }
    private int beforeGateNumber;

    // Start is called before the first frame update
    void Start()
    {
        GateNumber = 1;
        clearNum++;
        parts.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GateNumber);
        if (parts == null)
            return;

        if (GateNumber != beforeGateNumber)
        {
            if (GateNumber == 3)
            {
                DisplayDialogue.dialogue.Enqueue("Good");
            }

            if (GateNumber == 5)
            {
                DisplayDialogue.dialogue.Enqueue("TwoLeft");
            }
        }
        
        
        
        if (GateNumber == clearNum && parts.activeSelf == false) 
        {
            parts.SetActive(true);
            DisplayDialogue.dialogue.Enqueue("Happy");
            DisplayDialogue.dialogue.Enqueue("ActiveMainPart");
            SoundManager.instance.Play("Arrival");
        }

        beforeGateNumber = GateNumber;
    }
}
