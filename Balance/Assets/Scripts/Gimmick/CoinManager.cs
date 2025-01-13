using Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    [Header("コインオブジェクトをインスタンスする場所(取る順番で入れる)")]
    [SerializeField] private GameObject[] coins;
    [Header("インスタンスするパーツオブジェクト")]
    [SerializeField] private GameObject parts;

    //取ったコインの数
    private int countCoin;

    // Start is called before the first frame update
    void Start()
    {
        Set();
        Count();
    }

    // Update is called once per frame
    void Update()
    {
        if (parts != null)
         InstanceParts();
    }   

    private void Set()
    {
        countCoin = 0;

        parts.SetActive(false);
    }

    public void Count()
    {
        countCoin++;
        if (countCoin < coins.Length)
        {
            CoinSpawn();
            if (countCoin > 1)
            {
                SoundManager.instance.Play("GetCoin");
                if (countCoin == 2)
                {
                    DisplayDialogue.dialogue.Enqueue("CoinExplain");
                }

                if (countCoin == 4)
                {
                    DisplayDialogue.dialogue.Enqueue("Good");
                }
            }
            
        }
    }

    private void CoinSpawn()
    {
        GameObject newCoin = Instantiate(coin);
        newCoin.transform.position = coins[countCoin].transform.position;
    }

    private void InstanceParts()
    {
        if (countCoin >= coins.Length && parts.activeSelf == false)
        {
            parts.SetActive(true);
            SoundManager.instance.Play("Arrival");
            DisplayDialogue.dialogue.Enqueue("Happy");
        }
    }
}
