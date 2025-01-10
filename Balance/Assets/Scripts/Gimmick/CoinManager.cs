using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("コインオブジェクト(取る順番で入れる)")]
    [SerializeField] private GameObject[] coins;
    [Header("インスタンスするパーツオブジェクト")]
    [SerializeField] private GameObject parts;

    //取ったコインの数
    private int countCoin;

    // Start is called before the first frame update
    void Start()
    {
        Set();
        CoinSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        InstanceParts();
//        Debug.Log(countCoin);
    }

    private void Set()
    {
        countCoin = 0;

        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].SetActive(false);
        }
        parts.SetActive(false);
    }

    public void Count()
    {
        countCoin++;
        if (countCoin < coins.Length)
        {
            CoinSpawn();
        }
    }

    private void CoinSpawn()
    {
        coins[countCoin].SetActive(true);
    }

    private void InstanceParts()
    {
        if (countCoin >= coins.Length)
        {
            parts.SetActive(true);
        }
    }
}
