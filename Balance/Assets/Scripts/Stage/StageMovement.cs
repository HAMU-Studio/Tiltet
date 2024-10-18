using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMovement : MonoBehaviour
{
    private TiltControl tiltControl;

    void Start()
    {
        // TiltControlコンポーネントを取得
        tiltControl = GetComponent<TiltControl>();

        // nullチェック
        if (tiltControl == null)
        {
            Debug.LogError("TiltControlが見つかりません。スクリプトを適切なオブジェクトにアタッチしてください。");
        }
    }

    void Update()
    {
        // MaxForcePointを取得してデバッグ出力
        if (tiltControl != null)
        {
            Vector3 maxForcePoint = tiltControl.MaxForcePoint;
            Debug.Log("最大の力がかかるポイント: " + maxForcePoint);
        }
    }

    /*public void StopMovement()
    {
        StopAllCoroutines();
        enabled = false;

        GravitySensor gravitySensor = GetComponent<GravitySensor>();
        if (gravitySensor != null)
        {
            gravitySensor.enabled = false;
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ |
                              RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EncountArea"))
        {
            StopMovement();
        }
    }*/
}
