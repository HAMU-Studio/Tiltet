using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

public class GimmickManager : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject[] stands;
    [SerializeField] private GameObject[] parts;
    [SerializeField] private GameObject[] coinPositions;

    [Header("クリアに必要なコインの取得数")]
    [SerializeField] int numCoin;

    //取ったコインの数
    private int countCoin;
    //取ったパーツの数
    private int countParts;

    // 部品の数で配列数を変える
    Vector3[] PartsPosition = new Vector3[2];

    // Start is called before the first frame update
    void Start()
    {
        Set();
        CoinSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (countParts >= 2)
        {

        }
    }
    void FixUpdate()
    {

    }

    private void Set()
    {
        // コインの枚数
        numCoin = 5;

        countCoin = 5;
        countParts = 0;

        PartsPosition[0] = stands[0].transform.position + new Vector3(0, 15.0f, 0);
        PartsPosition[1] = stands[1].transform.position;

        //PartsPosition[0] += new Vector3(0, 10.0f, 0);
        PartsPosition[1].y += 10.0f;

        /*GameObject LeftParts = Instantiate(parts[0]);
        LeftParts.transform.position = PartsPosition[0];*/
    }

    private void CoinSpawn()
    {
        GameObject Coin = Instantiate(coin);
        Coin.transform.position = coinPositions[countCoin].transform.position;
    }

    private void CoinGimmick()
    {
        if (countCoin >= numCoin)
        {
            GameObject CoinParts = Instantiate(parts[0]);
            CoinParts.transform.position = PartsPosition[0];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //特定のエリアに侵入するとコインが出現する仕掛け
        /*if (collision.gameObject.name == "CoinArea")
        {
            CoinSpawn();
            Destroy(collision.gameObject);
        }*/

        if(collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            countCoin++;

            if (countCoin < numCoin)
            {
                CoinSpawn();
            }
            else
            {
                CoinGimmick();
            }
        }

        if(collision.gameObject.CompareTag("Parts"))
        {
            Destroy(collision.gameObject);
            countParts++;
        }
    }
}
