using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{

    [Header("Selección de equipo")]
    [SerializeField] private TMP_Dropdown teamDropdown;
    [SerializeField] private Image teamLogo;

    [Header("Botones")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;

    private DatabaseManager databaseManager;
    private Dictionary<string, Sprite> teamLogos;

    private void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        teamLogos = new Dictionary<string, Sprite>();

        LoadTeamsAndLogos();
        
        //Eventos
        teamDropdown.onValueChanged.AddListener(UpdateTeamLogo);
        startGameButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(BackToMainMenu);
    }

    private void LoadTeamsAndLogos() 
    {
        if (databaseManager == null)
        {
            return;
        }

        //Obtener los equipos desde la base de datos
        List<(int TeamId, string TeamName)> teams = databaseManager.ReadTeams();

        //Limpiar el dropdown y añadir una opción predeterminada
        teamDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var team in teams)
        {
            options.Add(team.TeamName);

            //Cargar el logo del equipo
            Sprite logo = Resources.Load<Sprite>($"TeamLogos/{team.TeamName}");
            if (logo != null)
            {
                teamLogos[team.TeamName] = logo;
            }
        }
        teamDropdown.AddOptions(options);
    }

    private void UpdateTeamLogo (int selectedIndex)
    {
        string selectedTeam = teamDropdown.options[selectedIndex].text;

        if (teamLogos.ContainsKey(selectedTeam))
        {
            teamLogo.sprite = teamLogos[selectedTeam];
        }
    }

    private void StartGame()
    {
        //Guardar las selecciones del jugador
        string selectedTeamName = teamDropdown.options[teamDropdown.value].text;
        PlayerPrefs.SetString("TeamName", selectedTeamName);

        //Cargar la siguiente escena
        SceneManager.LoadScene("GameScene");
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
