using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class EnemyGetOn : MonoBehaviour
{
    //スポーン範囲オブジェクト
    [SerializeField] private GameObject minimumValue;
    [SerializeField] private GameObject muximumValue;

    [Header("飛ぶときの最高点")] 
    [SerializeField] private int addHight = 5;

    //スポーン範囲オブジェクト用
    Vector3 miniPos = new Vector3();
    Vector3 maxPos = new Vector3();

    //ベジェ曲線用
    Vector3 spawnPosition = new Vector3();
    Vector3 top = new Vector3();
    Vector3 destination = new Vector3();
    private float enemySpeed;
    private float t = 0.0f;

    //探査機に到着したか否か
    private bool arrived;

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
        arrived = false;

        //スポーン範囲オブジェクトのポジション取得
        miniPos = minimumValue.transform.position;
        maxPos = muximumValue.transform.position;

        //出発地点
        spawnPosition = transform.position;

        //目的地の設定
        destination.x = Random.Range(miniPos.x, maxPos.x);
        destination.y = miniPos.y;
        destination.z = Random.Range(miniPos.z, maxPos.x);

        //飛ぶときの高さの最高到達点
        top = new Vector3((spawnPosition.x + destination.x) / 2,
                           spawnPosition.y + addHight,
                          (spawnPosition.z + destination.z) / 2);

        enemySpeed = 10 / Vector3.Distance(spawnPosition, destination);
    }

    private void GetOn()
    {
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
    }
}
