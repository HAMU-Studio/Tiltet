using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySphere : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private bool testmove = false;

    private float[] distance;
    private GameObject[] players;
    private GameObject target;
    private Rigidbody enemyRb;   

    Vector3 Direction = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        //playerのタグがついているオブジェクトを代入
        players = GameObject.FindGameObjectsWithTag("Player");

        // players配列の長さに基づいてdistance配列を初期化
        if (players.Length > 0)
        {
            distance = new float[players.Length];
        }

        enemyRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(2.5f, 0, 0);
        SearchPlayer();

        // targetがnullでないことを確認
        if (target != null)
        {
            //進行方向
            Direction = (target.transform.position - transform.position).normalized;

            enemyRb.AddForce(Direction * moveSpeed);
        }
    }

    void FixedUpdate()
    {
        if(testmove)
        {
            Direction =new Vector3 (0f, 0f, 5f);
            enemyRb.AddForce(Direction);
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