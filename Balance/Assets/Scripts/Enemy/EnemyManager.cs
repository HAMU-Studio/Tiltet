using UnityEngine;
using System.FadeSystem;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    /*private enum Wave
    {
        WAVE1,
        WAVE2,
        WAVE3,
    }
    private Wave wave;*/

    //取り込み//
    [SerializeField] private FadeAndSceneTransition _transition;
    [Header("探査機")]
    [SerializeField] private GameObject stage;
    [Header("0...丸 1...楕円")]
    [SerializeField] private GameObject[] enemys;
    [SerializeField] private GameObject[] enemySpawnPoints;
    [Header("フェーズ表示用テキスト")]
    [SerializeField] Image waveText;
    [Header("スプライト格納配列")]
    [SerializeField] Sprite[] waveSprite;
    [Header("戦闘UI")]
    [SerializeField] private GameObject FightUI;
    [Header("ゲージ")]
    [SerializeField] Slider[] waveGauge;

    [Header("ウェーブの合計")]
    [SerializeField] private int waveNom;
    private int nowWave;

    [Header("敵がスポーンするインターバル")]
    [SerializeField] private float spawnInterval;

    //いる？？
    /*[Header("敵が存在できる最大数")]
    [SerializeField] private int circleLimit;
    [SerializeField] private int ellipseLimit;*/

    [Header("敵が一回にスポーンする数")]
    private int numSpawnAtOnce = 1;
    [Header("ウェーブごとの敵が出てくる数")]
    [SerializeField] private int[] sphereLimit;
    [SerializeField] private int[] ellipseLimit;

    private int[] totalEnemyNum;
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
    private int destroyEnemy;

    //敵の数検知
    private bool ableSphereSpawn;
    private bool ableEllipseSpawn;
    //private bool ableSpawn;

    //ウェーブごとにスポーンした数を数える
    private int countSpawnSphere;
    private int countSpawnEllipse;

    //プレイヤーが二人いたら始まる
    private bool start;

    private bool finishAnimation;

    private string BGM = "Fight"; 

    private float time;
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
        nowWave = 0;
        countSpawnEllipse = 0;
        countSpawnSphere = 0;

        ableSphereSpawn = true;
        ableEllipseSpawn = true;
        //ableSpawn = true;

        //敵がスポーンする範囲
        GameObject stage = GameObject.FindWithTag("Ground");
        Vector3 stagePos = stage.transform.position;

        minPos = new Vector3(stagePos.x - 7.0f,
                             stagePos.y + 2.05f,
                             stagePos.z - 6.0f);

        maxPos = new Vector3(stagePos.x + 7.0f,
                             stagePos.y + 2.05f,
                             stagePos.z + 6.0f);

        finishAnimation = false;
        noSphere = true;
        noEllipse = true;
        once = false;
        //wave = Wave.WAVE1;

        for (int i = 0; i < waveGauge.Length; i++)
        {
            waveGauge[i] = waveGauge[i].GetComponent<Slider>();
        }

        //初期化
        totalEnemyNum = new int[waveNom];
        //敵の出てくる数
        for (int i = 0; i < waveNom; i++)
        {
            totalEnemyNum[i] = sphereLimit[i] + ellipseLimit[i];
        }
    }

    private ThrowawayMethod medhod;
    // Update is called once per frame
    void Update()
    {
        //SoundManager.instance.Play(BGM);

        //デバッグ用
        //Pを押すと1人でも始められる
        if (Input.GetKeyDown(KeyCode.P) )
        {
            //Debug.Log(destroyEnemy);
            /*for (int i = 0; i < waveNom; i++)
            {
                Debug.Log(totalEnemyNum[i]);
            }*/
            start = true;
        }

        if (finishAnimation)
        {
            if (once)
            {
                waveNom--;
                FightUI.SetActive(true);
                SoundManager.instance.Play(BGM);
                once = false;
            }

            if (start)
            {
                CheckPresenceOfEnemy();
                ableSpawn();
                waveText.sprite = waveSprite[nowWave];
                waveGauge[nowWave].value = 1.0f - ((1.0f / totalEnemyNum[nowWave]) * destroyEnemy);

                spawnTime += Time.deltaTime;

                if (destroyEnemy >= totalEnemyNum[nowWave])
                {
                    NextWave();
                    Debug.Log(nowWave);
                }

                if (spawnTime > spawnInterval)
                {
                    EnemySpawn();
                    spawnTime = 0;
                }

                //デバッグ用
                /*if (ableSphereSpawn)
                {
                    SpawnCircleEnemy();
                }
                else if (ableEllipseSpawn)
                {
                    SpawnEllipseEnemy();
                }*/
            }
            else
            {
                CheckPlayer();
            }
        }
        else
        {
            if (once == false)
            {
                SoundManager.instance.Play("Siren");
                once = true;
            }
            // time += Time.deltaTime;
        }
    }

    private void ableSpawn()
    {
        if (countSpawnSphere < sphereLimit[nowWave])
        {
            ableSphereSpawn = true;
        }
        else
        {
            ableSphereSpawn = false;
        }
        if (countSpawnEllipse < ellipseLimit[nowWave])
        {
            ableEllipseSpawn = true;
        }
        else
        {
            ableEllipseSpawn = false;
        }
    }

    private void EnemySpawn()
    {
        //0...丸 1...楕円
        int enemyKinds;

        //どっちかがスポーンできるかどうか調べる
        if (ableEllipseSpawn || ableSphereSpawn)
        {
            //どっちも大丈夫
            if (ableSphereSpawn && ableEllipseSpawn)
            {
                enemyKinds = Random.Range(0, enemys.Length);
            }
            //丸だけ
            if (!ableSphereSpawn)
            {
                enemyKinds = 1;
            }
            //楕円だけ
            else
            {
                enemyKinds = 0;
            }
            
            int enemySpawnPos = Random.Range(0, enemySpawnPoints.Length);

            GameObject newEnemy = Instantiate(enemys[enemyKinds]);
            newEnemy.transform.position = enemySpawnPoints[enemySpawnPos].transform.position;

            //カウント
            if (enemyKinds == 0)
            {
                countSpawnSphere++;
            }
            if (enemyKinds == 1)
            {
                countSpawnEllipse++;
            }
        }
        else
        {
            //スポーンできない
        }
    }

    private void CheckPresenceOfEnemy()
    {
        GameObject[] EllipseNum;
        GameObject[] SphereNum;
        EllipseNum = GameObject.FindGameObjectsWithTag("EllipseEnemy");
        SphereNum = GameObject.FindGameObjectsWithTag("SphereEnemy");

        if (SphereNum.Length == 0)
        {
            noSphere = true;
        }
        else
        {
            noSphere = false;
        }

        if (EllipseNum.Length == 0)
        {
            noEllipse = true;
        }
        else
        {
            noEllipse = false;
        }

        //存在できる敵制限するの？？
        /*if (circleLimit <= SphereNum.Length)
       {
           ableSphereSpawn = false;
       }
       else
       {
           ableSphereSpawn = true;
       }
        if (ellipseLimit <= EllipseNum.Length)
        {
            ableEllipseSpawn = false;
        }
        else
        {
            ableEllipseSpawn = true;
        }*/
    }

    private void NextWave()
    {
        //敵が一人も残ってない状態
        if (noSphere && noEllipse)
        {
            if(nowWave < waveNom)
            {
                nowWave++;
            }
            else if (nowWave == waveNom)
            {
                start = false;
                StartCoroutine(GameManager.instance.FightClear());
            }
            waveGauge[nowWave - 1].value = 0f;

            //初期化
            countSpawnSphere = 0;
            countSpawnEllipse = 0;
            destroyEnemy = 0;
            spawnTime = spawnInterval;
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

    public void DestroySphere()
    {
        destroyEnemy++;
    }
    public void DestroyEllipse()
    {
        destroyEnemy++;
    }
    public Vector3 GetDownOnBoard()
    {
        //ステージの傾きに沿ったベクトル（下に傾いてるほうに向いてる）
        Vector3 downOnBoard = Vector3.ProjectOnPlane(Vector3.down, stage.transform.up).normalized;

        return downOnBoard;
    }
    public float GetStageTilt()
    {
        //ステージの傾き渡し
        float tilt = Vector3.Angle(stage.transform.up, Vector3.up);

        return tilt;
    }
    public void FinishAnimation()
    {
        finishAnimation = true;
    }

    //dontuse//
    /*private void CheckEllipseEnemy()
    {

    }*/
    /*public void DestroyEnemy()
   {
       destroyEnemy++;
   }*/
    /*public void SpawnCircleEnemy()
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
    }*/
}