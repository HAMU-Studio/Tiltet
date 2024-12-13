using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThrow : MonoBehaviour
{
    //動作確認用 写し
    
    /*
    * 1.マウスクリック
    * 2.クリックされた座標を取得
    * 3.取得した座標にSleap関数で飛ばす
    */

    // 落下地点（クリックした座標）
    Vector3 ThrowPoint = new Vector3(0, 0, 0);

    // 射出角度
    public float ThrowingAngle = 60;

    // 地上にいるときのY座標
    public float baseHeight = 1.0f;

    // 速度調整用のパラメータ
    public float speedMultiplier = 1.0f;  // デフォルトは1.0（変更可能）

    void Update()
    {
        _Throw();
    }

    void _Throw()
    {
        // 左クリック時のマウス座標を取得
        if (Input.GetMouseButtonDown(0))
        {
            // マウスポインタの位置を指すレイを作成
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 地上を表す平面を作成
            var basePlane = new Plane(Vector3.up, -baseHeight);

            if (basePlane.Raycast(mouseRay, out var enter))
            {
                // レイと平面の交差があれば、その地点を目的地とする
                ThrowPoint = mouseRay.GetPoint(enter);
                Throwing();
            }
        }
    }

    void Throwing()
    {
        // 射出速度を算出
        Vector3 velocity = CalculateVelocity(transform.position, ThrowPoint, ThrowingAngle);

        // 速度調整用のパラメータを掛ける
        velocity *= speedMultiplier;

        // 射出
        Rigidbody rid = this.GetComponent<Rigidbody>();

        // FreezePositionをオフにして...
        rid.constraints &= ~RigidbodyConstraints.FreezePosition;

        // 速度を直接設定
        rid.velocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 他の物体と接触したらFreezePositionをオンにする
        Rigidbody rid = this.GetComponent<Rigidbody>();
        rid.constraints |= RigidbodyConstraints.FreezePosition;
    }

    Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
}