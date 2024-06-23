using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{

    public event EventHandler OnCoinCollected;
    private static CoinManager instance;


    //Please refactor this shit to have a scriptable objects for each level 
    private int cointsCount = 1;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public static CoinManager GetInstance()
    {
        return instance;
    }

    public void CollectCoin()
    {
        OnCoinCollected?.Invoke(this, EventArgs.Empty);
    }

    public int GetAllCoinsCount()
    {
        return cointsCount;
    }
}
