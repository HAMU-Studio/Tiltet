using UnityEngine;

namespace System
{
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] private GameObject savePoint;
        private void Start()
        {
            // すでにこのアイテムを取得済みorエンカウント済みなら消す
            if (FieldItemsManager.instance.GetIsAcquired(gameObject.name) == true)
            {
                Destroy(gameObject);
            }
        }
        private void Save()
        {
            GameManager.instance.SaveAircraftPos(savePoint.transform.position);
        }

        private void GetItemOrEncount()
        {
            if (FieldItemsManager.instance.GetIsAcquired(gameObject.name) == false)
            {
                FieldItemsManager.instance.ItemGet(gameObject.name);
            }
        }

        private ThrowawayMethod method1 = new ThrowawayMethod();
        private ThrowawayMethod method2 = new ThrowawayMethod();
        private void OnTriggerEnter(Collider other)
        {
            method1.RunOnce(Save); 
            method2.RunOnce(GetItemOrEncount);
        }
    }
}