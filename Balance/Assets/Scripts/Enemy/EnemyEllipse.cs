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

    private enum EnemyState
    {
        GetOn,
        Arrive,
        Attack,
        Dead
    }

    [Header("この敵がでるフィールド")]
    [SerializeField] private EnemyType enemyType;
    [Header("突撃する強さ")]
    [SerializeField] private float moveSpeed = 50.0f;
    [Header("ray飛ばす方向ガイド")]
    [SerializeField] private GameObject guide;

    Vector3 m_stageCenter = new Vector3(0.0f, 2.0f, 0.0f);
    private EnemyState enemyState;

    private GameObject[] players;
    private Rigidbody enemyRb;
    private float _speed;
    private float time;
    private float m_distanceFromCenter;
    private bool ableAttack;

    Vector3 _Direction = new Vector3();
    Vector3 _prePosition = new Vector3();// 前の位置
    private float[] distance;
    private GameObject target;

    EnemyManager enemymanager;

    private bool ablemove;

    // Start is called before the first frame update
    void Start()
    {
        Set();
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
        ableAttack = true;

        _prePosition = transform.position;
        _Direction = Vector3.forward;
        enemyRb = GetComponent<Rigidbody>();

        ablemove = false;

        enemyState = EnemyState.GetOn;

        GameObject enemyManager = GameObject.Find("EnemyManager");
        enemymanager = enemyManager.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.Arrive)
        {
            m_distanceFromCenter = Vector3.Distance(this.transform.position, m_stageCenter);

            CheckDirection();
            Die();

            if (!ablemove)
            {
                //止まったらok
                if (_speed < 0.001f)
                {
                    enemyRb.constraints = RigidbodyConstraints.None;
                    ablemove = true;
                }
                else
                {
                    enemyRb.constraints = RigidbodyConstraints.FreezeAll;
                }
            }
            else
            {
                if (enemyState == EnemyState.Attack)
                {
                    time += Time.deltaTime;

                    if (time >= 2.0f)
                    {
                        time = 0.0f;
                        enemyState = EnemyState.Arrive;
                    }
                }
                else
                {
                    CheckPlayer();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (enemyState == EnemyState.Attack)
        {
            if (flontAttack)
            {
                enemyRb.AddForce(_Direction * moveSpeed, ForceMode.Impulse);
                //Debug.Log("attack");
                flontAttack = false;
                backAttack = false;
            }
            else
            {
                if(backAttack)
                {
                    enemyRb.AddForce(-_Direction * moveSpeed, ForceMode.Impulse);
                    //Debug.Log("attack");
                    flontAttack = false;
                    backAttack = false;
                }
            }
        }
    }

    private void CheckDirection()
    {
        Vector3 nowposition = this.transform.position;
        Vector3 downOnBoard = enemymanager.GetDownOnBoard();
        float angle = Vector3.Angle(downOnBoard, transform.right);
        float speed = Vector3.Distance(transform.position,_prePosition);
        //Debug.Log(angle);

        //転がり始めたら(ステージに沿ったベクトルと敵の右ベクトルが作る角度)
        if (angle >= 20f && angle <= 160f)
        {
            //進行方向（移動量ベクトル）
            //Vector3 direction = (nowposition - _prePosition).normalized;
            //enemyRb.constraints = RigidbodyConstraints.None;
            _Direction = downOnBoard;

        }
        else
        {
            //敵が横に傾いたときは止まる？
            //enemyRb.constraints = RigidbodyConstraints.FreezeAll;
            _Direction = Vector3.Cross(transform.right, Vector3.down).normalized;
        }

        //前の位置を代入
        _prePosition = nowposition;
        _speed = speed;
    }

    private bool flontAttack = false;
    private bool backAttack = false;
    float rayLength = 5f;
    private void CheckPlayer()
    {
        Vector3 launchsiteR = new Vector3(transform.position.x + 2f, transform.position.y, transform.position.z);
        Vector3 launchsiteL = new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z);

        //デバッグ用
        Debug.DrawRay(launchsiteR, _Direction * rayLength, Color.red);
        Debug.DrawRay(launchsiteR, -_Direction * rayLength, Color.red);
        Debug.DrawRay(launchsiteL, _Direction * rayLength, Color.red);
        Debug.DrawRay(launchsiteL, -_Direction * rayLength, Color.red);

        //進行方向右
        if (Physics.Raycast(launchsiteR, _Direction, out RaycastHit hitFR, rayLength))
        {
            if (hitFR.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("前右Rayが当たった: " + hitFR.collider.name);
                flontAttack = true;
            }
        }
        //進行方向左
        if (Physics.Raycast(launchsiteL, _Direction, out RaycastHit hitFL, rayLength))
        {
            if (hitFL.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("前左Rayが当たった: " + hitFR.collider.name);
                flontAttack = true;
            }
        }
        //後ろ右
        if (Physics.Raycast(launchsiteR, -_Direction, out RaycastHit hitBR, rayLength))
        {
            if (hitBR.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("後ろ右Rayが当たった: " + hitFR.collider.name);
                backAttack = true;
            }
        }
        //後ろ左
        if (Physics.Raycast(launchsiteL, -_Direction, out RaycastHit hitBL, rayLength))
        {
            if (hitBL.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("後ろ左Rayが当たった: " + hitFR.collider.name);
                backAttack = true;
            }
        }

        if (flontAttack || backAttack)
        {
            Debug.Log("attack");
            enemyState = EnemyState.Attack;
        }
    }
    private void Die()
    {
        //探索気よりも外に出た、下に行ったら
        if (m_distanceFromCenter >= 17f || transform.position.y < -2f)
        {
            enemymanager.DestroyEllipse();
            enemyState = EnemyState.Dead;
        }
    }

    //探査機に乗ったら攻撃開始
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("到着");
            _prePosition = transform.position;
            enemyState = EnemyState.Arrive;
        }
    }


    //dontuse//

    /*private void SetGuidePosition()
    {
        Vector3 nowpos = transform.position;
        float posZ = nowpos.z + 2f;

        guides[0].transform.position = new Vector3(nowpos.x, nowpos.y, posZ);
        guides[1].transform.position = new Vector3(nowpos.x, nowpos.y, -posZ);

        //guides[0].transform.position = transform.position;
        //guides[1].transform.position = transform.position + Vector3.left * 1.5f;
    }*/

    //Ray ray = new Ray(transform.position, _Direction);
    //RaycastHit hit;
    /*if (Physics.CapsuleCast(
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
    }*/
    //ガイド生成
    /*guides = new GameObject[2];
    for (int i = 0; i < 2; i++)
    {
        //Debug.Log("guideSet");
        guides[i] = Instantiate(guide, transform.position, transform.localRotation);　
    }*/

    /*Vector3 myRotation = transform.localEulerAngles;
    transform.rotation = Quaternion.Euler(0.0f, myRotation.y, myRotation.z);
    enemyRb.constraints = RigidbodyConstraints.FreezeRotationX;*/
    /*if(floating)
           {
               transform.position += new Vector3(0f, 1.0f, 0f);
               floating = false;
           }*/
    //Vector3 myRotation = transform.localEulerAngles;
}

