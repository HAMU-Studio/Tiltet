using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

public class EncountAnimation : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    private Animator anim;
    private float time;
    private int waitTime;
    private bool reflected;

    // Start is called before the first frame update
    void Start()
    {
        time = 0f;
        //waitTime = Random.Range(0, 3);
        waitTime = 5;
        anim = gameObject.GetComponent<Animator>();
        reflected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (time >= waitTime)
        {
          //  Destroy(gameObject);
            //anim.SetBool("AbleMove", true);
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    private void OnBecameVisible()
    {
        anim.SetBool("AbleMove", true);
        reflected = true;
    }

    private void OnBecameInvisible()
    {
        if(reflected)
        {
          //  Destroy(gameObject);
        }
    }
}
