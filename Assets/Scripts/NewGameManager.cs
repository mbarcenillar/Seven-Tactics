using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NewGameManager : MonoBehaviour
{
    [Header("Configuracion del equipo")]
    [SerializeField] private InputField teamNameInput;
    [SerializeField] private InputField coachNameInput;
    [SerializeField] private InputField coachSurnameInput;
    [SerializeField] private InputField coachAgeInput;

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

        startGameButton.onClick.AddListener(StartGame);
        backButton.onClick.AddListener(BackToMainMenu);
        saveButton.onClick.AddListener(SaveGame);
        cancelButton.onClick.AddListener(CancelSaveGame);

        defaultColor = teamNameInput.image.color;
    }

    public void OnFieldSelected(InputField field)
    {
        field.image.color = selectedColor;
    }

    public void OnFieldDeselected(InputField field)
    {
        field.image.color = defaultColor;
    }

    private void StartGame()
    {
        bool allFieldsValid = true;

        //Validar campos vacios
        if (string.IsNullOrWhiteSpace(teamNameInput.text))
        {
            MarkFieldAsError(teamNameInput);
            allFieldsValid = false;
            Debug.Log("El nombre del equipo no puede estar vacio");
        }
        else
        {
            ResetFieldColor(teamNameInput);
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
        string teamName = teamNameInput.text;
        string coachName = coachNameInput.text;
        string coachSurname = coachSurnameInput.text;

        
        PlayerPrefs.SetString("TeamName", teamName);
        PlayerPrefs.SetString("CoachName", coachName);
        PlayerPrefs.SetString("CoachSurname", coachSurname);
        PlayerPrefs.SetInt("CoachAge", coachAge);

        //Guardar en la base de datos
        databaseManager.AddTeam(teamName);

        var team = databaseManager.GetTeamByName(teamName);
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

     private void MarkFieldAsError(InputField field)
    {
        field.image.color = errorColor;
    }

    private void ResetFieldColor(InputField field)
    {
        field.image.color = defaultColor;
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
