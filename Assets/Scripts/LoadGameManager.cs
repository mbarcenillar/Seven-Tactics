using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject savedGameButtonPrefab;
    [SerializeField] private Transform savedGamesContainer;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button deleteGameButton;

    private void Start()
    {
        backButton.onClick.AddListener(BackToMainMenu);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
