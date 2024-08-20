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

    private Rigidbody m_RB;
    private void FixedUpdate()
    {
        if (isRescue)
        {
           // time += Time.fixedDeltaTime;
            m_RB = rescuedPlayer.GetComponent<Rigidbody>();
           
            m_RB.AddForce(m_RB.mass * delayForce / Time.fixedDeltaTime, ForceMode.Force);

            CheckRescueDone();
            // rescuedPlayer.GetComponent<Rigidbody>().AddForce(rescuedPlayer.GetComponent<Rigidbody>().mass * delayForce / Time.fixedDeltaTime, ForceMode.Force);
            /*if (time >= finish)
            {
                time = 0f;
                isRescue = false;
            }*/
            //この辺あやしい
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
        }
    }

    private GameObject rescuedPlayer;
    public void SetRescuedPlayer(GameObject Player)
    {
        rescuedPlayer = Player;
    }
    
    [SerializeField] private Vector3 scalePower;
    
    [Header("救出アクションで飛ばす先(反対側の板)")]
    [SerializeField] private GameObject landingPoint;
    private bool isRescue;
    private Vector3 delayForce;
    public void RescueAction()
    {
        if (canRescueAct == false)
        {
            return;
        }

        isRescue = true;
        Vector3 direction = (landingPoint.transform.position - rescuedPlayer.transform.position).normalized;

        //Yだけ強くして一本釣りっぽさ表現
        direction = Vector3.Scale(direction, new Vector3(scalePower.x, scalePower.y, scalePower.z));

        delayForce.x = direction.x;
        delayForce.z = direction.z;
       
        rescuedPlayer.GetComponent<PlayerController>().ChangePlayerState(false);

        SetRBVelocity();
        
        //最初にY方向だけ力加えた後少しずつX、Z方向にも加えてゆく
        m_RB.AddForce(new Vector3(0, direction.y, 0), ForceMode.Impulse);

        this.GetComponent<Renderer>().enabled = false;

        canRescueAct = false;
    }

    private void CheckRescueDone()
    {
        Vector3 playerPos = rescuedPlayer.transform.position;
        Vector3 targetPoint = landingPoint.transform.position;
        
        Debug.Log("playerPos = " + playerPos);
        Debug.Log("targetPos = " + targetPoint);
        
        if (Mathf.Approximately(playerPos.x, targetPoint.x) &&
            Mathf.Approximately(playerPos.z, targetPoint.z))
        {
            Debug.Log("Stop!");
            isRescue = false;
            SetRBVelocity();
        }
    }

    private void SetRBVelocity()
    {
        m_RB = rescuedPlayer.GetComponent<Rigidbody>();
        m_RB.velocity = Vector3.zero;
        m_RB.angularVelocity = Vector3.zero;
    }
}
