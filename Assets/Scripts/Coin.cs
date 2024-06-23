using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    private CoinManager coinManager;
    [SerializeField] private LayerMask playerLayerMask;

    private void Start()
    {
        coinManager = CoinManager.GetInstance();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            coinManager.CollectCoin();
            Destroy(this.gameObject);
        }
    }


}
