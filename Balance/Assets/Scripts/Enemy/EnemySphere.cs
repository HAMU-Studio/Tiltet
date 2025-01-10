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

    [Header("この敵がでるフィールド")]
    [SerializeField] private  EnemyType enemyType;

    [Header("動くスピード")]
    [SerializeField] private float m_moveSpeed = 1.0f;

    [Header("最低速度")]
    [SerializeField] private float m_minSpeed = 0.1f;

    [Header("最高速度")]
    [SerializeField] private float m_maxSpeed = 0.2f;

    [Header("踏ん張り始める角度")]
    [SerializeField] private float m_funbariAngle = 15.0f;

    [Header("爆発の範囲")]
    [SerializeField] private GameObject explosionRenge;

    [SerializeField] private GameObject explosionEffect;

    //[SerializeField] private GameObject[] home; 

    private Animator anim;
    private float[] m_distance;
    private GameObject[] m_players;
    private GameObject m_target;
    private Rigidbody enemyRb;
    private float funbariTime = 0.0f;
    private float m_angle = 0.0f;
    private float explosionTime = 0.0f;

    private bool arrived;
    private bool life;
    private bool escape;
    private bool brake;
    private bool funbari;
    private bool ableExplosion;
    private bool stop;

    Vector3 m_nowPos = new Vector3();
    Vector3 m_prePosition = new Vector3();
    Vector3 m_direction = new Vector3();
    Vector3 m_stageCenter = new Vector3(0.0f, 2.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        explosionTime += Time.deltaTime;

        if (life)
        {
            if (arrived)
            {
                if (!stop)
                {
                    m_nowPos = transform.position;

                    SetMoveSpeed();
                    Funbari();

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

                    if (funbari)
                    {
                        funbariTime += Time.deltaTime;

                        if (funbariTime > 1.0f)
                        {
                            funbari = false;
                            funbariTime = 0.0f;
                        }
                    }
                }
            }
        }
        else
        {
            if(escape)
            {
                m_direction = (new Vector3(25.0f, -9.3f, 34.2f) - transform.position).normalized;
            }
        }

        if (enemyType == EnemyType.VOLCANO)
        {
            //Explosion();

            if (explosionTime > 6.0f)
            {
                enemyRb.constraints = RigidbodyConstraints.FreezeAll;
                stop = true;
                anim.SetBool("explosion", true);
            }

            if (explosionTime > 7.0f)
            {
                explosionEffect.SetActive(true);
            }

            if (explosionTime > 7.5f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (life)
        {
            if (arrived)
            {
                /*if (funbari)
                {
                    enemyRb.AddForce((m_stageCenter - transform.position).normalized * m_angle / 5.0f);
                }*/
                if (!stop)
                {
                    enemyRb.AddForce(m_direction * m_moveSpeed);

                    Brake();
                }
            }
        }
        else
        {
            if (escape)
            {
                //enemyRb.AddForce(m_direction * m_moveSpeed);
            }
        }
    }

    private void Set()
    {
        arrived = false;
        life = true;
        escape= false;
        brake = false;
        funbari = false;
        stop = false;
        ableExplosion = false;

        //最初にこれでplayer初期化(消すな)
        m_players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        // どうせプレイヤーは二人なので二個で初期化
        m_distance = new float[2];

        enemyRb = GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void SetTarget()
    {
        //距離を調査
        for (int i = 0; i < m_players.Length; i++)
        {
            m_distance[i] = Vector3.Distance(transform.position, m_players[i].transform.position);
        }

        //どっちのplayerのほうが近いか
        m_target = m_players[0];
        if (m_distance[1] < m_distance[0])
        {
            m_target = m_players[1];
        }
    }
    private void DebugSetTarget()
    {
        m_target = m_players[0];
    }

    private void SetMoveSpeed()
    {
        //距離の２乗が返ってくる
        float distance = (m_stageCenter - m_target.transform.position).sqrMagnitude;

        if (distance < 50)
        {
            m_moveSpeed = 1.0f;
        }
        else if (distance >= 50 && distance < 200)
        {
            m_moveSpeed = 2.0f;
        }
        else
        {
            m_moveSpeed = 3.0f;
        }
    }

    private void Brake()
    {
        Vector3 nowPos = transform.position;
        Vector3 enemyDirection = (nowPos - m_prePosition).normalized;
        float speed = (nowPos - m_prePosition).magnitude;

        if (speed > m_maxSpeed)
        {
            enemyRb.AddForce(-enemyDirection * (m_moveSpeed + 1.5f));
        }

        m_prePosition = nowPos;
    }

    private void Funbari()
    {
        float distance = (this.transform.position - m_stageCenter).magnitude;
        Vector3 parallel = new Vector3(m_nowPos.x, m_stageCenter.y, m_nowPos.z);

        m_angle = Vector3.Angle((parallel - m_stageCenter), (m_nowPos - m_stageCenter));

        if (!funbari)
        {
            if (distance > 10.0f)
            {
                if (m_angle >= m_funbariAngle)
                {
                    Debug.Log("funbari");
                    funbari = true;
                }
            }
        }
    }

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

    //着地した時に近くにいたプレイヤーを追いかける
    private void OnCollisionEnter(Collision collision)
    {
        if (m_target == null)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                //DebugSetTarget();
               SetTarget();
                arrived = true;
                m_prePosition = transform.position;
            }
        }

        if(collision.gameObject.CompareTag("Terrain"))
        {
            if(!life)
            {
                escape = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            arrived = true;
            anim.SetBool("Arrived", true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            life = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}