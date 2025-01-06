using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    [Header("置くゲートを通る順番に入れていく")]
    [SerializeField] private GameObject[] gates;

    public int GateNumber { get; set; }

    private int nuwNumber;


    // Start is called before the first frame update
    void Start()
    {
        nuwNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
