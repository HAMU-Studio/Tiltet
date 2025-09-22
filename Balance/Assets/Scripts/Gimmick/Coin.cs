using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public event Action OnGetCoin;
   
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Ground"))
        {
            OnGetCoin?.Invoke();
        }
    }

    public void DestroyCoin() => Destroy(gameObject);

}
