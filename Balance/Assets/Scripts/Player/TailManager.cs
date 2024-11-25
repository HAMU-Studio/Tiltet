using System;
using UnityEngine;

namespace Player
{
    public class TailManager : MonoBehaviour
    {
        private PlayerManager m_PM;

        private void Start()
        {
            m_PM = GetComponentInParent<PlayerManager>();

        }

        private void Update()
        {
            /*if (m_PM.AnimState.IsName("Wire"))
            {
                Debug.Log("a");
                transform.Rotate(Vector3.back, 10f * Time.deltaTime); 
            }*/
        }

        private void LateUpdate()
        {
        
        }
    }
}