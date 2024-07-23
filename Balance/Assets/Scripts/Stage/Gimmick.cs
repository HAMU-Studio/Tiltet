using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

public class Gimmick : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject[] stands;
    [SerializeField] private GameObject[] parts;
    [SerializeField] private GameObject[] coinPositions;

    [SerializeField] private TextMeshProUGUI clearText;

    private int numCoin;
    private int countCoin;
    private int countParts;
    private GameObject[] Coins;

    // 部品の数で配列数を変える
    Vector3[] PartsPosition = new Vector3[2];

    // Start is called before the first frame update
    void Start()
    {
        // コインの枚数
        numCoin = 9;
        countCoin = 0;
        countParts = 0;

        PartsPosition[0] = stands[0].transform.position;
        PartsPosition[1] = stands[1].transform.position;

        PartsPosition[0].y += 10.0f;
        PartsPosition[1].y += 10.0f;

        GameObject LeftParts = Instantiate(parts[0]);
        LeftParts.transform.position = PartsPosition[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (countParts >= 2)
        {
            clearText.text = "Clear!";
        }
    }
    void FixUpdate()
    {

    }

    private void CoinSpawn()
    {
        Coins = new GameObject[numCoin];

        for (int i = 0; i < numCoin; i++)
        {
            Coins[i] = Instantiate(coin);
            Coins[i].transform.position = coinPositions[i].transform.position;
        }
    }

    private void CoinGimmick()
    {
        if (countCoin >= numCoin)
        {
            GameObject RightParts = Instantiate(parts[1]);
            RightParts.transform.position = PartsPosition[1];
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "CoinArea")
        {
            CoinSpawn();
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            countCoin++;

            CoinGimmick();
        }

        if(collision.gameObject.CompareTag("Parts"))
        {
            Destroy(collision.gameObject);
            countParts++;
        }
    }
}
