using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
//using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;


public class EnemyEllipse : MonoBehaviour
{
    //[SerializeField] private GameObject front;
    private GameObject[] players;
    private Rigidbody enemyRb;
    private float time;
    private float moveSpeed = 3.0f;
    private bool ableAssault;
    private bool Assault;

    Vector3 _Direction = new Vector3();
    Vector3 _prePosition = new Vector3();// 前の位置
    Vector3 _position = new Vector3();// 現在の位置

    private float[] distance;
    private GameObject target;

    Vector3 Direction = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ableAssault)
        {
            CheckPlayer();
        }
        else
        {
            time += Time.deltaTime;

            if (time <= 0.5f)
            {
                Debug.Log("hit");
                enemyRb.velocity = Vector3.zero;
                Assault = true;
                ableAssault = false;
            }
        }

        if(Assault)
        {
            enemyRb.AddForce(_Direction * moveSpeed, ForceMode.Impulse);
            Assault = false;
        }

        Debug.DrawRay(transform.position, _Direction * 100.0f, Color.red);
    }

    void FixedUpdate()
    {
        CheckDirection();
    }

    private void Set()
    {
        //playerのタグがついているオブジェクトを代入
        players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        if (players.Length > 0)
        {
            distance = new float[players.Length];
        }

        time = 0;
        ableAssault = false;

        _position = Vector3.zero;
        _prePosition = transform.position;
        _Direction = Vector3.forward;
        enemyRb = GetComponent<Rigidbody>();
    }

    private void CheckDirection()
    {
        // 今の位置を代入しなおす
        _position = this.transform.position;

        if(_position == _prePosition)
        {
            return;
        }

        //進行方向（移動量ベクトル）
        _Direction = _position - _prePosition;

        //前の位置を代入
        _prePosition = _position;
    }

    private void CheckPlayer()
    {
        Ray ray = new Ray(transform.position, _Direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                ableAssault = true;
            }
        }
    }

    private void SearchPlayer()
    {
        //playerが1人もいない時
        if (players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            return;
        }
        //playerが1人の時
        if (players.Length == 1)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            target = players[0];
            return;
        }

        //距離を調査
        for (int i = 0; i < players.Length; i++)
        {
            distance[i] = Vector3.Distance(this.transform.position, players[i].transform.position);
        }

        //どっちのplayerのほうが近いか
        target = players[0];
        if (distance[1] < distance[0])
        {
            target = players[1];
        }
    }
}

