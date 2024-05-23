using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.GetInstance();
        gameManager.OnPlayerDied += OnPlayerDied;
        gameObject.SetActive(false);
    }

    private void OnPlayerDied(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
            
    }
}
