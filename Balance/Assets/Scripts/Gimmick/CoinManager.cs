using Dialogue;
using System;
using UnityEngine;
using UnityEngine.Serialization;


public class CoinManager : MonoBehaviour
{
    [FormerlySerializedAs("coinObj")] [FormerlySerializedAs("coin")] [SerializeField] private GameObject coinPrefab;
    [Header("コイン生成場所(取る順番で入れる)")]
    [SerializeField] private GameObject[] coins;
    [Header("インスタンスするパーツ")]
    [SerializeField] private GameObject parts;

    [SerializeField] private GimmickState gimmickType;

    //取ったコインの数
    private int countCoin;
    
    void Start()
    {
        Init();
    }
    
    private void Init()
    {
        countCoin = 0;
        parts.SetActive(false);
        // 最初のコインスポーン
        CountCoin();
    }

    private void CountCoin()
    {
        countCoin++;
        if (countCoin <= coins.Length)
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

    // 次のコインがスポーンするたびにイベントの再登録をする
    private Coin coin;
    private void CoinSpawn()
    {
        if (coinPrefab == null)
            return;
        
        GameObject coinObj = Instantiate(coinPrefab);
        coinObj.transform.position = coins[countCoin - 1].transform.position;
        
        // 次のコインのインスタンスにイベント登録
        coin = coinObj.GetComponent<Coin>();
        coin.OnGetCoin += OnGetCoin;
    }

    private void OnGetCoin()
    {
        if (GameManager.instance.GimmickState == GimmickState.Normal)
        {
            // 一つ目ならその地帯のStateに切り替え
            GameManager.instance.GimmickState = gimmickType;
        }
        else if (GameManager.instance.GimmickState != gimmickType)
        {
            // 他エリアのギミック進行中はキャンセル
            Debug.Log("他エリアのギミックが進行中です。");
            return;
        }
        
        coin.OnGetCoin -= OnGetCoin;
        coin.DestroyCoin();
        CountCoin();

        if (countCoin > coins.Length && parts.activeSelf == false)
        {
            InstanceParts();
            GameManager.instance.GimmickState = GimmickState.Normal;
        }
    }

    private void InstanceParts()
    {
        parts.SetActive(true);
        
        SoundManager.instance.Play("Arrival");
        DisplayDialogue.dialogue.Enqueue("Happy");
    }
}
