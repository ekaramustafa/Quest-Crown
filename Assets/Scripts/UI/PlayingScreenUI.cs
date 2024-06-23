using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingScreenUI : MonoBehaviour
{
    //cache vars
    private CoinManager coinManager;

    [SerializeField]private TextMeshProUGUI coinText;
    [SerializeField]private Image coinImage;
    private int collectedCoinCount;

    private void Awake()
    {
        collectedCoinCount = 0;
        coinImage.color = new Color(coinImage.color.r, coinImage.color.b, coinImage.color.g, 0.5f);
    }

    private void Start()
    {
        coinManager = CoinManager.GetInstance();
        coinManager.OnCoinCollected += OnCoinCollected;
    }

    private void OnCoinCollected(object sender, System.EventArgs e)
    {
        collectedCoinCount++;
        coinText.SetText(collectedCoinCount.ToString()+"/"+coinManager.GetAllCoinsCount());
        coinImage.color = new Color(coinImage.color.r, coinImage.color.b, coinImage.color.g, 1f);
    }
}
