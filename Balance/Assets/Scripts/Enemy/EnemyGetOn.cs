using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class EnemyGetOn : MonoBehaviour
{
    Vector3 stagePos = new Vector3();

    [Header("飛ぶときの最高点")] 
    [SerializeField] private int addHight = 5;

    //ベジェ曲線用
    Vector3 spawnPosition = new Vector3();
    Vector3 top = new Vector3();
    Vector3 destination = new Vector3();
    private float enemySpeed;
    private float t = 0.0f;

    //探査機に到着したか否か
    private bool arrived;

    private bool gotOff;

    // Start is called before the first frame update
    void Start()
    {
        Set(); 
    }

    // Update is called once per frame
    void Update()
    {
       if (!arrived)
       {
            GetOn();
       }
    }

    private void Set()
    {
        //スポーンする範囲の取得
        GameObject enemyManager = GameObject.Find("EnemyManager");
        EnemyManager enemymanager;
        enemymanager = enemyManager.GetComponent<EnemyManager>();
        Vector3 minPos = enemymanager.minPos;
        Vector3 maxPos = enemymanager.maxPos;

        arrived = false;
        gotOff = false;

        //出発地点
        spawnPosition = transform.position;

        //目的地の設定
        destination.x = Random.Range(minPos.x, maxPos.x);
        destination.y = minPos.y;
        destination.z = Random.Range(minPos.z, maxPos.x);

        //飛ぶときの高さの最高到達点
        top = new Vector3((spawnPosition.x + destination.x) / 2,
                           spawnPosition.y + addHight,
                          (spawnPosition.z + destination.z) / 2);

        enemySpeed = 10 / Vector3.Distance(spawnPosition, destination);
    }

    private void GetOn()
    {
        //着地地点を見る
        transform.LookAt(destination);

        t += enemySpeed * Time.deltaTime;
        Vector3 a = Vector3.Lerp(spawnPosition, top, t);
        Vector3 b = Vector3.Lerp(top, destination, t);

        transform.position = Vector3.Lerp(a, b, t);

        if (t > 1)
        {
           arrived = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            arrived = true;
        }

        if(collision.gameObject.CompareTag("Destroy"))
        {
            gotOff = false;
        }
    }
}
