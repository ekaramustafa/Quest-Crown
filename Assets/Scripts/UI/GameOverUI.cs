using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    private GameManager gameManager;

    [SerializeField] private Transform retryButton;
    [SerializeField] private Transform backButton;
    [SerializeField] private Transform exitButton;


    private void Start()
    {
        gameManager = GameManager.GetInstance();
        gameManager.OnPlayerDied += OnPlayerDied;
        gameObject.SetActive(false);

        retryButton.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.LoadCurrentLevel();
        };

        exitButton.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Application.Quit();
        };

        backButton.GetComponent<Button_UI>().ClickFunc = () =>
        {
            Loader.Load(Loader.Scene.LevelSelectorMenu);
        };
    }

    private void OnPlayerDied(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    
}
