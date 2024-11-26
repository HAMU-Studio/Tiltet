using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum FIELD_TYPE
{
    GREEN,    //緑地帯
    VOLCANIC, //火山帯
    SNOW      //寒冷帯
}

public class PhaseManager : MonoBehaviour
{
    [Header("フェーズ表示用テキスト")]
    [SerializeField] TextMeshProUGUI phasesText;

    private int circleLimit;
    private int ellipseLimit;
    private int numSpawnAtOnce;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    private void Set()
    {
        //GameObject enemyManager = GameObject.Find("EnemyManager");
        EnemyManager enemymanager;
        enemymanager = this.GetComponent<EnemyManager>();
        circleLimit = enemymanager.CircleLimit;
        ellipseLimit = enemymanager.EllipseLimit;
        numSpawnAtOnce = enemymanager.NumSpawnAtOnce;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
