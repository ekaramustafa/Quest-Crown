using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    [SerializeField] private PolygonCollider2D confinerCollider;
    public int PlayerCount { get; set; }


    public event EventHandler OnPlayerDied;


    public enum GameState
    {
        PLAYING,
        GAMEOVER
    }

    public GameState gamestate;

    public static GameManager GetInstance()
    {
        return instance;
    }


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        gamestate = GameState.PLAYING;
    }

    public PolygonCollider2D GetConfinerCollider()
    {
        return confinerCollider;
    }


    public GameState GetGameState()
    {
        return gamestate;
    }

    public void ChangeStateTo(GameState state)
    {
        gamestate = state;
        if(state == GameState.GAMEOVER)
        {
            OnPlayerDied?.Invoke(this, EventArgs.Empty);
        }
    }




}
