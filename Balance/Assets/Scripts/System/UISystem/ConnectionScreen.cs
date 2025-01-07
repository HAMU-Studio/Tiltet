using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class ConnectionScreen : MonoBehaviour
{
    //このあたりはプロトタイプのみ
    /*[SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private Button startButton;*/
    [SerializeField] private GameObject connectionScreen;
    [SerializeField] private InGameUISystems UISystems;
    
    [SerializeField] private GameObject m_1PImage;
    [SerializeField] private GameObject m_2PImage;
    public void StartButton()
    {
        /*startText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);*/
        /*m_1PImage.SetActive(false);
        m_2PImage.SetActive(false);
        GameManager.instance.StartGame();*/
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_1PImage.SetActive(false);
        m_2PImage.SetActive(false);
        once = false;
        GameManager.instance.AircraftMoveSwitch(false);
        
        if (GameManager.instance.isConnected == true)
        {
            connectionScreen.SetActive(false);
            SoundManager.instance.Play("GreenStage");
            GameManager.instance.AircraftMoveSwitch(true);
        }
    }

    private bool once;
    void Update()
    {
        if (GameManager.instance.isConnected)
            return;
            
        if (m_1PImage.activeSelf && m_2PImage.activeSelf)
        {
            if (once == false)
            {
                StartCoroutine("ConnectSuccess");
                once = true;
            }
            else
            {
                return;
            }
        }
        
        //プレイヤーがスポーンしたら対応する画像オン、今後アニメーションに変わりそう
        if (GameManager.instance.P1Spawn == true)
        {
           // Debug.Log("p1Spawn = " + GameManager.instance.P1Spawn);
           if (m_1PImage.activeSelf == false)
           {
               m_1PImage.SetActive(true);
               SoundManager.instance.Play("Connected");
           }
        }
        if (GameManager.instance.P2Spawn == true)
        {
           // Debug.Log("p2Spawn = " + GameManager.instance.P2Spawn);
           if (m_2PImage.activeSelf == false)
           {
               m_2PImage.SetActive(true);
               SoundManager.instance.Play("Connected");
           }
        }
    }

    /// <summary>
    /// 接続完了したら二秒待ってゲーム開始
    /// </summary>
    /// <returns></returns>
    private IEnumerator ConnectSuccess()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.isConnected = true;
        connectionScreen.SetActive(false);
        SoundManager.instance.Play("GreenStage");
        GameManager.instance.AircraftMoveSwitch(true);
        UISystems.StartTimer();
    }
}
