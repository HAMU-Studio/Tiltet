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

  
    void Start()
    {
        time = 0f;
        //waitTime = Random.Range(0, 3);
        waitTime = 5;
        anim = gameObject.GetComponent<Animator>();
        reflected = false;
    }

    private ThrowawayMethod m_method = new ThrowawayMethod();
    void Update()
    {
        if (time >= waitTime)
        {
            Destroy(gameObject);
            m_method.RunOnce(GameManager.instance.PlayerUnLock); 
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
            Destroy(gameObject);
        }
    }
}
