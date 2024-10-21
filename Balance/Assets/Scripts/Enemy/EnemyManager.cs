using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject stage;
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject[] enemySpawnPoints;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval = 3.0f;

    [Header("敵がスポーンする場所の数")]
    [SerializeField] private int spawnNum = 4;
    //一度にスポーンする数
    private int spawnLimit = 1;

    private float spawnTime;

    //敵の数検知
    private GameObject[] enemies;
    private int enemyKinds;
    private bool ableSpawn;

    //敵のスポーン場所
    Vector3 enemyPos = new Vector3();

    //スポーン範囲オブジェクト用
    Vector3 miniPos = new Vector3();
    Vector3 maxPos = new Vector3();

    private Rigidbody enemyRb;

    private float enemyCount;

    Vector3[] SpawnPos = new Vector3[4];


    // Start is called before the first frame update
    void Start()
    {
        Set();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;

        if (spawnTime > spawnInterval)
        { 
            CheakEnemy();

            if (ableSpawn)
            {
                for (int i = 0; spawnLimit > i; i++)
                {
                    EnemySpawn();
                }
            }

            spawnTime = 0;
        }
    }

    private void Set()
    {
        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;

        ableSpawn = true;

        enemyCount = 0;
    }

    private void EnemySpawn()
    {
        //0...丸 1...楕円
        enemyKinds = Random.Range(0, enemys.Length);
        GameObject newEnemy = Instantiate(enemys[enemyKinds]);

        spawnNum = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[spawnNum].transform.position;

        if (enemyKinds == 1)
        {
            if (spawnNum == 0)
            {
                newEnemy.transform.Rotate(0, -90.0f, 0);
            }
            if (spawnNum == 1)
            {
                newEnemy.transform.Rotate(0, 90.0f, 0);
            }
        }
    }

    private void CheakEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //enemyNum = enemies.Length;

        if (2 < enemies.Length)
        {
            ableSpawn = false;
        }
        else
        {
            ableSpawn = true;
        }

    }

}