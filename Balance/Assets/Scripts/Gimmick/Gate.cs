using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("このゲートの通る順番")]
    [SerializeField] private int gateNumber;

    private int nextNumber;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gateManager = GameObject.Find("GateManager");
        GateManager gatemanager;
        gatemanager = gateManager.GetComponent<GateManager>();
        nextNumber = gatemanager.GateNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckNumber()
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {

        }
    }
}
