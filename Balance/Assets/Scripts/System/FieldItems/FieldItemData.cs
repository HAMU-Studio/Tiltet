using UnityEngine;

namespace System
{
    public class FieldItemData : MonoBehaviour
    {
        public GameObject[] fieldItems;

        private void Start()
        {
            if (GameManager.instance != null)
                GameManager.instance.OnInitGame += Init;

            if (GameManager.instance.isConnected == false &&
                     GameManager.instance.isSkip == false)
            {
                GameManager.instance.InitGame(false);
            }
        }

        private void OnDisable()
        {
            if (GameManager.instance != null)
                GameManager.instance.OnInitGame -= Init;
        }

        private void Init()
        {
            Debug.Log("Item acquisition status has been reset");
            FieldItemsManager.instance.Initialize(fieldItems);
        }
    }
}