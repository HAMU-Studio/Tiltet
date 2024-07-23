using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySensor : MonoBehaviour
{
    public enum SensorType
    {
        Forward,  // 前方移動センサー
        Backward, // 後方移動センサー
        Left,     // 左移動センサー
        Right     // 右移動センサー
    }

    public SensorType sensorType; // センサーの種類を指定
    public GameObject stage; // 動かす床オブジェクト
    private StageMovement stageMovement; // StageMovement コンポーネントへの参照
    private HashSet<Collider> playersInRange = new HashSet<Collider>(); // 範囲内のプレイヤーをトラッキングするためのセット

    void Start()
    {
        // 床オブジェクトが設定されている場合、StageMovement コンポーネントを取得
        if (stage != null)
        {
            stageMovement = stage.GetComponent<StageMovement>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがセンサーの範囲に入ったとき
        if (other.CompareTag("Player") && stageMovement != null)
        {
            playersInRange.Add(other); // プレイヤーをセットに追加

            switch (sensorType)
            {
                case SensorType.Forward:
                    // センサーが「前方」の場合、床を前方に移動させる
                    stageMovement.AddForce(Vector3.forward);
                    break;
                case SensorType.Backward:
                    // センサーが「後方」の場合、床を後方に移動させる
                    stageMovement.AddForce(Vector3.back);
                    break;
                case SensorType.Left:
                    // センサーが「左」の場合、床を左に移動させる
                    stageMovement.AddForce(Vector3.left);
                    break;
                case SensorType.Right:
                    // センサーが「右」の場合、床を右に移動させる
                    stageMovement.AddForce(Vector3.right);
                    break;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // プレイヤーがセンサーの範囲から出たとき
        if (other.CompareTag("Player") && stageMovement != null)
        {
            playersInRange.Remove(other); // プレイヤーをセットから削除

            // センサーの種類に応じて力を削除
            switch (sensorType)
            {
                case SensorType.Forward:
                    stageMovement.RemoveForce(Vector3.forward);
                    break;
                case SensorType.Backward:
                    stageMovement.RemoveForce(Vector3.back);
                    break;
                case SensorType.Left:
                    stageMovement.RemoveForce(Vector3.left);
                    break;
                case SensorType.Right:
                    stageMovement.RemoveForce(Vector3.right);
                    break;
            }
        }
    }
}