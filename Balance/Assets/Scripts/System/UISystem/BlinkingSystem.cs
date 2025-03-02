using Dialogue;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace System
{
    public class BlinkingSystem : MonoBehaviour
    {
        [Header("表示場所")] [SerializeField] Image[] lifeImage = default!;
        [Header("通常時画像")] [SerializeField] Sprite[] truelife = default!;

        [Header("ダメージ時の表示間隔")] [SerializeField]
        float[] duration = default!;

        private static int lifeCount = 12;

        void Awake()
        {
            InitializeLife();
        }

        private void InitializeLife()
        {
            // シーン切り替わった時とかのために体力状況を確認した初期化
            // 体力は3*4
            for (int i = 0; i < 12; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i + j < GameManager.instance.Life) // 体力状況を加味
                    {
                        lifeImage[i + j].sprite = truelife[j];
                    }
                }
            }
        }

        //number：表示画像番号 x：偶数奇数判定
        void lifeChange(int number, int x)
        {
            if (x % 2 == 0)
            {
                lifeImage[number].enabled = false;
            //    aircraft.gameObject.GetComponent<Renderer>().material = falseMaterial;
            }
            else
            {
                lifeImage[number].enabled = true;
               // aircraft.gameObject.GetComponent<Renderer>().material = trueMaterial;
            }
        }

        private bool isInvicible;
        public IEnumerator DamageIndication(int i)
        {
            if (!isInvicible)
                isInvicible = true;
            else
            {
                yield break;
            }
            //  _MainGameManager.isInvincible = true;   //点滅中は無敵に

            yield return new WaitForSeconds(0.15f);
            //WaitForSecondsでそれぞれ待機してからLifeChangeを行う
            for (int j = 0; j < duration.Length; j++)
            {
                lifeChange(i, j);
                yield return new WaitForSeconds(duration[j]);
            }

            //最後は減らさなければならないのでfalseに
            lifeImage[i].enabled = false;
            
         
           if (GameManager.instance.Life == 12)
           {
               DisplayDialogue.dialogue.Enqueue("hyo");
               DisplayDialogue.dialogue.Enqueue("wa");
           }

           if (GameManager.instance.Life == 6)
           {
               DisplayDialogue.dialogue.Enqueue("wa");
               DisplayDialogue.dialogue.Enqueue("HalfLife01");
           }

           if (GameManager.instance.Life == 3)
           {
               DisplayDialogue.dialogue.Enqueue("hyo");
               DisplayDialogue.dialogue.Enqueue("LifePinch");
               DisplayDialogue.dialogue.Enqueue("HalfLife02");
           }
           
            GameManager.instance.Life--;
            if (GameManager.instance.Life <= 0)
            {
                StartCoroutine(GameManager.instance.GameOver());
            }
            
            isInvicible = false;
            yield return null;
        }
    }
}