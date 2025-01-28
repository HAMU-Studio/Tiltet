using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace System
{
    public class FieldItemsManager : MonoBehaviour
    {
        public static FieldItemsManager instance = null;
        
        private Dictionary<string, bool> list = new();

        [HideInInspector]
        public bool[] acquired;

        private void Awake()
        {
            if (instance == null)
            {
                transform.parent = null;
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // 他のシーン遷移した時の二重生成防ぐ
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                foreach (var pair in list)
                {
                    Debug.Log($"Key: {pair.Key}, Value: {pair.Value}");
                }
            }
        }

        /// <summary>
        /// ゲーム開始時のみ呼び出したい
        /// </summary>
        /// <param name="items">フィールドアイテムのデータ</param>
        public void Initialize(GameObject[] items)
        {
            acquired = new bool[items.Length];
            list = new();

            for (int i = 0; i < items.Length; i++)
            {
                list.Add(items[i].name, acquired[i]);
            }
       
        }

        /// <summary>
        /// 自機が触れてパーツなどのItemを取得した時呼び出す Destroy
        /// </summary>
        /// <param name="name">gameObject.name</param>
        public void GetItem(string name)
        {
            // TryGetValueはコピーだから直接更新できない
            if (list.ContainsKey(name))
            {
                list[name] = true; // 直接更新
            }
            else
            {
                Debug.LogAssertion("The item does not exist : " + name);
            }
        }

        public bool GetIsAcquired(string name)
        {
            if (instance.list.TryGetValue(name, out bool acquired))
            {
                return acquired;
            }
            
            Debug.LogAssertion("The item does not exist : " + name);
            return false;
        }
        
    }
}