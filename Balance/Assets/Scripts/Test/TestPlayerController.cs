using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.P))
        {
            transform.position += new Vector3(0.0f, 0.0f, 0.05f);
        }

        if(Input.GetKey(KeyCode.Equals))
        {
            transform.position += new Vector3(0.0f, 0.0f, -0.05f);
        }

        if (Input.GetKey(KeyCode.L))
        {
            transform.position += new Vector3(-0.05f, 0.0f, 0.0f);
        }

        if (Input.GetKey(KeyCode.Semicolon))
        {
            transform.position += new Vector3(0.05f, 0.0f, 0.0f);
        }
    }
}
