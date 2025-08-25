using UnityEngine;
using System.FadeSystem;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    private enum Wave
    {
        WAVE1,
        WAVE2,
        WAVE3,
    }

    [SerializeField] private FadeAndSceneTransition _transition;

    [Header("探査機")]
    [SerializeField] private GameObject stage;
    [Header("0...丸 1...楕円")]
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval = 3.0f;

    [Header("敵が存在できる最大数")]
    [SerializeField] private int circleLimit;
    [SerializeField] private int ellipseLimit = 2;

    [Header("敵が一回にスポーンする数")]
    private int numSpawnAtOnce = 1;

    [Header("FirstWaveの敵の数")]
    [SerializeField] private int firstWave = 5;
    [Header("SecondWaveの敵の数")]
    [SerializeField] private int secondWave = 3;
    [Header("FinalWaveの敵の数")]
    [SerializeField] private int finalWave = 6;

    [Header("フェーズ表示用テキスト")]
    [SerializeField] Image waveText;
    [Header("スプライト格納配列")]
    [SerializeField] Sprite[] waveSprite;

    [Header("戦闘UI")]
    [SerializeField] private GameObject FightUI;

    [Header("ゲージ")]
    [SerializeField] Slider[] waveGauge;

    [Header("デバッグ用")]
    [SerializeField] private bool circleEnemyTest;
    [SerializeField] private bool ellipseEnemyTest;

    //敵がスポーンする範囲→EnemyGetOnへ
    public Vector3 minPos { get; set; }
    public Vector3 maxPos { get; set; }

    //敵のリミット渡し→WaveManagerへ
    public int CircleLimit {  get; set; }
    public int EllipseLimit {  get; set; }
    public int NumSpawnAtOnceLimit {  get; set; }

    private float spawnTime;
    private int m_EnemyNum = 0;
    private Wave wave;
    private int destroySphere;
    private int destroyEllipse;
    private int destroyEnemy;

    //敵の数検知
    private bool ableCircleSpawn;
    private bool ableEllipseSpawn;
    private bool ableSpawn;

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
    private bool once;
    private bool isClear;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }
    private void Set()
    {
        FightUI.SetActive(false);

        start = false;
        // ゲームが始まったと同時にスポーン（なくてもいい）
        spawnTime = spawnInterval;
        destroyEnemy = 0;

        ableCircleSpawn = true;
        ableEllipseSpawn = true;
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

        count1 = 0;
        count2 = 0;
        count3 = 0;
        noSphere = true;
        noEllipse = true;
        once = false;
        wave = Wave.WAVE1;

        for (int i = 0; i < waveGauge.Length; i++)
        {
            waveGauge[i] = waveGauge[i].GetComponent<Slider>();
        }
    }

    private ThrowawayMethod medhod;
    // Update is called once per frame
    void Update()
    {
        //SoundManager.instance.Play(BGM);

        //デバッグ用
        //Pを押すと1人でも始められる
        if (Input.GetKeyDown(KeyCode.P))
        {
            start = true;
        }

        if (time >= 0.5f && time <= 2f && once == false)
        {
            SoundManager.instance.Play("Siren");
            once = true;
        }

        if (time >= 5.0f)
        {
            
            if (once)
            {
                FightUI.SetActive(true);
                SoundManager.instance.Play(BGM);
                once = false;
            }

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

                        switch (wave)
                        {
                            case Wave.WAVE1:
                                waveText.sprite = waveSprite[0];
                                if (count1 >= firstWave)
                                {
                                    if (noSphere && noEllipse)
                                    {
                                        wave = Wave.WAVE2;
                                        destroyEnemy = 0;
                                        spawnTime = spawnInterval;
                                    }
                                }
                                else
                                {
                                    SpawnCircleEnemy();
                                    count1++;
                                    spawnTime = 0;
                                }
                                break;
                            case Wave.WAVE2:
                                waveText.sprite = waveSprite[1]; ;
                                if (count2 >= secondWave)
                                {
                                    if (noSphere && noEllipse)
                                    {
                                        wave = Wave.WAVE3;
                                        destroyEnemy = 0;
                                        spawnTime = spawnInterval;
                                    }
                                }
                                else
                                {
                                    SpawnEllipseEnemy();
                                    count2++;
                                    spawnTime = 0;
                                }
                                break;
                            case Wave.WAVE3:
                                waveText.sprite = waveSprite[2];
                                if (count3 >= finalWave)
                                {
                                    if (noSphere && noEllipse && !isClear)
                                    {
                                        StartCoroutine(GameManager.instance.FightClear());
                                        isClear = true;
                                    }
                                }
                                else
                                {
                                    EnemySpawn();
                                    count3++;
                                    spawnTime = 0;
                                }
                                break;
                        }
                    }

                }

                //ゲージの管理
                switch (wave)
                {
                    case Wave.WAVE1:
                        waveGauge[0].value = 1.0f - ((1.0f / firstWave) * destroyEnemy);
                        break;
                    case Wave.WAVE2:
                        waveGauge[0].value = 0f;
                        waveGauge[1].value = 1.0f - ((1.0f / secondWave) * destroyEnemy);
                        break;
                    case Wave.WAVE3:
                        waveGauge[1].value = 0f;
                        waveGauge[2].value = 1.0f - ((1.0f / finalWave) * destroyEnemy);
                        break;
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

    public Vector3 GetStageTilt()
    {
        //ステージの傾き渡し
        float tilt = Vector3.Angle(stage.transform.up, Vector3.up);

        //ステージの傾きに沿ったベクトル（下に傾いてるほうに向いてる）
        Vector3 downOnBoard = Vector3.ProjectOnPlane(Vector3.down, stage.transform.up).normalized;

        return downOnBoard;
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

        if (SphereNum.Length == 0)
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

    public void DestroyEnemy()
    {
        destroyEnemy++;
    }

    public void SpawnCircleEnemy()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[0]);
        SoundManager.instance.Play("EnemyFly");
        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }

    public void SpawnEllipseEnemy()
    {
        int enemySpawnPos;

        GameObject newEnemy = Instantiate(enemys[1]);
        SoundManager.instance.Play("EnemyFly");
        enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);
        newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;
    }
}