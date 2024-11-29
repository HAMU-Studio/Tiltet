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
    [Header("今のいる場所")]
    [SerializeField] private FIELD_TYPE FieldType;
    [Header("FirstWaveの敵の数")]
    [SerializeField] private int firstWave = 10;
    [Header("SecondWaveの敵の数")]
    [SerializeField] private int secondWave = 5;
    [Header("FinalWaveの敵の数")]
    [SerializeField] private int finalWave = 20;

    [Header("フェーズ表示用テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;

    EnemyManager enemymanager;
    //enemymanager.CircleLimit;
    //enemymanager.EllipseLimit;
    //enemymanager.NumSpawnAtOnce;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }
    
    private void Set()
    {
        enemymanager = this.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
