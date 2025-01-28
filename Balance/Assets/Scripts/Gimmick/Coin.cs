using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum FieldType
    {
        GREEN,
        SNOW,
        VOLCANIC
    }

    [Header("このコインが置いてある場所")]
    [SerializeField] private FieldType fieldType;

    private bool count;

    CoinManager coinmanager;

    // Start is called before the first frame update
    void Start()
    {
        switch (fieldType)
        {
            case FieldType.GREEN:
                GameObject greenCoinManager = GameObject.Find("GreenCoinManager");
                coinmanager = greenCoinManager.GetComponent<CoinManager>();
                Debug.Log("みどり");
                break;
            case FieldType.SNOW:
                GameObject snowCoinManager = GameObject.Find("SnowCoinManager");
                coinmanager = snowCoinManager.GetComponent<CoinManager>();
                break;
            case FieldType.VOLCANIC:
                GameObject volcanicCoinManager = GameObject.Find("VolcanicCoinManager");
                coinmanager = volcanicCoinManager.GetComponent<CoinManager>();
                break;
            default:
                break;
        }

        count = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(count)
        {
            coinmanager.Count();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Ground"))
        {
            count = true;
        }
    }
}
