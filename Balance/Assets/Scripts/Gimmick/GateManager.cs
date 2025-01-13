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

        if(GateNumber==clearNum)
        {
            parts.SetActive(true);
            SoundManager.instance.Play("Arrival");
        }

    }
}
