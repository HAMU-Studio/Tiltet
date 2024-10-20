using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

public class GimmickManager : MonoBehaviour
{
    GameManager gamemanager;
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
    Vector3[] MainPartsPosition = new Vector3[3];
    Vector3[] SubPartsPosition = new Vector3[2];

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
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // コインの枚数
        numCoin = 0;

        countCoin = 5;
        countParts = 0;

        SubPartsPosition[0] = stands[0].transform.position + new Vector3(0, 15.0f, 0);
        SubPartsPosition[1] = stands[1].transform.position + new Vector3(0, 15.0f, 0);

        /*GameObject LeftParts = Instantiate(parts[0]);
        LeftParts.transform.position = SubPartsPosition[0];*/
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
            CoinParts.transform.position = SubPartsPosition[0];
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
            gamemanager.AddPartsNum();
        }
    }
}
