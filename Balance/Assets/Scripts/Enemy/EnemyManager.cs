using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("0...丸 1...楕円")]
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval = 3.0f;

    [Header("敵が存在できる最大数")]
    [SerializeField] private int spawnLimit = 2;

    [Header("敵が一回にスポーンする数")]
    private int spawnNum = 1;

    private float spawnTime;

    //敵の数検知
    private bool ableSpawn;

    //プレイヤーが二人いたら始まる
    private bool start;

    //デバッグ用
    private bool circleEnemyTest;

    // Start is called before the first frame update
    void Start()
    {
        Set();

        circleEnemyTest = true;
    }

    // Update is called once per frame
    void Update()
    {
        //デバッグ用
        //Pを押すと1人でも始められる
        if(Input.GetKeyDown(KeyCode.P))
        {
            start = true;
        }

        if (start)
        {
            CheakEnemy();
            
            if(ableSpawn)
            {
                spawnTime += Time.deltaTime;

                if (spawnTime > spawnInterval)
                {
                    for (int i = 0; spawnNum > i; i++)
                    {
                        if (circleEnemyTest)
                        {
                            CircleEnemySpawn();
                        }
                        else
                        {
                            EnemySpawn();
                        }
                    }
                    spawnTime = 0;
                }
            }
        }
    }

    private void Set()
    {
        start = false;
        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;

        ableSpawn = true;
    }

    private void EnemySpawn()
    {
        int enemyKinds;
        int enemySpawnPos;
        //0...丸 1...楕円
        enemyKinds = Random.Range(0, enemys.Length);
        GameObject newEnemy = Instantiate(enemys[enemyKinds]);

        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;

        if (enemyKinds == 1)
        {
            if (enemySpawnPos == 0)
            {
                newEnemy.transform.Rotate(0, -90.0f, 0);
            }
            if (enemySpawnPos == 1)
            {
                newEnemy.transform.Rotate(0, 90.0f, 0);
            }
        }
    }

    private void CheakEnemy()
    {
        GameObject[] enemyNum;
        enemyNum = GameObject.FindGameObjectsWithTag("Enemy");

        if (spawnLimit <= enemyNum.Length)
        {
            ableSpawn = false;
        }
        else
        {
            ableSpawn = true;
        }
    }

    private void CheakPlayer()
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 2)
        {
            start = true;
        }
    }

    private void CircleEnemySpawn()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[0]);

        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }
}