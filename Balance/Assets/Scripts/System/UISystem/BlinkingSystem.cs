using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class BlinkingSystem : MonoBehaviour
    {
        // GameObject aircraft; 自機の半透明は時間的にむりそう
        [Header("表示場所")] [SerializeField] Image[] lifeImage = default!;
        [Header("通常時画像")] [SerializeField] Sprite truelife = default!;
        [Header("ダメージ時画像")] [SerializeField] Sprite falselife = default!;
        /*[Header("通常時マテリアル")] [SerializeField] Material trueMaterial = default!;

        [Header("ダメージ時マテリアル")] [SerializeField]
        Material falseMaterial = default!;*/

        [Header("ダメージ時の表示間隔")] [SerializeField]
        float[] duration = default!;

        private static int lifeCount = 3;

        void Awake()
        {
            InitializeLife();
        }

        private void InitializeLife()
        {
            // シーン切り替わった時とかのために体力状況を確認した初期化
            for (int i = 0; i < GameManager.instance.Life; i++)
            {
                //lifeImage[i].enabled = truelife;
                lifeImage[i].sprite = truelife;
            }
        }

        //number：表示画像番号 x：偶数奇数判定
        void lifeChange(int number, int x)
        {
            if (x % 2 == 0)
            {
                lifeImage[number].sprite = falselife;
            //    aircraft.gameObject.GetComponent<Renderer>().material = falseMaterial;
            }
            else
            {
                lifeImage[number].sprite = truelife;
               // aircraft.gameObject.GetComponent<Renderer>().material = trueMaterial;
            }
        }

        public IEnumerator DamageIndication(int i)
        {
            //  _MainGameManager.isInvincible = true;   //点滅中は無敵に

            yield return new WaitForSeconds(0.15f);
            //WaitForSecondsでそれぞれ待機してからLifeChangeを行う
            for (int j = 0; j < duration.Length; j++)
            {
                lifeChange(i, j);
                yield return new WaitForSeconds(duration[j]);
            }

            //最後は減らさなければならないのでfalseに
            lifeImage[i].sprite = falselife;

            //プレイヤーのマテリアルを通常に。ハートのより点滅の回数が増えてしまう
            // yield return new WaitForSeconds(0.1f);
           // aircraft.gameObject.GetComponent<Renderer>().material = trueMaterial;
            GameManager.instance.Life--;
            if (GameManager.instance.Life <= 0)
            {
                // 負け判定
            }

            //  _MainGameManager.isInvincible = false;
            yield return null;
        }
    }
}