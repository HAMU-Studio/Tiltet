using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine;

public class Rescue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        canRescueAct = false;
        delayForce = Vector3.zero;
        isRescue = false;
        time = 0f;
    }

    private float time;

    private static float finish = 1f;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isRescue)
        {
            time += Time.fixedDeltaTime;
            rescuedPlayer.GetComponent<Rigidbody>().AddForce(rescuedPlayer.GetComponent<Rigidbody>().mass * delayForce / Time.fixedDeltaTime, ForceMode.Force);
            if (time >= finish)
            {
             
                time = 0f;
                isRescue = false;
            }
        }
    }

    private GameObject rescuePlayer;
    private bool canRescueAct;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canRescueAct = true;
            rescuePlayer = other.gameObject;
            Debug.Log("canRescueAct = " + canRescueAct);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canRescueAct = false;
            Debug.Log("canRescueAct = " + canRescueAct);
        }
    }

    private GameObject rescuedPlayer;
    public void SetRescuedPlayer(GameObject Player)
    {
        rescuedPlayer = Player;
    }
    
    [SerializeField] private Vector3 scalePower;
    private bool isRescue;
    private Vector3 delayForce;
    public void RescueAction()
    {
        if (canRescueAct == false)
        {
            return;
        }

        isRescue = true;
        Vector3 direction = (rescuePlayer.transform.position - rescuedPlayer.transform.position).normalized;

        //Yだけ強くして一本釣りっぽさ表現
        direction = Vector3.Scale(direction, new Vector3(scalePower.x, scalePower.y, scalePower.z));

        delayForce.x = direction.x;
        delayForce.z = direction.z;
         
        rescuedPlayer.GetComponent<PlayerController>().ChangePlayerState(false);
        rescuedPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rescuedPlayer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        rescuedPlayer.GetComponent<Rigidbody>().AddForce(new Vector3(0, direction.y, 0), ForceMode.Impulse);

        canRescueAct = false;
    }
}
