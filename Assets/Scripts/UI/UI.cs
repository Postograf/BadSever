using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

public enum ButtonsSet
{
    Start,
    InGame
} 

public class UI : MonoBehaviour
{
    [SerializeField] private GameController _gameController;

    [Header("Menu")]
    [SerializeField] private GameObject _menu;
    [SerializeField] private Button _start;
    [SerializeField] private Button _continue;
    [SerializeField] private Button _restart;
    [SerializeField] private Button _exit;
    [SerializeField] private TMP_Text _gameResult;
    [SerializeField] private string _winningText;
    [SerializeField] private string _losingText;

    [Header("HUD")]
    [SerializeField] private GameObject _hud;
    [SerializeField] private Button _pause;

    private void Start()
    {
        SwitchPageTo(_menu);
        _gameResult.gameObject.SetActive(false);

        _start.onClick.AddListener(OnStart);
        _continue.onClick.AddListener(OnContinue);
        _restart.onClick.AddListener(OnRestart);
        _exit.onClick.AddListener(OnExit);

        ChangeButtonsSet(ButtonsSet.Start);

        _pause.onClick.AddListener(OnPause);

        _gameController.GameEnded += OnGameEnd;
    }

    private void OnStart()
    {
        ChangeButtonsSet(ButtonsSet.InGame);
        _gameResult.gameObject.SetActive(false);

        _gameController.StartGame();
        SwitchPageTo(_hud);
    }

    private void OnPause()
    {
        _gameController.Pause(true);

        SwitchPageTo(_menu);
    }

    private void OnContinue()
    {
        _gameController.Pause(false);

        SwitchPageTo(_hud);
    }

    private void OnRestart()
    {
        _gameResult.gameObject.SetActive(false);
        _gameController.RefreshAllObjects();

        SwitchPageTo(_hud);
    }

    private void OnExit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif
    }

    private void OnGameEnd(GameResult result)
    {
        SwitchPageTo(_menu);

        _gameResult.gameObject.SetActive(true);
        _gameResult.text = result == GameResult.Win ? _winningText : _losingText;
    }

    private void SwitchPageTo(GameObject page)
    {
        _gameController.Pause(page == _menu);

        _hud.SetActive(_hud == page);
        _menu.SetActive(_menu == page);
    }

    private void ChangeButtonsSet(ButtonsSet buttonsSet)
    {
        _start.gameObject.SetActive(buttonsSet == ButtonsSet.Start);
        _continue.gameObject.SetActive(buttonsSet == ButtonsSet.InGame);
        _restart.gameObject.SetActive(buttonsSet == ButtonsSet.InGame);
    }
}
