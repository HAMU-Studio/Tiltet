using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySensor : MonoBehaviour
{
    [SerializeField] private TiltControl tiltControl; // TiltControlスクリプトへの参照

    void Update()
    {
        // TiltControlからRotationXとRotationZを取得
        if (tiltControl != null)
        {
            float rotationX = tiltControl.RotationX;
            float rotationZ = tiltControl.RotationZ;

            // 現在の回転を反映 (Y軸回転は維持)
            Vector3 currentRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(-rotationX, currentRotation.y, rotationZ);
        }
        else
        {
            Debug.LogError("TiltControlが設定されていません。Inspectorで設定してください。");
        }
    }
}
