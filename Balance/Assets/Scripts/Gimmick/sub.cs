using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sub : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            GameManager.instance.AddSubPartsNum();
            SoundManager.instance.Play("GetSubPart");
            Destroy(gameObject);
        }
    }
}
