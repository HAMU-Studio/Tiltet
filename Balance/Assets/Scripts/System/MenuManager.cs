using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("タイトル画面")]
    [SerializeField] private GameObject title;
    [Header("オプション画面")]
    [SerializeField] private GameObject option;
    [Header("ゲームをやめますか画面")]
    [SerializeField] private GameObject question;
    [SerializeField] private GameObject fadeImage;


    // Start is called before the first frame update
    void Start()
    {
        title.SetActive(true);
        option.SetActive(false);
        question.SetActive(false);
        fadeImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Title()
    {
        title.SetActive(true);
        option.SetActive(false);
    }

    public void Option()
    {
        title.SetActive(false);
        option.SetActive(true);
    }

    public void Question()
    {
        question.SetActive(true);
    }

    public void No()
    {
        question.SetActive(false);
    }

}
