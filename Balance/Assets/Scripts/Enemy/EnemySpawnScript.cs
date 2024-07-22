using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;

    //スポーン範囲オブジェクト
    [SerializeField] private GameObject minimumValue;
    [SerializeField] private GameObject muximumValueX;
    [SerializeField] private GameObject muximumValueZ;

    //敵がスポーンするインターバル
    [SerializeField] private float spawnInterval = 3.0f;

    //一度にスポーンする数
    [SerializeField] private int spawnNum = 1;

    private float spawnTime;

    //スポーンの範囲
    private float lowestPositionX;
    private float lowestPositionZ;
    private float highestPositionX;
    private float highestPositionZ;

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

    // Start is called before the first frame update
    void Start()
    {
        //スポーン範囲オブジェクトのポジション取得
        miniPos = minimumValue.transform.position;
        maxPosX = muximumValueX.transform.position;
        maxPosZ = muximumValueZ.transform.position;

        //スポーン範囲代入
        lowestPositionX = miniPos.x;
        lowestPositionZ = miniPos.z;
        highestPositionX = maxPosX.x;
        highestPositionZ = maxPosZ.z;

        //高さの取得
        enemyPos.y = miniPos.y;

        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;

        ableSpawn = true;
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
                for (int i = 0; spawnNum > i; i++)
                {
                    EnemySpawn();
                }
            }

            spawnTime = 0;
        }
    }

    private void EnemySpawn()
    {
        enemyKinds = Random.Range(0, 2);
        GameObject newEnemy = Instantiate(enemy[enemyKinds]);

        enemyPos.x = Random.Range(lowestPositionX, highestPositionX);
        enemyPos.z = Random.Range(lowestPositionZ, highestPositionZ);
        //enemyPos = new Vector3(ramdomPositionX, 0, ramdomPositionZ);

        newEnemy.transform.position = enemyPos;
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