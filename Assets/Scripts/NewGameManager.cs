using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    [Header("Configuracion del equipo")]
    [SerializeField] private InputField coachNameInput;
    [SerializeField] private InputField coachSurnameInput;
    [SerializeField] private InputField coachAgeInput;

    [Header("Botones")]
    [SerializeField] private TMP_Dropdown teamDropdown;

    [Header("Botones")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button backButton;

    [Header("Panel de guardar")]
    [SerializeField] private GameObject saveGamePanel;
    [SerializeField] private InputField saveGameInputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    private DatabaseManager databaseManager;

    private Color defaultColor = Color.white;
    private Color errorColor = new Color(0.9647f, 0.6039f, 0.6039f);
    private Color selectedColor = new Color(0.8f, 0.8f, 1f);

    private void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();

        LoadTeamsIntoDropdown();

        startGameButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(BackToMainMenu);
        saveButton.onClick.AddListener(SaveGame);
        cancelButton.onClick.AddListener(CancelSaveGame);

    }

    public void OnFieldSelected(InputField field)
    {
        field.image.color = selectedColor;
    }

    public void OnFieldDeselected(InputField field)
    {
        field.image.color = defaultColor;
    }

    private void LoadTeamsIntoDropdown() 
    {
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager no encontrado.");
            return;
        }

        // Obtener los equipos desde la base de datos
        List<(int TeamId, string TeamName)> teams = databaseManager.ReadTeams();

        // Limpiar el dropdown y añadir una opción predeterminada
        teamDropdown.ClearOptions();
        List<string> options = new List<string> { "Selecciona tu equipo" };

        foreach (var team in teams)
        {
            options.Add(team.TeamName);
        }

        teamDropdown.AddOptions(options);
        Debug.Log("Equipos cargados en el desplegable.");
    }

    private void StartGame()
    {
        bool allFieldsValid = true;

        //Validar campos vacios
        if (teamDropdown.value == 0) // La primera opción es "Selecciona tu equipo"
        {
            MarkFieldAsError(teamDropdown);
            allFieldsValid = false;
            Debug.LogWarning("Debes seleccionar un equipo.");
        }
        else
        {
            ResetFieldColor(teamDropdown);
        }
        if (string.IsNullOrWhiteSpace(coachNameInput.text))
        {
            MarkFieldAsError(coachNameInput);
            allFieldsValid = false;
            Debug.Log("El nombre del entrenador no puede estar vacio");
        }
        else
        {
            ResetFieldColor(coachNameInput);
        }
        if (string.IsNullOrWhiteSpace(coachSurnameInput.text))
        {
            MarkFieldAsError(coachSurnameInput);
            allFieldsValid = false;
            Debug.Log("El apellido del entrenador no puede estar vacio");
        }
        else
        {
            ResetFieldColor(coachSurnameInput);
        }
        string coachAgeText = coachAgeInput.text.Trim();
        if (!int.TryParse(coachAgeText, out int coachAge) || coachAge <= 0)
        {
            MarkFieldAsError(coachAgeInput);
            allFieldsValid = false;
            Debug.LogWarning("La edad del entrenador debe ser un número válido mayor a 0.");
        }
        else
        {
            ResetFieldColor(coachAgeInput);
        }
        if (!allFieldsValid)
        {
            Debug.LogWarning("Algunos campos no están rellenos correctamente.");
            return;
        }

        //Guardar las selecciones del jugador
        string selectedTeamName = teamDropdown.options[teamDropdown.value].text;
        string coachName = coachNameInput.text;
        string coachSurname = coachSurnameInput.text;

        
        PlayerPrefs.SetString("TeamName", selectedTeamName);
        PlayerPrefs.SetString("CoachName", coachName);
        PlayerPrefs.SetString("CoachSurname", coachSurname);
        PlayerPrefs.SetInt("CoachAge", coachAge);

        //Guardar en la base de datos

        var team = databaseManager.GetTeamByName(selectedTeamName);
        int teamId = team.Value.TeamId;

        databaseManager.AddCoach(coachName, coachSurname, coachAge, teamId);

        Debug.Log("Equipo y entrenador guardados en la base de datos.");

        //Cargar la siguiente escena
        saveGamePanel.SetActive(true);
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

     private void MarkFieldAsError(Selectable field)
    {
        if (field is InputField inputField)
        {
            inputField.image.color = errorColor;
        }
        else if (field is TMP_Dropdown dropdown)
        {
            dropdown.GetComponent<Image>().color = errorColor;
        }
    }

    private void ResetFieldColor(Selectable field)
    {
        if (field is InputField inputField)
        {
            inputField.image.color = defaultColor;
        }
        else if (field is TMP_Dropdown dropdown)
        {
            dropdown.GetComponent<Image>().color = defaultColor;
        }
    }

    private void SaveGame()
    {
        if (string.IsNullOrWhiteSpace(saveGameInputField.text))
        {
            Debug.LogWarning("El nombre de la partida no puede estar vacío.");
            return;
        }

        string saveGameName = saveGameInputField.text;

        // Guardar en la base de datos
        databaseManager.AddSavedGame(saveGameName);

        Debug.Log($"Partida guardada: {saveGameName}");

        // Proceder al juego
        SceneManager.LoadScene("GameScene");
    }

    private void CancelSaveGame()
    {
        saveGamePanel.SetActive(false);
        Debug.Log("Guardar partida cancelado. Volviendo a la pantalla principal.");
    }
}
