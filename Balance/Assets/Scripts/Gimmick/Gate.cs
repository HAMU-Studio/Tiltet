using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("このゲートの通る順番(1から)")]
    [SerializeField] private int gateNumber;

    private int nowNumber;

    GateManager gatemanager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gateManager = GameObject.Find("GateManager");
        gatemanager = gateManager.GetComponent<GateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(nowNumber);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            nowNumber = gatemanager.GateNumber;
            //Debug.Log("ぶつかった");

            if (nowNumber == gateNumber)
            {
                gatemanager.GateNumber = gateNumber + 1;
                SoundManager.instance.Play("Connected");
                //Debug.Log("正解");
            }
            else
            {
               // gatemanager.GateNumber = 1;
                //Debug.Log("残念");
            }
        }
    }
}
