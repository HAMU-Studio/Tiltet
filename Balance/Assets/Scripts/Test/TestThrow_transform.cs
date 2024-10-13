using UnityEngine;

public class TestThrow_transform : MonoBehaviour
{
    //投げる水平スピード
    public float speed = 10;

    //射出角度
    [Range(0.0f, 90.0f)] public float throwingAngle = 60.0f;

    //地上にいるときのY座標
    public float baseHeight = 1.0f;

    //投げるフラグ
    bool Throwing;

    Vector3 FromPoint;
    Vector3 ToPoint;
    float Travel;

    void Update()
    {
        _Throw();
    }

    void _Throw()
    {
        //左クリック時のマウス座標を取得
        if (Input.GetMouseButtonDown(0))
        {
            //マウスポインタの位置を指すレイを作成
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //地上を表す平面を作成
            var basePlane = new Plane(Vector3.up, -baseHeight);
            if (basePlane.Raycast(mouseRay, out var enter))
            {
                //レイと平面の交差があれば、その地点を目的地とする
                ToPoint = mouseRay.GetPoint(enter);

                //現在の位置を地上の高さに合わせ、その地点を出発地とする
                FromPoint = transform.position;
                FromPoint.y = baseHeight;

                //移動量を0.0にリセットしておく
                Travel = 0.0f;
                Throwing = true;

                print($"{FromPoint} -> {ToPoint}");
            }
        }

        if (Throwing)
        {
            //出発地からの水平移動量を求め...
            Travel += speed * Time.deltaTime;

            //出発地と目的地の距離を求め...
            var distance = Vector3.Distance(FromPoint, ToPoint);

            //進行割合を求め...
            var t = Travel / distance;

            if (t < 1.0f)
            {
                //tが0.5（つまり中間地点）からどれだけ離れているかを求める
                //中間地点で0.0、出発地や目的地で1.0となるような値にする
                var d = Mathf.Abs(t - 0.5f) * 2.0f;

                //現在の水平位置を決め...
                var p = Vector3.Lerp(FromPoint, ToPoint, t);

                //高さを二次関数の曲線に沿って調整し...
                p.y += Mathf.Tan(Mathf.Deg2Rad * this.throwingAngle) * 0.25f * distance * (1.0f - (d * d));

                //位置を設定する
                transform.position = p;
            }
            else
            {
                //tが1.0に到達したら移動終了とする
                transform.position = ToPoint;
                Throwing = false;
            }
        }
    }
}
    
