using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text teamNameText;
    [SerializeField] private Image teamLogo;

    [Header("Panels")]
    [SerializeField] private GameObject lineupPanel;
    [SerializeField] private GameObject standingsPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons")]
    [SerializeField] private Button lineupButton;
    [SerializeField] private Button standingsButton;
    [SerializeField] private Button matchButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private Dictionary<string, string> loadedFormation = new Dictionary<string, string>();
    private string selectedFormation;
    private Dictionary<string, Sprite> teamLogos;

     private void Start()
    {
        //Configurar el nombre y logo del equipo
        string teamName = PlayerPrefs.GetString("TeamName", "Equipo Desconocido");
        teamNameText.text = "Equipo: " + teamName;

        teamLogos = new Dictionary<string, Sprite>();

        LoadLogo();

        //Configurar botones
        lineupButton.onClick.AddListener(() => ShowPanel(lineupPanel));
        standingsButton.onClick.AddListener(() => ShowPanel(standingsPanel));
        statsButton.onClick.AddListener(() => ShowPanel(statsPanel));
        optionsButton.onClick.AddListener(() => ShowPanel(optionsPanel));
        matchButton.onClick.AddListener(OpenMatchScene);
        exitButton.onClick.AddListener(GoToMainMenu);

        //Iniciar mostrando un panel vacío
        ShowPanel(null);

        //Cargar formación desde PlayerPrefs
        selectedFormation = PlayerPrefs.GetString("SelectedFormation", null);
        if (string.IsNullOrEmpty(selectedFormation))
        {
            Debug.LogWarning("No se encontró una formación guardada.");
            return;
        }

        Debug.Log($"Formación cargada: {selectedFormation}");

        //Cargar jugadores por posición desde PlayerPrefs
        loadedFormation.Clear();
        foreach (string position in GetPositionsForFormation(selectedFormation))
        {
            string playerName = PlayerPrefs.GetString($"Position_{position}", null);
            if (!string.IsNullOrEmpty(playerName))
            {
                loadedFormation[position] = playerName;
                Debug.Log($"Posición: {position}, Jugador: {playerName}");
            }
        }
    }

    private void LoadLogo() 
    {
        string teamName = PlayerPrefs.GetString("TeamName", "Equipo Desconocido");
            //Cargar el logo del equipo
            Sprite logo = Resources.Load<Sprite>($"TeamLogos/{teamName}");
            if (logo != null)
            {
                teamLogos[teamName] = logo;
            }
    }

    private void ShowPanel(GameObject panelToShow)
    {
        //Ocultar todos los paneles
        lineupPanel.SetActive(false);
        standingsPanel.SetActive(false);
        statsPanel.SetActive(false);
        optionsPanel.SetActive(false);

         //Activar el panel solicitado
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    private List<string> GetPositionsForFormation(string formation)
    {
        //Define las posiciones según la formación
        Dictionary<string, List<string>> formationPositions = new Dictionary<string, List<string>>
        {
            { "1-3-2", new List<string> { "Portero", "Defensa 1", "Defensa 2", "Defensa 3", "Medio 1", "Medio 2" } },
            { "1-4-1", new List<string> { "Portero", "Defensa 1", "Defensa 2", "Defensa 3", "Defensa 4", "Medio" } },
            { "2-2-2", new List<string> { "Portero", "Defensa 1", "Defensa 2", "Medio 1", "Medio 2", "Delantero 1", "Delantero 2" } },
            { "2-3-1", new List<string> { "Portero", "Defensa 1", "Defensa 2", "Medio 1", "Medio 2", "Medio 3", "Delantero" } },
            { "3-2-1", new List<string> { "Portero", "Defensa 1", "Defensa 2", "Defensa 3", "Medio 1", "Medio 2", "Delantero" } }
        };

        return formationPositions.ContainsKey(formation) ? formationPositions[formation] : new List<string>();
    }

    private void OpenMatchScene()
    {
        SceneManager.LoadScene("MatchScene");
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
