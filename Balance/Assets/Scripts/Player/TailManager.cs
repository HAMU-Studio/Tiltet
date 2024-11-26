using System;
using UnityEngine;

namespace Player
{
    public class TailManager : MonoBehaviour
    {
        private PlayerManager m_PM;
        private Vector3 m_def;
        
        [SerializeField] private Transform parent;

        private void Awake()
        {
            m_def = transform.localRotation.eulerAngles;
        }

        private void Start()
        {
            m_PM = GetComponentInParent<PlayerManager>();

        }
        private Vector3 tailDirection;
        private float tailVelocity = 0.0f;
        private void Update()
        { 
            if (m_PM.AnimState.IsName("Wire"))
            {
                Transform tail = transform;
                tailDirection = GameManager.instance.Pivot.transform.position - tail.position;
                float taleAngle = Mathf.Atan2(tailDirection.y, tailDirection.x) * Mathf.Rad2Deg;

               // Vector3 parentVec = parent.localRotation.eulerAngles;
                
               // transform.localRotation = Quaternion.Euler(m_def - parentVec);
                
                float taleCurrentAngle = Mathf.SmoothDampAngle(tail.rotation.eulerAngles.z, taleAngle, ref tailVelocity, 0.3f);

                tail.rotation = Quaternion.Euler(0.0f, 0.0f, taleCurrentAngle);
         
                //transform.Rotate(Vector3.back, 10f * Time.deltaTime); 
            }
        }

        private void LateUpdate()
        {
        
        }
    }
}