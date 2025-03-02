using Dialogue;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class ConnectionScreen : MonoBehaviour
{
    [SerializeField] private GameObject connectionScreen;
    [SerializeField] private InGameUISystems UISystems;

    [SerializeField] private bool DebugDiaogue;
    
    [SerializeField] private GameObject m_1PImage;
    [SerializeField] private GameObject m_2PImage;
    
    void Start()
    {
        m_1PImage.SetActive(false);
        m_2PImage.SetActive(false);
        once = false;
        GameManager.instance.AircraftMoveSwitch(false);
        
        if (GameManager.instance.isConnected == true)
        {
            // すでに接続済み(戦闘から探索に戻った時など)
            connectionScreen.SetActive(false);
            SoundManager.instance.Play("GreenStage");
            GameManager.instance.AircraftMoveSwitch(true);
            UISystems.StartTimer();
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
        
        connectionScreen.SetActive(false);
        GameManager.instance.PlayerLock();
        
        if (DebugDiaogue) GameManager.instance.isSkip = true;
        
        if (GameManager.instance.isSkip == false)
        {
            OPDialogue();
            yield return new WaitForSeconds(15.5f);
        }
        
        GameManager.instance.isConnected = true;
 
        DisplayDialogue.system.Enqueue("Start");
        
        SoundManager.instance.Play("GreenStage");
        SoundManager.instance.Play("Start");
        GameManager.instance.PlayerUnLock();
        
        GameManager.instance.AircraftMoveSwitch(true);
        UISystems.StartTimer();

       
        if (GameManager.instance.isSkip == false)
        {
            yield return new WaitForSeconds(2f);
        
            StartDialogue();
        }

        if (GameManager.instance.isSkip == false)
        {
            GameManager.instance.isSkip = true;
            Debug.Log("isSkip = " + GameManager.instance.isSkip);
        }
            
        yield return null;
    }

    private void OPDialogue()
    {
        for (int i = 0; i < 5; i++)
        {
            DisplayDialogue.dialogue.Enqueue($"OP0{i + 1}");
        }
    }

    private void StartDialogue()
    {
        for (int i = 0; i < 3; i++)
        {
            DisplayDialogue.dialogue.Enqueue($"InGame0{i + 1}");
        }
    }
}
