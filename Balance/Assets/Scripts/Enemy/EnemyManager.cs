using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;
using TMPro;

public enum FIELD_TYPE
{ 
    GREEN,    //緑地帯
    VOLCANIC, //火山帯
    SNOW      //寒冷帯
}

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

    [Header("フェーズ表示用テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;

    [Header("デバッグ用")]
    [SerializeField] private bool circleEnemyTest;
    [SerializeField] private bool ellipseEnemyTest;

    //敵がスポーンする範囲
    public Vector3 minPos { get; set; }
    public Vector3 maxPos { get; set; }

    private float spawnTime;

    //敵の数検知
    private bool ableSpawn;

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
            CheakEnemy();
            
            if(ableSpawn)
            {
                spawnTime += Time.deltaTime;

                if (spawnTime > spawnInterval)
                {
                    for (int i = 0; spawnNum > i; i++)
                    {
                        //デバッグ用
                        if (circleEnemyTest)
                        {
                            CircleEnemySpawn();
                        }
                        else if (ellipseEnemyTest)
                        {
                            EllipseEnemySpawn();
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

    private void CheakEnemy()
    {
        GameObject[] SphereNum;
        GameObject[] EllipseNum;
        SphereNum = GameObject.FindGameObjectsWithTag("SphereEnemy");
        EllipseNum = GameObject.FindGameObjectsWithTag("EllipseNum");

        if (spawnLimit <= SphereNum.Length)
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

    private void EllipseEnemySpawn()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[1]);

        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }
}