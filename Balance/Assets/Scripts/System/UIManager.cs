using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //このあたりはプロトタイプのみ
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private Button startButton;
    public void StartButton()
    {
        startText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        GameManager.instance.StartGame();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
      
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
