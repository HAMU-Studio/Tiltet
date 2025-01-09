using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    //いるかな？
    /*[Header("タイトルに戻りますか？")]
    [SerializeField] private GameObject questionAgain;  */

    [Header("タイトルシーンの名前")]
    [SerializeField] private string titleScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene(titleScene);
    }
}
