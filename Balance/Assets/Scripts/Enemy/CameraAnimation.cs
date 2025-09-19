using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    private GameObject enemyManager;
    EnemyManager enemymanager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GameObject.Find("EnemyManager");
        enemymanager = enemyManager.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAnimationEnd()
    {
        enemymanager.FinishAnimation();
        //Debug.Log("アニメーション終了！");
    }
}
