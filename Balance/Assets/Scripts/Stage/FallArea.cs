using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallArea : MonoBehaviour
{
    [SerializeField] private GameObject respawnPos;

    //DestroyAreaに触れたら消える
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = respawnPos.transform.position;
        }
    }
}
