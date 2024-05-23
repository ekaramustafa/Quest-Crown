using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    private GameManager gameManager;

    [SerializeField] private Transform retryButton;

    private void Start()
    {
        gameManager = GameManager.GetInstance();
        gameManager.OnPlayerDied += OnPlayerDied;
        gameObject.SetActive(false);

        retryButton.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.GameScene);
        };
    }

    private void OnPlayerDied(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    
}
