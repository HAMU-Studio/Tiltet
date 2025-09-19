using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    EnemyManager enemymanager;

    // Start is called before the first frame update
    void Start()
    {
        Set();
    }
    
    private void Set()
    {
        GameObject enemyManager = GameObject.Find("EnemyManager");
        enemymanager = enemyManager.GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SphereEnemy"))
        {
            enemymanager.DestroySphere();
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("EllipseEnemy"))
        {
            enemymanager.DestroyEllipse();
            Destroy(other.gameObject);
        }
    }
}
