using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class alphaVer : MonoBehaviour
{
    [SerializeField] private GameObject MainParts;
    public GameObject[] GameObjectsTohidden;
    // Start is called before the first frame update
    void Start()
    {
        //シーンが破棄されたときに呼び出されるようにする
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            //サブシーンを呼び出しているときに非表示にするゲームオブジェクト
            foreach (GameObject obj in GameObjectsTohidden)
            {
                obj.SetActive(false);
            }
            //メインシーンにサブシーンを追加表示する
            Application.LoadLevelAdditive("Fight");
            //SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        }
    }

    private void Clear()
    {
        SceneManager.LoadScene("Clear");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MainParts"))
        {
            Clear();
        }

        if (collision.gameObject.CompareTag("EncountArea"))
        {
            //サブシーンを呼び出しているときに非表示にするゲームオブジェクト
            foreach (GameObject obj in GameObjectsTohidden)
            {
                obj.SetActive(false);
            }
            //メインシーンにサブシーンを追加表示する
            Application.LoadLevelAdditive("Fight");
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        //シーンが破棄されたときに呼び出される
        //今回の例では、サブシーンが破棄されたら呼び出されるようになっています
        Debug.Log("OnSceneUnloaded: " + current.name);

        //本当は、どのシーンが破棄されたのか確認してから処理した方が良いかもしれない

        //ゲームオブジェクトを表示する
        foreach (GameObject obj in GameObjectsTohidden)
        {
            obj.SetActive(true);
        }
    }
}
