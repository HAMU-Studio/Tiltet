using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("ポーズ画面")]
    [SerializeField] private GameObject pauseImage;
    [Header("タイトルシーンの名前")]
    [SerializeField] private string titleScene;
    [Header("1P")]
    [SerializeField] private GameObject firstPlayerImage;
    [Header("2P")]
    [SerializeField] private GameObject secondPlayerImage;
    [SerializeField] private GameObject questionImage;
    // Start is called before the first frame update

    void Start()
    {
        pauseImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene(titleScene);
    }

    public void Question()
    {
        questionImage.SetActive(true);
    }

    public void No()
    {
        questionImage.SetActive(false);
    }

    //ポーズ画面表示
    public void DisplayPauseImage()
    {
        pauseImage.SetActive(true);
    }
    //ポーズ画面非表示
    public void HiddenPauseImage()
    {
        pauseImage.SetActive(false);
    }

    //1P画像表示
    public void Display1P()
    {
        firstPlayerImage.SetActive(true);
    }
    //2P画像表示
    public void Display2P()
    {
        secondPlayerImage.SetActive(true);
    }
}
