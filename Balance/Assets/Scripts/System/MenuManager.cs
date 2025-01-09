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

    // Start is called before the first frame update
    void Start()
    {
        
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
}
