using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicEffect : MonoBehaviour
{
    EnemyManager enemymanager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject enemyManager = GameObject.Find("EnemyManager");
        enemymanager = enemyManager.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //爆発のエフェクトが終了したら、デストロイと死んだ判定
    public void OnAnimationEnd()
    {
        enemymanager.DestroySphere();
        Destroy(transform.root.gameObject);
    }
}
