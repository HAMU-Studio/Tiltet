using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : MonoBehaviour
{
    [Header("動くスピード")]
    [SerializeField] private float moveSpeed = 0.5f;

    private float[] distance;
    private GameObject[] players;
    private GameObject target;
    private Rigidbody enemyRb;
    private bool arrived;

    Vector3 Direction = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        // targetがnullでないことを確認
        if (target != null)
        {
            //進行方向
            Direction = (target.transform.position - transform.position).normalized;
        }
        else
        {
            if(arrived)
            {
                SearchPlayer();
            }
        }
    }

    void FixedUpdate()
    {
        enemyRb.AddForce(Direction * moveSpeed);
    }

    private void Set()
    {
        arrived = false;

        //最初にこれでplayer初期化(消すな)
        players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        if (players.Length > 0)
        {
            distance = new float[players.Length];
        }

        enemyRb = GetComponent<Rigidbody>();
    }

    private void SearchPlayer()
    {
        //playerが1人もいない時
        if (players.Length == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            return;
        }
        else
        {
            //playerが1人の時
            if (players.Length == 1)
            {
                players = GameObject.FindGameObjectsWithTag("Player");
                target = players[0];
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

    //着地した時に近くにいたプレイヤーを追いかける
    private void OnCollisionEnter(Collision collision)
    {
        if (target == null)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                arrived = true;
                SearchPlayer();
            }
        }
    }
}