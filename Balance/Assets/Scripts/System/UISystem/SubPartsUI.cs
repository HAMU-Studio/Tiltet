using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubPartsUI : MonoBehaviour
{
    [SerializeField] private Image number;
    [SerializeField] private GameObject ten;
    [SerializeField] private Sprite[] numbers;

    private int subParts;

    // Start is called before the first frame update
    void Start()
    {
        ten.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        subParts = GameManager.instance.GetSubPartsNum();

        if (subParts == 10)
        {
            ten.SetActive(true);
            number.sprite = numbers[0];
        }
        else
        {
            number.sprite = numbers[subParts];
        }
    }
}
