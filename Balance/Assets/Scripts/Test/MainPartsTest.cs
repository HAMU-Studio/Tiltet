using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPartsTest : MonoBehaviour
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
            GameManager.instance.AddMainPartsNum();
            Destroy(this.gameObject);
            SoundManager.instance.Play("GetMainPart");
        }
    }
}
