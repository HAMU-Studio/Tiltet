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
    private enum EnemyType
    {
        GREEN,
        SNOW,
        VOLCANO
    }

    [Header("この敵がでるフィールド")]
    [SerializeField] private EnemyType enemyType;

    [Header("突撃する強さ")]
    [SerializeField] private float moveSpeed = 50.0f;

    private GameObject[] players;
    private Rigidbody enemyRb;
    private float time;
    private bool m_arrived;
    private bool ableAssault;
    private bool assault;

    Vector3 _Direction = new Vector3();
    Vector3 _prePosition = new Vector3();// 前の位置
    Vector3 _position = new Vector3();// 現在の位置

    private float[] distance;
    private GameObject target;

    Vector3 Direction = new Vector3();

    private bool ablemove;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_arrived)
        {
            if (!ablemove)
            {
                time += Time.deltaTime;

                if (time <= 0.5f)
                {
                    enemyRb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    time = 0.0f;
                    enemyRb.constraints = RigidbodyConstraints.None;
                    ablemove = true;
                }
            }

            if (ablemove)
            {
                if (!ableAssault)
                {
                    time += Time.deltaTime;

                    if (time >= 2.0f)
                    {
                        time = 0.0f;
                        ableAssault = true;
                    }
                }
                else
                {
                    CheckPlayer();
                }

                if (assault)
                {
                    enemyRb.AddForce(_Direction * moveSpeed, ForceMode.Impulse);
                    ableAssault = false;
                    assault = false;
                }
            }
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
        m_arrived= false;
        assault = false;
        ableAssault = true;

        _position = Vector3.zero;
        _prePosition = transform.position;
        _Direction = Vector3.forward;
        enemyRb = GetComponent<Rigidbody>();

        ablemove = false;
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
        //Ray ray = new Ray(transform.position, _Direction);
        //RaycastHit hit;
        if (Physics.CapsuleCast(
            transform.position + new Vector3(1.5f,0.0f,0.0f),
            transform.position + new Vector3(-1.5f,0.0f,0.0f),
            4.0f,
            _Direction,
            out var hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                assault = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            m_arrived = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}

