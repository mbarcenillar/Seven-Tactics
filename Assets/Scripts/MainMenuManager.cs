using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TMP_Text newGameButton;
    [SerializeField] private TMP_Text loadGameButton;
    [SerializeField] private TMP_Text settingsButton;
    [SerializeField] private TMP_Text exitButton;

    private string newGameText;

    private void Start()
    {
        if (newGameButton == null)
        {
            newGameButton = GetComponentInChildren<TMP_Text>();
        }

        newGameText = newGameButton.text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        newGameButton.text = $"<u>{newGameText}</u>";
        newGameButton.ForceMeshUpdate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        newGameButton.text = newGameText;
        newGameButton.ForceMeshUpdate();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("NewGameScene");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("LoadGameScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    }

    public void ShowQuitConfirmation()
    {
        confirmationPanel.SetActive(true);
    }

    public void HideQuitConfirmation()
    {
        confirmationPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
