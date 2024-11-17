using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private int gateNumber;

    private GateManager gatemanager;
    private int nextNumber;

    // Start is called before the first frame update
    void Start()
    {
        //gatemanagerから変数を共有
        this.gatemanager = FindObjectOfType<GateManager>();
        gatemanager.Order = gateNumber;
        //GameObject obj = GameOblect.Find("GateManager");
        //gatemanager = obj.GetComponent<GateManager>();

        gateNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name=="Floor")
        {
            nextNumber = gatemanager.Order + 1;

            if (gateNumber == nextNumber)
            {
                gatemanager.Order = gateNumber;
            }
            else
            {
                gatemanager.Order = 0;
            }
        }
    }
}
