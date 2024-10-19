using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //このあたりはプロトタイプのみ
    /*[SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private Button startButton;*/
    [SerializeField] private GameObject ConnectionScreen;
    
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
    }

    private bool once;
    void Update()
    {
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
           }
        }
        if (GameManager.instance.P2Spawn == true)
        {
           // Debug.Log("p2Spawn = " + GameManager.instance.P2Spawn);
           if (m_2PImage.activeSelf == false)
           {
               m_2PImage.SetActive(true);
           }
        }
    }

    private IEnumerator ConnectSuccess()
    {
        yield return new WaitForSeconds(2f);
        
        GameManager.instance.StartGame();
        ConnectionScreen.SetActive(false);
    }
}
