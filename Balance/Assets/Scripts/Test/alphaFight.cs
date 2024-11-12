using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class alphaFight : MonoBehaviour
{
    //メイン画面に戻るボタン押されたときの処理
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //サブシーンを削除してメインシーンを表示する
            SceneManager.UnloadScene("Fight");
        }
    }
}