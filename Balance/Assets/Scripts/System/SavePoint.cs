using UnityEngine;

namespace System
{
    public class SavePoint : MonoBehaviour
    {
        [SerializeField] private GameObject savePoint;
        private void Save()
        {
            GameManager.instance.SaveAircraftPos(savePoint.transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            Save();
        }
    }
}