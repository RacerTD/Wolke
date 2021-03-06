﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : ManagerModule<GameManager>
{
    #region Player
    [SerializeField] private PlayerController playerPrefab;
    [HideInInspector] public PlayerController PlayerController;
    private PlayerSpawn playerStartSpawnPoint;
    private RespawnPoint playerRespawnPoint;
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
        if (PlayerController == null)
        {
            PlayerController = Instantiate(playerPrefab, playerStartSpawnPoint.transform.position, playerStartSpawnPoint.transform.rotation);
        }
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

    /// <summary>
    /// Sets the starting spawn point for the player
    /// </summary>
    /// <param name="playerSpawn"></param>
    public void SetStartSpawnPoint(PlayerSpawn playerSpawn)
    {
        if (playerStartSpawnPoint == null)
            playerStartSpawnPoint = playerSpawn;
        else
            Destroy(playerSpawn.gameObject);
    }

    public void SetRespawnPoint(RespawnPoint point)
    {
        playerRespawnPoint = point;
    }

    /// <summary>
    /// Resets the player to the spawn point
    /// </summary>
    public void ResetPlayerToSpawn()
    {
        PlayerController.transform.position = playerRespawnPoint.transform.position;
        PlayerController.transform.rotation = playerRespawnPoint.transform.rotation;
    }
}

public enum GameState
{
    Playing,
    Paused
}