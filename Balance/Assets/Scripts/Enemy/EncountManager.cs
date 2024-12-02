using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncountManager : MonoBehaviour
{
    public bool isEncount {  get; set; }

    // Start is called before the first frame update
    void Start()
    {
        isEncount = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isEncount);
    }
}
