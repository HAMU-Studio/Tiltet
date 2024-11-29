using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Encount : MonoBehaviour
{
    private CameraManager cameraManager;

    void Start()
    {
        // CameraManagerをシーン内から探して取得
        cameraManager = FindObjectOfType<CameraManager>();

        if (cameraManager == null)
        {
            Debug.LogError("CameraManagerがシーンに見つかりません。");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Graund"))
        {
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
        }
    }
}