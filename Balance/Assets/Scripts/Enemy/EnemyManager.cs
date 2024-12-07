using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using UnityEngine;
using TMPro;
using FadeSystem;



public class EnemyManager : MonoBehaviour
{
    [SerializeField] private FadeAndSceneTransition _transition;

    [Header("0...丸 1...楕円")]
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval = 3.0f;

    [Header("敵が存在できる最大数")]
    [SerializeField] private int circleLimit = 5;
    [SerializeField] private int ellipseLimit = 2;

    [Header("敵が一回にスポーンする数")]
    private int numSpawnAtOnce = 1;

    [Header("フェーズ表示用テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;

    [Header("デバッグ用")]
    [SerializeField] private bool circleEnemyTest;
    [SerializeField] private bool ellipseEnemyTest;

    //敵がスポーンする範囲→EnemyGetOnへ
    public Vector3 minPos { get; set; }
    public Vector3 maxPos { get; set; }

    //敵のリミット渡し→PhaseManagerへ
    public int CircleLimit {  get; set; }
    public int EllipseLimit {  get; set; }
    public int NumSpawnAtOnceLimit {  get; set; }

    private float spawnTime;
    private int m_EnemyNum = 0;

    //敵の数検知
    private bool ableCircleSpawn;
    private bool ableEllipseSpawn;

    //プレイヤーが二人いたら始まる
    private bool start;

    private string BGM = "Fight"; 

    private float time;
    private bool wave1;
    private int count1;
    private bool wave2;
    private int count2;
    private bool wave3;
    private int count3;
    private bool noSphere;
    private bool noEllipse;

    // Start is called before the first frame update
    void Start()
    {
        wave1 = false;
        wave2 = false;
        wave3 = false;
        count1 = 0;
        count2 = 0;
        count3 = 0;
        noSphere = true;
        noEllipse = true;


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

        if (time >= 5.0f)
        {

            SoundManager.instance.Play();

            if (start)
            {
                CheckCircleEnemy();
                CheckEllipseEnemy();

                spawnTime += Time.deltaTime;

                if (spawnTime > spawnInterval)
                {
                    for (int i = 0; numSpawnAtOnce > i; i++)
                    {
                        //デバッグ用
                        /*if (ableCircleSpawn)
                         {
                             SpawnCircleEnemy();
                         }
                         else if (ableEllipseSpawn)
                         {
                             SpawnEllipseEnemy();
                         }*/

                        //β用
                        if(!wave1)
                        {
                            phasesText.text = "FirstWave";
                            count1++;
                            if (count1 >= 5)
                            {
                                if (noSphere)
                                {
                                    wave1 = true;
                                }
                            }
                            else
                            {
                                SpawnCircleEnemy();
                            }
                        }
                        else
                        {
                            if(!wave2)
                            {
                                phasesText.text = "SecondWave";
                                count2++;
                                if (count2 >= 3)
                                {
                                    if (noEllipse)
                                    {
                                        wave2 = true;
                                    }
                                }
                                else
                                {
                                    SpawnEllipseEnemy();
                                }
                            }
                            else
                            {
                                if (!wave3)
                                {
                                    phasesText.text = "LastWave";
                                    count3++;
                                    if (count3 >= 8)
                                    {
                                        if (noSphere && noEllipse)
                                        {
                                            wave3 = true;
                                            _transition.FadeStart();
                                            GameManager.instance.CurrentState = GameState.Search;
                                        }
                                    }
                                    else
                                    {
                                        EnemySpawn();
                                    }

                                }
                            }
                        }
                    }
                    spawnTime = 0;
                }
            }
            else
            {
                CheckPlayer();
            }
        }
        else
        {
            time += Time.deltaTime;
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
        if (ableCircleSpawn || ableEllipseSpawn)
        {  
            int enemyKinds;
            //0...丸 1...楕円
            enemyKinds = Random.Range(0, enemys.Length);

            if (enemyKinds == 0)
            {
                if (!ableCircleSpawn)
                {
                    enemyKinds = 1;
                }
            }
            else if (enemyKinds == 1)
            {
                if (!ableEllipseSpawn)
                {
                    enemyKinds = 0;
                }
            }
            int enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);

            GameObject newEnemy = Instantiate(enemys[enemyKinds]);
            newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
        }
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

        if(SphereNum.Length==0)
        {
            noSphere = true;
        }
        else
        {
            noSphere = false;
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

        if (EllipseNum.Length == 0)
        {
            noEllipse = true;
        }
        else
        {
            noEllipse = false;
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