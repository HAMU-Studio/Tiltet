using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        GetOn,
        Ready,
        ReSet,
        Go,
        Stop,
        Dead
    }

    [Header("この敵がでるフィールド")]
    [SerializeField] private EnemyType enemyType;
    //2ぐらいかなー
    [Header("動くスピード")]
    [SerializeField] private float getMoveSpeed;
    [Header("踏ん張り始める角度")]
    [SerializeField] private float m_funbariAngle;
    [Header("爆発の範囲")]
    [SerializeField] private GameObject explosionRenge;
    [Header("爆発のエフェクト")]
    [SerializeField] private GameObject explosionEffect;


    private EnemyState enemyState;
    private Animator anim;
    private GameObject[] m_players;
    private GameObject m_target;
    private Rigidbody enemyRb;

    private float[] m_distance;
    private float explosionTime = 0.0f;
    private float m_distanceFromCenter;
    private float m_moveSpeed;

    private bool escape;
    private bool ableExplosion;

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
        ableExplosion = false;

        //敵の状態
        enemyState = EnemyState.GetOn;

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

        //inspectorで設定した値を取得
        m_moveSpeed = getMoveSpeed;
    }

    //
    //
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyState);
        //animation
        AnimManager();

        m_distanceFromCenter = Vector3.Distance(this.transform.position, m_stageCenter);

        //着陸した後の処理
        if (enemyState != EnemyState.GetOn)
        {
            if (enemyState != EnemyState.Stop)
            {
                SetTarget();
                Die();

                if (enemyState == EnemyState.Go)
                {
                    // targetがnullでないことを確認
                    //進行方向
                    //方向に大きさはいらないので正規化
                    //Debug.Log(enemyState);
                    m_direction = (m_target.transform.position - transform.position).normalized;
                }
            }
            //爆発準備or探査機から出た
            else
            {

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
        //着陸した後の処理   
        if (enemyState != EnemyState.GetOn)
        {
            if (enemyState == EnemyState.Stop) 
            {
                //爆発準備or探査機から出た
            }
            else
            {
                //落ちそうになったら端っこで踏ん張る
                Funbari();
                //スピード出しすぎ防止
                Brake();

                if (enemyState == EnemyState.Go)
                {
                    //Debug.Log(m_moveSpeed);
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

    private void Explosion()
    {
        if (explosionTime > 6.0f && enemyState == EnemyState.Stop)
        {
            enemyRb.constraints = RigidbodyConstraints.FreezeAll;
            enemyState = EnemyState.Stop;
            //stop = true;
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
            //enemymanager.DestroyEnemy();
            Destroy(this.gameObject);
        }
    }

    //目標を設定
    //playerが二人なことはエネミーマネージャーで感知済み
    private bool targetNum;
    PlayerManager playerManager;
    private float time = 0f;
    private float coolTime = 1f;
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
        //最初の目標設定
        if (enemyState == EnemyState.Ready)
        {
            //距離を調査
            for (int i = 0; i < m_players.Length; i++)
            {
                m_distance[i] = Vector3.Distance(transform.position, m_players[i].transform.position);
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

            if (m_target != null)
            {
                //Debug.Log(enemyState);
                enemyState = EnemyState.Go;
            }
        }
        //追いかけてるとき
        else if (enemyState == EnemyState.Go)
        {
            //追いかけてた目標が落ちたら
            if (playerManager.rescState == RescueState.Wait)
            {
                //もう一つへ
                ChangeTarget(!targetNum);
                //enemyState = EnemyState.ReSet;
            }
            /*if (playerManager.rescState != RescueState.None)
            {
                Debug.Log(playerManager.rescState);
            }*/
        }
        //クールタイムが必要なら追加
        /*else if (enemyState == EnemyState.ReSet)
        {
            Debug.Log(enemyState);
            time += Time.deltaTime;

            if (time > coolTime)
            {
                enemyState = EnemyState.Go;
                time = 0f;
            }
        }*/
    }

    Vector3 prePosition = new Vector3();
    private float maxSpeed = 0.20f;
    //スピードがmaxSpeedを超えたらブレーキがかかる
    private void Brake()
    {
        Vector3 enemyDirection = (transform.position - prePosition).normalized;
        float speed = Vector3.Distance(transform.position, prePosition);

        //Debug.Log("スピード"+speed);
        if (speed >= maxSpeed)
        {
            //m_moveSpeed = 0f;
            enemyRb.AddForce(-enemyDirection * (m_moveSpeed * 1.5f));
        }
        else
        {
            //ブレーキの無効か(dontuse)
            /*if (getMoveSpeed == 0f)
            {
                m_moveSpeed = getMoveSpeed;
            }*/
        }

        prePosition = transform.position;
    }

    private float resistForce = 5f;
    private float resisttilt = 15f;
    private void Funbari()
    {
        //ステージの傾きに沿ったベクトルを取得
        float tilt = enemymanager.GetStageTilt();
        Vector3 downOnBoard = enemymanager.GetDownOnBoard();

        //敵がどれくらい端にいるか
        Vector3 localPos = transform.position - m_stageCenter;

        // 球が下り側にいるか判定
        float dot = Vector3.Dot(localPos.normalized, downOnBoard);

        //ターゲットをリセットした時のクールタイム(必要なら)
        /*if (enemyState == EnemyState.ReSet)
        {
            Vector3 resistDir = -localPos.normalized;
            resistDir += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, UnityEngine.Random.Range(-0.3f, 0.3f));

            enemyRb.AddForce(resistDir * resistForce, ForceMode.Force);
            //Debug.Log(enemyState);
        }*/
        //探査機が10度以上傾いてて(20度未満)
        //敵が中心から10離れてて
        //敵が下り側にいるか
        if (tilt >= 10f && resisttilt > tilt && m_distanceFromCenter >= 13f && dot > 0)
        {
            Vector3 resistDir = -localPos.normalized;
            //resistDir += new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0, UnityEngine.Random.Range(-0.3f, 0.3f));

            enemyRb.AddForce(resistDir * resistForce, ForceMode.Force);
            //Debug.Log("踏ん張り！");
        }
        /*else if (tilt >= resisttilt && m_distanceFromCenter >= 13f && dot > 0)
        {
            //resisttiltの角度以上いったら踏ん張らない
        }*/
    }

    private void Die()
    {
        //探索気よりも外に出た、下に行ったら
        if (m_distanceFromCenter >= 17f|| transform.position.y < -1f)
        {
            anim.SetBool("Arrived", false);
            //enemymanager.DestroySphere();
            enemyState = EnemyState.Stop;
        }
    }

    //着地した時に近くにいたプレイヤーを追いかける
    private void OnCollisionEnter(Collision collision)
    {
        //自機に着いたら
        if (collision.gameObject.CompareTag("Ground"))
        {
            //目標を設定しているか
            if (enemyState == EnemyState.GetOn)
            {
                Debug.Log(enemyState);
                enemyState = EnemyState.Ready;
                //目標を設定
                //SetTarget();
                //Debug.Log("tuita!");

                //今の場所を記録
                prePosition = transform.position;
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("atatta!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {

    }
}