using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ManagerModule<GameManager>
{
    #region Player
    public PlayerController PlayerController;
    #endregion
    #region CurrentGameState
    private GameState currentGameState = GameState.Playing;
    public GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
        set
        {
            currentGameState = value;
            UpdateGameState(value);
        }
    }

    /// <summary>
    /// Happens every time the GameState gets updated
    /// </summary>
    /// <param name="state"></param>
    private void UpdateGameState(GameState state)
    {
        Debug.Log($"GameState is now {currentGameState}");

        switch (state)
        {
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            default:
                break;
        }
    }
    #endregion

    private void Start()
    {

    }

    private void Update()
    {
        HandleGameState(currentGameState);
    }

    private void HandleGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                break;
            case GameState.Paused:
                break;
            default:
                break;
        }
    }
}

public enum GameState
{
    Playing,
    Paused
}