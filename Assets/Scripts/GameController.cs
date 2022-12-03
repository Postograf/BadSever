using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public enum GameResult
{
    Win,
    Lose
}

public class GameController : MonoBehaviour
{
    [SerializeField] private List<ObjectsController> _winObjects;
    [SerializeField] private List<ObjectsController> _loseObjects;
    [SerializeField] private Island _island;

    private bool _gameEnded;

    public event Action<GameResult> GameEnded;
    public event Action GameStarted;

    private void Start()
    {
        _gameEnded = false;

        foreach (var obj in _winObjects)
            obj.AllObjectsDead += () => EndGame(GameResult.Lose);

        foreach (var obj in _loseObjects)
            obj.AllObjectsDead += () => EndGame(GameResult.Win);
    }

    public void StartGame()
    {
        _island.gameObject.SetActive(true);
        GameStarted?.Invoke();
    }

    private void EndGame(GameResult gameResult)
    {
        if (_gameEnded == false)
        {
            GameEnded?.Invoke(gameResult);
            _gameEnded = true;
        }
    }

    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    public void RefreshAllObjects()
    {
        foreach (var obj in _winObjects.Concat(_loseObjects))
            obj.Refresh();

        _gameEnded = false;
    }
}
