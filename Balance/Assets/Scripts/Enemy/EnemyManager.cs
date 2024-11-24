using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;
using TMPro;


public class EnemyManager : MonoBehaviour
{
    [Header("0...丸 1...楕円")]
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval = 3.0f;

    [Header("丸い敵が存在できる最大数")]
    [SerializeField] private int circleLimit = 5;
    [SerializeField] private int ellipseLimit = 2;

    [Header("敵が一回にスポーンする数")]
    private int spawnNum = 1;

    [Header("フェーズ表示用テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;

    [Header("デバッグ用")]
    [SerializeField] private bool circleEnemyTest;
    [SerializeField] private bool ellipseEnemyTest;

    //敵がスポーンする範囲
    public Vector3 minPos { get; set; }
    public Vector3 maxPos { get; set; }

    private float spawnTime;
    private int m_EnemyNum = 0;

    //敵の数検知
    private bool ableCircleSpawn;
    private bool ableEllipseSpawn;

    //プレイヤーが二人いたら始まる
    private bool start;

    // Start is called before the first frame update
    void Start()
    {
        Set();
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
            CheckCircleEnemy();
            CheckEllipseEnemy();

            spawnTime += Time.deltaTime;

            if (spawnTime > spawnInterval)
            {
                for (int i = 0; spawnNum > i; i++)
                {
                    if (ableCircleSpawn)
                    {
                        SpawnCircleEnemy();
                    }

                    if (ableEllipseSpawn)
                    {
                        SpawnEllipseEnemy();
                    }
                }
                spawnTime = 0;
            }
        }
    }

    private void Set()
    {
        start = false;
        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;

        ableCircleSpawn = true;
        ableEllipseSpawn = true;

        //敵がスポーンする範囲
        GameObject stage = GameObject.FindWithTag("Ground");
        Vector3 stagePos = stage.transform.position;

        minPos = new Vector3(stagePos.x - 7.0f,
                             stagePos.y + 2.05f,
                             stagePos.z - 6.0f);

        maxPos = new Vector3(stagePos.x + 7.0f,
                             stagePos.y + 2.05f,
                             stagePos.z + 6.0f);
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
    }

    private void CheckCircleEnemy()
    {
        GameObject[] SphereNum;
        SphereNum = GameObject.FindGameObjectsWithTag("SphereEnemy");

        if (circleLimit <= SphereNum.Length)
        {
            ableCircleSpawn = false;
        }
        else
        {
            ableCircleSpawn = true;
        }
    }
    private void CheckEllipseEnemy()
    {
        GameObject[] EllipseNum;
        EllipseNum = GameObject.FindGameObjectsWithTag("EllipseEnemy");

        if (ellipseLimit <= EllipseNum.Length)
        {
            ableEllipseSpawn = false;
        }
        else
        {
            ableEllipseSpawn = true;
        }
    }

    private void CheckPlayer()
    {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 2)
        {
            start = true;
        }
    }

    private void SpawnCircleEnemy()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[0]);

        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }

    private void SpawnEllipseEnemy()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[1]);

        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }
}