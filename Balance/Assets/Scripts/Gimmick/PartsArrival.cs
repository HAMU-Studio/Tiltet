using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartsArrival : MonoBehaviour
{
    [SerializeField] private GameObject part; // サブパーツ
    [Header("クリアするための滞在時間")]
    [SerializeField] private float needStayTime = 5.0f; // 滞在時間
    [Header("ゲージがなくなるまでの時間")]
    [SerializeField] private float gaugeDeleteTime = 2.0f;

    [SerializeField] private GameObject gauge;
    [SerializeField] private TextMeshProUGUI gaugeText;

    private float m_stayTime; // 現在の滞在時間を保持
    private float m_leaveTime;
    private float checkMove;
    private bool isStay;
    private bool inArea;

    StageMovement stagemovement;

    private void Start()
    {
        GameObject stageMovement= GameObject.Find("stage 2");
        stagemovement = stageMovement.GetComponent<StageMovement>();

        gauge.SetActive(false);
        gaugeText.text = "";

        part.SetActive(false); // サブパーツを見えないようにする
        m_stayTime = 0f; // 現在の滞在時間を初期化
        m_leaveTime = 0.0f;
        isStay = false;
    }

    private void Update()
    {
        CheckMove();

        if (inArea)
        {

            if (isStay)
            {
                gauge.SetActive(true);
                TimeGauge();
                m_stayTime += Time.deltaTime; // 滞在時間を減少させる
                if (m_stayTime >= needStayTime)
                {
                    part.SetActive(true); // サブパーツを表示
                    Destroy(gameObject); // エリアを削除
                    Destroy(gauge);
                    Destroy(gaugeText);
                }
                Debug.Log("ge-ge");
            }
            else
            {
                m_leaveTime += Time.deltaTime;
                if(m_leaveTime>=gaugeDeleteTime)
                {
                    gauge.SetActive(false);
                    gaugeText.text = "";
                }

            }
        }
        else
        {
            gauge.SetActive(false);
            gaugeText.text = "";
        }
    }

    //エリアで自機が3秒止まるとクリア
    private void CheckMove()
    {
        Vector3 movementAmount = stagemovement.MovementAmount;
        checkMove = (movementAmount.x + movementAmount.y + movementAmount.z);

        //Debug.Log(checkMove);
        /*if (movementAmount == null)
        {
            gauge.SetActive(false);
            gaugeText.text = "";
        }*/

        if (-0.1 < checkMove && checkMove < 0.1f)
        {
            isStay = true;
        }
        //動いちゃうとリセット
        else
        {
            isStay = false;
            m_stayTime = 0.0f;
        }
    }

    private void TimeGauge()
    {
        //double displaytext = Math.Floor(m_stayTime);
        int displaytext = (int)m_stayTime;
        gaugeText.text = displaytext.ToString();

        gauge.GetComponent<Image>().fillAmount = m_stayTime - displaytext;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            inArea = true;
            Debug.Log("haitta");
        }
    }
    // エリアから出たら滞在時間をリセット
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            inArea = false;
            m_stayTime = 0f; // 滞在時間をリセット
        }
    }
}