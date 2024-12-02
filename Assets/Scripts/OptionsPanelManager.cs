using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsPanelManager : MonoBehaviour
{
    [Header("Volumen")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeValueText;
    [SerializeField] private RectTransform handleRectTransform;
    
    [Header("Gráficos")]
    [SerializeField] private Button fullScreenButton;
    [SerializeField] private Button windowedButton;
    
    [Header("General")]
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button saveChangesButton;
    [SerializeField] private Button discardChangesButton;

    private bool isFullScreen = true;
    private int originalVolume;
    private bool originalFullScreen;
    
    private void Start()
    {
        //Cargar ajustes guardados
        LoadSettings();

        // Guardar los valores iniciales correctamente
        originalVolume = Mathf.RoundToInt(volumeSlider.value);
        originalFullScreen = isFullScreen;

        //Configurar los botones gráficos
        fullScreenButton.onClick.AddListener(SetFullScreen);
        windowedButton.onClick.AddListener(SetWindowed);

        //Configurar el botón guardar
        saveButton.onClick.AddListener(ShowConfirmationPanel);

        //Configurar el slider
        volumeSlider.onValueChanged.AddListener(value => UpdateVolumeDisplay(Mathf.RoundToInt(value)));
        UpdateVolumeDisplay(Mathf.RoundToInt(volumeSlider.value));

        //Configurar el panel de confirmación
        saveChangesButton.onClick.AddListener(SaveAndReturn);
        discardChangesButton.onClick.AddListener(DiscardAndReturn);

        // Suscribir eventos
        saveGameButton.onClick.AddListener(SaveGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void UpdateVolumeDisplay(int volume)
    {
        //Mover el texto debajo del Handle
        if (volumeValueText != null)
        {
            volumeValueText.text = volumeSlider.value.ToString();
        }
    }

    private void SetFullScreen()
    {
        isFullScreen = true;
        Screen.fullScreen = true;

         //Restaurar la resolución nativa si lo deseas
        int screenWidth = Screen.currentResolution.width;
        int screenHeight = Screen.currentResolution.height;
        Screen.SetResolution(screenWidth, screenHeight, true);

        Debug.Log("Modo pantalla completa activado.");
    }

    private void SetWindowed()
    {
        isFullScreen = false;
        Screen.fullScreen = false;

        //Cambiar la resolución
        int windowWidth = 1280;
        int windowHeight = 720;
        Screen.SetResolution(windowWidth, windowHeight, false);

        Debug.Log("Modo ventana activado.");
    }

    private void SaveSettings()
    {
        //Guardar el volumen
        int volumeValue = Mathf.RoundToInt(volumeSlider.value);
        PlayerPrefs.SetInt("Volume", volumeValue);

        //Guardar el modo pantalla
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);

        PlayerPrefs.Save();

        // Actualizar valores originales
        originalVolume = volumeValue;
        originalFullScreen = isFullScreen;
        
        Debug.Log("Ajustes guardados.");
    }

    private void LoadSettings()
    {
        //Cargar el volumen
        int savedVolume = PlayerPrefs.GetInt("Volume", 10);
        volumeSlider.value = savedVolume;

        //Cargar el modo pantalla
        isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        Screen.fullScreen = isFullScreen;

        Debug.Log($"Ajustes cargados: Volumen = {savedVolume}, FullScreen = {isFullScreen}");
    }

    private void ShowConfirmationPanel()
    {
         if (HasPendingChanges())
        {
            confirmationPanel.SetActive(true);
        }
        else
        {
            CloseConfirmationPanel();
        }
    }

    private void CloseConfirmationPanel()
    {
        confirmationPanel.SetActive(false);
    }

    private void SaveAndReturn()
    {
        SaveSettings();
        CloseConfirmationPanel();
    }

    private void SaveGame()
    {
        // Lógica para guardar la partida
        Debug.Log("Partida guardada.");
        // Aquí puedes llamar a tu método en `DatabaseManager` para guardar la partida
    }

    private void GoToMainMenu()
    {
        // Confirmación antes de salir (opcional)
        SceneManager.LoadScene("MainMenuScene");
    }

    private void DiscardAndReturn()
    {
        // Restaurar el volumen al valor original
        int savedVolume = PlayerPrefs.GetInt("Volume", 10);
        volumeSlider.value = savedVolume;

        // Restaurar el modo de pantalla al valor original
        bool savedFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        Screen.fullScreen = savedFullScreen;

        if (!savedFullScreen)
        {
            // Si el modo es ventana, ajustar la resolución original
            Screen.SetResolution(1280, 720, false);
        }
        else
        {
            // Restaurar la resolución de pantalla completa
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        CloseConfirmationPanel();
    }

    private bool HasPendingChanges()
    {
        int currentVolume = Mathf.RoundToInt(volumeSlider.value);
        return currentVolume != originalVolume || isFullScreen != originalFullScreen;
    }


}
