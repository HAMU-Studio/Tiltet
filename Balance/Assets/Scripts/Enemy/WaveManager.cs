using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public enum FIELD_TYPE
    {
        GREEN,    //緑地帯
        VOLCANIC, //火山帯
        SNOW      //寒冷帯
    }
    private enum Wave
    {
        WAVE1,
        WAVE2,
        WAVE3,
    }

    [Header("今のいる場所")]
    [SerializeField] private FIELD_TYPE FieldType;

    [Header("FirstWaveの敵の数")]
    [SerializeField] private int firstWave = 5;
    [Header("SecondWaveの敵の数")]
    [SerializeField] private int secondWave = 3;
    [Header("FinalWaveの敵の数")]
    [SerializeField] private int finalWave = 10;

    [Header("戦闘UI")]
    [SerializeField] GameObject waveGauge;
    [Header("テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;
    [Header("ゲージ")]

    private Wave wave;
    
    EnemyManager enemymanager;
    //enemymanager.CircleLimit;
    //enemymanager.EllipseLimit;
    //enemymanager.NumSpawnAtOnce;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }
    
    private void Set()
    {
        enemymanager = this.GetComponent<EnemyManager>();
        time = 0f;
        waveGauge.SetActive(false);

        wave = Wave.WAVE1;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time >= 5.0f)
        {
            waveGauge.SetActive(true);
        }

        switch(wave)
        {
            case Wave.WAVE1:

        }
    }

    private void FirstWave()
    {
        enemymanager.CircleLimit = firstWave;
        enemymanager.EllipseLimit = 0;
    }
    private void SecondWave()
    {
        enemymanager.CircleLimit = firstWave;
        enemymanager.EllipseLimit = 0;
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SphereEnemy"))
        {

            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("EllipseEnemy"))
        {

        }
    }
}
