using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Vertical : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private Transform m_player;
    private Ray m_ray;
    private RaycastHit m_hit;
    private Quaternion m_rot;
    void Update()
    {
        SetRotation();
    }

    private void SetRotation()
    {
        //プレイヤーの真下方向にRayを飛ばす
        m_ray = new Ray(m_player.position, -transform.up);
        Physics.Raycast(m_ray, out m_hit, 2);
        
        m_rot = Quaternion.FromToRotation(transform.up, m_hit.normal);
        
        GetComponent<Rigidbody>().MoveRotation(m_rot * transform.rotation);
    }
}
