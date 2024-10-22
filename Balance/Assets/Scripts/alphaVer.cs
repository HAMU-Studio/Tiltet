using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class alphaVer : MonoBehaviour
{
    [SerializeField] private GameObject MainParts;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            SceneManager.LoadScene("Fight", LoadSceneMode.Additive);
        }
    }
}
