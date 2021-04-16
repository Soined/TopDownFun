using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Main;

    private void Awake()
    {
        if(Main != null && Main != this)
        {
            Destroy(this);
        } else Main = this;
    }
    #endregion

    private void Start()
    {
        ChangeGameState(GameState.Playing);
    }

    /// <summary>
    /// Change this by using ChangeGameState(GameState newGameState)
    /// </summary>
    public GameState GameState { get => gameState; }

    private GameState gameState = GameState.Playing;

    public void ChangeGameState(GameState newGameState)
    {
        switch(newGameState)
        {
            case GameState.Playing:
                UIManager.Main.ChangeUIState(UIManager.State.HUD);
                Time.timeScale = 1;
                break;
            case GameState.Pause:
                UIManager.Main.ChangeUIState(UIManager.State.Pause);
                Time.timeScale = 0;
                break;
        }
        gameState = newGameState;
    }

    public void OnPause()
    {
        if(gameState == GameState.Playing)
        {
            ChangeGameState(GameState.Pause);
        } else if(gameState == GameState.Pause)
        {
            ChangeGameState(GameState.Playing);
        }
    }
}

public enum GameState
{
    Playing,
    Pause
}