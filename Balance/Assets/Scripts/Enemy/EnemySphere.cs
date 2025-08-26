using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.ParticleSystem;

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
        ReSet,
        Go,
        Dead
    }

    [Header("この敵がでるフィールド")]
    [SerializeField] private EnemyType enemyType;
    [Header("動くスピード")]
    [SerializeField] private float m_moveSpeed;
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
    private float explosionTime = 0.0f;

    private bool life;
    private bool escape;
    private bool ableExplosion;
    private bool stop;

    Vector3 m_nowPos = new Vector3();
    Vector3 m_direction = new Vector3();
    Vector3 m_stageCenter = new Vector3(0.0f, 2.0f, 0.0f);

    EnemyManager enemymanager;
    PlayerManager[] getPlayerManagers;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    private void Set()
    {
        escape = false;
        stop = false;
        ableExplosion = false;

        //敵の状態
        enemyState = EnemyState.Ready;

        //最初にこれでplayer類初期化(消すな)
        m_players = GameObject.FindGameObjectsWithTag("Player");
        getPlayerManagers = new PlayerManager[m_players.Length];
        for (int i = 0; i < m_players.Length; i++)
        {
            getPlayerManagers[i] = m_players[i].GetComponent<PlayerManager>();
        }

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
        //Debug.Log(enemyState);
        //animation
        AnimManager();

        if (!stop)
        {
            if (enemyState != EnemyState.Ready)
            {
                SetTarget();

                m_nowPos = transform.position;

                // targetがnullでないことを確認
                if (m_target != null)
                {
                    //進行方向
                    //方向に大きさはいらないので正規化
                    m_direction = (m_target.transform.position - transform.position).normalized;
                }
            }
        }

        if (escape)
        {
            m_direction = (new Vector3(25.0f, -9.3f, 34.2f) - transform.position).normalized;
        }

        explosionTime += Time.deltaTime;
        if (enemyType == EnemyType.VOLCANO)
        {
            Explosion();
        }
    }


    void FixedUpdate()
    {
        if (!stop)
        {
            //落ちそうになったら端っこで踏ん張る
            Funbari();
            //スピード出しすぎ防止
            Brake();

            if (enemyState == EnemyState.Go)
            {
                enemyRb.AddForce(m_direction * m_moveSpeed);
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

    private void Explosion()
    {
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

    //目標を設定
    //playerが二人なことはエネミーマネージャーで感知済み
    private float time = 0f;
    private float coolTime = 1f;
    private bool targetNum;
    PlayerManager playerManager;
    //playernum =true:0, =false:1 に振り分け
    private void ChangeTarget(bool playernum)
    {
        targetNum = playernum;
        int idx = targetNum ? 0 : 1;
        m_target = m_players[idx];
        playerManager = getPlayerManagers[idx];
    }
    private void SetTarget()
    {
        //プレイヤーが二人いるか（再度確認）
        if (m_players.Length == 2)
        {
            //最初の目標設定
            if (enemyState == EnemyState.Ready)
            {
                //距離を調査
                for (int i = 0; i < m_players.Length; i++)
                {
                    m_distance[i] = Vector3.Distance(m_nowPos, m_players[i].transform.position);
                }

                //どっちのplayerのほうが近いか
                if (m_distance[1] > m_distance[0])
                {
                    ChangeTarget(true);
                }
                else
                {
                    ChangeTarget(false);
                }
                enemyState = EnemyState.Go;
            }
            //追いかけてるとき
            else if(enemyState == EnemyState.Go)
            {
                // Debug.Log(playerManager.rescState);
                //追いかけてた目標が落ちたら
                if(playerManager.rescState == RescueState.Wait)
                {
                    //Debug.Log(playerManager.rescState);
                    //もう一つへ
                    ChangeTarget(!targetNum);
                    enemyState = EnemyState.ReSet;
                    //Debug.Log(enemyState);
                }
                /*if (playerManager.rescState != RescueState.None)
                {
                    Debug.Log(playerManager.rescState);
                }*/
            }
            else if(enemyState == EnemyState.ReSet)
            {
                Debug.Log(enemyState);
                time += Time.deltaTime;

                if (time > coolTime)
                {
                    enemyState = EnemyState.Go;
                    time = 0f;
                }
            }
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

    private float resistForce = 5f;
    private void Funbari()
    {
        //ステージの傾きに沿ったベクトルを取得
        float tilt = enemymanager.GetStageTilt();
        Vector3 downOnBoard = enemymanager.GetDownOnBoard();

        //敵がどれくらい端にいるか
        Vector3 localPos = transform.position - m_stageCenter;
        float distance = Vector3.Distance(this.transform.position, m_stageCenter);

        // 球が下り側にいるか判定
        float dot = Vector3.Dot(localPos.normalized, downOnBoard);

        //ターゲットをリセットした時のクールタイム
        if (enemyState == EnemyState.ReSet)
        {
            Vector3 resistDir = -localPos.normalized;
            resistDir += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, UnityEngine.Random.Range(-0.3f, 0.3f));

            enemyRb.AddForce(resistDir * resistForce, ForceMode.Force);
            //Debug.Log(enemyState);
        }
        //探査機が10度以上傾いてて(20度未満)
        //敵が中心から10離れてて
        //敵が下り側にいるか
        else if (tilt >= 10f && 20f > tilt && distance >= 13f && dot > 0)
        {
            Vector3 resistDir = -localPos.normalized;
            resistDir += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, UnityEngine.Random.Range(-0.3f, 0.3f));

            enemyRb.AddForce(resistDir * resistForce, ForceMode.Force);
            //Debug.Log("踏ん張り！");
        }
        else if (tilt >= 20f && distance >= 13f && dot > 0)
        {
            //20度以上いったら踏ん張らない
        }
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

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("atatta!");

        }

        if (collision.gameObject.CompareTag("Terrain"))
        {
            if (!life)
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
}