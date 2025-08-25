using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : MonoBehaviour
{
    private enum EnemyType
    {
        GREEN,
        SNOW,
        VOLCANO
    }

    private enum EnemyState
    {
        Ready,
        //Set,
        Go,
        Dead
    }

    [Header("この敵がでるフィールド")]
    [SerializeField] private EnemyType enemyType;
    [Header("動くスピード")]
    [SerializeField] private float m_moveSpeed;
    //[Header("最低速度")]
    //[SerializeField] private float m_minSpeed;
    [Header("踏ん張り始める角度")]
    [SerializeField] private float m_funbariAngle;
    [Header("爆発の範囲")]
    [SerializeField] private GameObject explosionRenge;
    [Header("爆発のエフェクト")]
    [SerializeField] private GameObject explosionEffect;

    //[SerializeField] private GameObject[] home; 

    private EnemyState enemyState;
    private Animator anim;
    private GameObject[] m_players;
    private GameObject m_target;
    private Rigidbody enemyRb;

    private float[] m_distance;
    private float funbariTime = 0.0f;
    private float m_angle = 0.0f;
    private float explosionTime = 0.0f;

    private bool life;
    private bool escape;
    private bool brake;
    private bool funbari;
    private bool ableExplosion;
    private bool stop;

    Vector3 m_nowPos = new Vector3();
    Vector3 m_direction = new Vector3();
    Vector3 m_stageCenter = new Vector3(0.0f, 2.0f, 0.0f);

    EnemyManager enemymanager;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    private void Set()
    {
        escape = false;
        brake = false;
        funbari = false;
        stop = false;
        ableExplosion = false;

        //敵の状態
        enemyState = EnemyState.Ready;

        //最初にこれでplayer初期化(消すな)
        m_players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        // どうせプレイヤーは二人なので二個で初期化
        m_distance = new float[2];

        enemyRb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        GameObject enemyManager = GameObject.Find("EnemyManager");
        enemymanager = enemyManager.GetComponent<EnemyManager>();

        //inspectorで設定した値を取得(dontuse)
        movespeed = m_moveSpeed;
    }

    //
    //
    // Update is called once per frame
    void Update()
    {
        //animation
        AnimManager();

        if (enemyState == EnemyState.Go)
        {
            if (!stop)
            {
                m_nowPos = transform.position;

                //最大速度の調整
                //SetMoveSpeed();
                //スピード出しすぎ防止
                Brake();


                // targetがnullでないことを確認
                if (m_target != null)
                {
                    //進行方向
                    //方向に大きさはいらないので正規化
                    m_direction = (m_target.transform.position - transform.position).normalized;


                }
                /*else if (m_target == null)
                {
                    SetTarget();
                }*/

                //一秒端っこで踏ん張る
                if (funbari)
                {
                    funbariTime += Time.deltaTime;

                    if (funbariTime > 1.0f)
                    {
                        funbariTime = 0.0f;
                        funbari = false;
                    }
                }
                else
                {
                    Funbari();
                }
            }
        }

        else
        {
            if (escape)
            {
                m_direction = (new Vector3(25.0f, -9.3f, 34.2f) - transform.position).normalized;
            }
        }

        explosionTime += Time.deltaTime;

        if (enemyType == EnemyType.VOLCANO)
        {
            //Explosion();

            if (explosionTime > 6.0f && stop == false)
            {
                enemyRb.constraints = RigidbodyConstraints.FreezeAll;
                stop = true;
                anim.SetBool("explosion", true);
            }

            if (explosionTime > 7.0f && explosionEffect.activeSelf == false)
            {
                explosionEffect.SetActive(true);    // 爆発エフェクト再生
                gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;  //敵を視覚的にオフ
                SoundManager.instance.Play("Explosion");
            }

            if (explosionTime > 7.86f)
            {
                enemymanager.DestroyEnemy();
                Destroy(this.gameObject);
            }
        }
    }

    private float funbariPower = 3.0f;
    void FixedUpdate()
    {
        if (enemyState == EnemyState.Go)
        {
            if (funbari)
            {
                //enemyRb.AddForce((m_stageCenter - m_nowPos).normalized * funbariPower);
                //Debug.Log("funbari");
            }
            else
            {
                if (!stop)
                {
                    enemyRb.AddForce(m_direction * m_moveSpeed);
                }
            }
        }
    }

    private void AnimManager()
    {
        switch (enemyState)
        {
            case EnemyState.Ready:
                break;
            //case EnemyState.Set:
            //break;
            case EnemyState.Go:
                anim.SetBool("Arrived", true);
                break;
            case EnemyState.Dead:
                break;

        }
    }

    //目標を設定
    //playerが二人なことはエネミーマネージャーで感知済み
    private void SetTarget()
    {
        //距離を調査
        for (int i = 0; i < m_players.Length; i++)
        {
            m_distance[i] = Vector3.Distance(m_nowPos, m_players[i].transform.position);
        }

        //どっちのplayerのほうが近いか
        m_target = m_players[0];
        if (m_distance[1] < m_distance[0])
        {
            m_target = m_players[1];
        }

        enemyState = EnemyState.Go;
    }

    private void SetMoveSpeed()
    {
        //プレイヤーが端にいると追い打ちをかけてくる
        //distance=0~2
        float distance = Vector3.Distance(m_stageCenter, m_target.transform.position);
        //Debug.Log(distance);

        if (distance <= 1)
        {
            m_maxSpeed = 0.2f;
        }
        else if (1 < distance)
        {
            m_maxSpeed = 0.4f;
        }
    }

    Vector3 m_prePosition = new Vector3();
    private float m_maxSpeed = 0.15f;
    private float movespeed;
    //スピードがmaxSpeedを超えたらブレーキがかかる
    private void Brake()
    {
        Vector3 enemyDirection = (m_nowPos - m_prePosition).normalized;
        float speed = Vector3.Distance(m_nowPos, m_prePosition);

        //Debug.Log("スピード"+speed);
        if (speed >= m_maxSpeed)
        {
            m_moveSpeed = 0f;
            enemyRb.AddForce(-enemyDirection * (movespeed * 2f));
        }
        else
        {
            //ブレーキの無効か
            if (m_moveSpeed == 0f)
            {
                m_moveSpeed = movespeed;
            }
        }
            m_prePosition = m_nowPos;
    }

    /*private void Funbari()
    {
        float distance = Vector3.Distance(this.transform.position, m_stageCenter);
        //Debug.Log(distance);
        Vector3 parallel = new Vector3(m_nowPos.x, m_stageCenter.y, m_nowPos.z);

        //
        //m_angle = Vector3.Angle(Vector3.forward, (m_nowPos - m_stageCenter));
        m_angle = Vector3.Angle((parallel - m_stageCenter), (m_nowPos - m_stageCenter));
        //Debug.Log(m_angle);
        //端っこがdistance=15くらい
        if (distance > 13.0f)
        {
            if (m_angle >= m_funbariAngle)
            {
                Debug.Log("funbari");
                funbari = true;
            }
        }
    }*/

    private void Funbari()
    {
        //ステージの傾きに沿ったベクトルを取得
        Vector3 downOnBoard = enemymanager.GetStageTilt();
        Vector3 localPos = transform.position - m_stageCenter;
        float distance = Vector3.Distance(this.transform.position, m_stageCenter);

        // 球が下り側にいるか判定
        float dot = Vector3.Dot(localPos.normalized, downOnBoard);
    }

    private void Knockback()
    {

    }

    //着地した時に近くにいたプレイヤーを追いかける
    private void OnCollisionEnter(Collision collision)
    {
        //自機に着いたら
        if (collision.gameObject.CompareTag("Ground"))
        {
            //目標を設定しているか
            if (enemyState == EnemyState.Ready)
            {
                //目標を設定
                SetTarget();
                //Debug.Log("tuita!");

                //今の場所を記録
                m_prePosition = transform.position;
            }
        }

        if(collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("atatta!");

        }

        if(collision.gameObject.CompareTag("Terrain"))
        {
            if(!life)
            {
                escape = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            life = false;
        }
    }

    ////DontUse////
    /*private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            arrived = true;
            anim.SetBool("Arrived", true);
        }
    }*/
    /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }*/
    /*private void Explosion()
    {
        explosionTime += Time.deltaTime;
        if (time >= 2.0f)
        {
            enemyRb.constraints = RigidbodyConstraints.FreezeAll;
            stop = true;
            anim.SetBool("explosion", true);
        }
        if (time >= 3.0f)
        {
            explosionRenge.SetActive(true);
            explosionEffect.SetActive(true);    
        }
        if (time >= 3.5f)
        {
            Destroy(gameObject);
        }
    }*/
    /*private void DebugSetTarget()
    {
        m_target = m_players[0];
    }*/
}