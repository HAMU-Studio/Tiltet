using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubPartsUI : MonoBehaviour
{
    [SerializeField] private Image number;
    [SerializeField] private GameObject ten;
    [SerializeField] private Sprite[] numbers;

    private int subparts;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        subparts = GameManager.instance.GetSubPartsNum();

        if (subparts == 10)
        {
            ten.SetActive(true);
        }
        else
        {
            number.sprite = numbers[subparts];
        }

    }
}
