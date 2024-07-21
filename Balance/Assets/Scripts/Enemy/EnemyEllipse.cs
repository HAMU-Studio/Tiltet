﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
//using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class EnemyEllipse : MonoBehaviour
{
    //[SerializeField] private GameObject front;
    private GameObject[] players;
    private Rigidbody enemyRb;
    private float moveSpeed;

    Vector3 Direction = new Vector3();
    Vector3 _prePosition = new Vector3();//前の位置
    Vector3 _position = new Vector3();//現在の位置

    // Start is called before the first frame update
    void Start()
    {
        _position = Vector3.zero;
        _prePosition = transform.position;
        Direction = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (Direction == Vector3.zero)
        {
            return;
        }

        CheckPlayer();
        Debug.DrawRay(transform.position, Direction * 1000.0f, Color.red);
    }

    void FixedUpdate()
    {
        CheckDirection();
    }

    void CheckDirection()
    {
        // 今の位置を代入しなおす
        _position = this.transform.position;

        //進行方向（移動量ベクトル）
        Direction = _position - _prePosition;

        //前の位置を代入
        _prePosition = _position;
    }

    void CheckPlayer()
    {
        Ray ray = new Ray(transform.position, Direction);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit,10))
        {
            if(hit.collider.CompareTag("Player"))
            {
                enemyRb.AddForce(Direction * moveSpeed);
            }
        }
    }
}
