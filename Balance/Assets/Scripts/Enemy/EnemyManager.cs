using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    [SerializeField] private GameObject[] spawnPoint;

    //敵がスポーンするインターバル
    [SerializeField] private float spawnInterval = 3.0f;

    //一度にスポーンする数
    [SerializeField] private int spawnLimit = 1;

    private float spawnTime;

    //敵の数検知
    private GameObject[] enemies;
    private int enemyKinds;
    private bool ableSpawn;

    //敵のスポーン場所
    Vector3 enemyPos = new Vector3();

    //スポーン範囲オブジェクト用
    Vector3 miniPos = new Vector3();
    Vector3 maxPosX = new Vector3();
    Vector3 maxPosZ = new Vector3();

    private Rigidbody enemyRb;

    private float enemyCount;
    private int spawnNum;
    

    // Start is called before the first frame update
    void Start()
    { 
        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;

        ableSpawn = true;

        enemyCount = 0;

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

    private void EnemySpawn()
    {
        //0...丸 1...楕円
        enemyKinds = Random.Range(0, enemy.Length);
        GameObject newEnemy = Instantiate(enemy[enemyKinds]);

        spawnNum = Random.Range(0, spawnPoint.Length);
        newEnemy.transform.position = spawnPoint[spawnNum].transform.position;

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