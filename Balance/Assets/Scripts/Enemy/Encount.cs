using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Encount : MonoBehaviour
{ 
    private CameraManager cameraManager;

    EncountManager encountmanager;
    [SerializeField] private string fightSceneName;

    void Start()
    {
        GameObject encountManager = GameObject.Find("EncountManager");
        encountmanager = encountManager.GetComponent<EncountManager>();

        // CameraManagerをシーン内から探して取得
        cameraManager = FindObjectOfType<CameraManager>();

        if (cameraManager == null)
        {
            Debug.LogError("CameraManagerがシーンに見つかりません。");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
           // encountmanager.isEncount = true;
            encountmanager.Encount(fightSceneName);

            if (cameraManager != null)
            {
                // 現在アクティブなカメラを取得
                CinemachineVirtualCamera activeCamera = cameraManager.GetActiveCamera();
                if (activeCamera != null)
                {
                    // アクティブなカメラのFOVをズーム
                    StartCoroutine(cameraManager.ZoomCamera(activeCamera, 20, 1f));
                }
                else
                {
                    Debug.LogWarning("アクティブなカメラが見つかりません。");
                }
            }

            Destroy(gameObject);
        }
    }
}