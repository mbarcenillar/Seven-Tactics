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

    private int selectedGameId = -1; // ID de la partida seleccionada
    private DatabaseManager databaseManager;
    private GameObject currentSelectedItem;

    private IEnumerator Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();

        if (databaseManager == null)
        {
            Debug.LogError("No se encontró DatabaseManager en la escena.");
            yield break;
        }

        // Espera hasta que DatabaseManager esté listo
        while (!databaseManager.IsReady)
        {
            yield return null;
        }

        loadGameButton.onClick.AddListener(LoadSelectedGame);
        backButton.onClick.AddListener(BackToMainMenu);
        deleteGameButton.onClick.AddListener(DeleteSelectedGame);

        //Desactivar los botones por defecto
        loadGameButton.interactable = false;
        deleteGameButton.interactable = false;

        LoadSavedGames();
    }

    private void LoadSavedGames()
    {
		// Eliminar partidas listadas previamente (si las hay)
        foreach (Transform child in savedGamesContainer)
        {
            Destroy(child.gameObject);
        }

        // Obtener todas las partidas guardadas desde la base de datos
        var savedGames = databaseManager.GetAllSavedGames();

        foreach (var (gameId, name) in savedGames)
        {
            // Crear un nuevo elemento en la lista
            GameObject newItem = Instantiate(savedGameButtonPrefab, savedGamesContainer);

            // Configurar el texto del nombre de la partida
            var nameText = newItem.transform.Find("TextButton/NameText").GetComponent<TMP_Text>();
            nameText.text = name;

            // Hacer el texto clicable
            var mainButton = newItem.transform.Find("TextButton")?.GetComponent<Button>();
            if (mainButton != null)
            {
            mainButton.onClick.AddListener(() => SelectGame(gameId, newItem));
            }
        }
    }

    private void SelectGame(int gameId, GameObject selectedItem)
    {
        // Desmarcar el elemento seleccionado previamente
        if (currentSelectedItem != null)
        {
            ResetItemStyle(currentSelectedItem);
        }

        // Actualizar el nuevo elemento seleccionado
        currentSelectedItem = selectedItem;
        selectedGameId = gameId; // Guardar el ID de la partida seleccionada

        // Cambiar el estilo visual del elemento seleccionado
        HighlightItemStyle(currentSelectedItem);

        Debug.Log($"Partida seleccionada: {gameId}");

        //Activar los botones
        loadGameButton.interactable = true;
        deleteGameButton.interactable = true;
    }

    private void LoadSelectedGame()
    {
        if (selectedGameId == -1)
        {
            Debug.LogWarning("No se ha seleccionado ninguna partida.");
            return;
        }

        var gameName = databaseManager.LoadGameById(selectedGameId);
        if (gameName != null)
        {
            Debug.Log($"Cargando partida: {gameName}");
            // Cargar la escena principal del juego
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("Error al cargar la partida.");
        }
    }

    private void DeleteSelectedGame() 
    {
        // Eliminar la partida de la base de datos
        databaseManager.DeleteSavedGame(selectedGameId);
        Debug.Log($"Partida con ID {selectedGameId} eliminada.");

        // Recargar la lista de partidas
        LoadSavedGames();

        // Reiniciar el estado
        selectedGameId = -1;
        loadGameButton.interactable = false;
        deleteGameButton.interactable = false;
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void HighlightItemStyle(GameObject item)
    {
        // Cambiar el fondo del elemento
        var textButtonImage = item.transform.Find("TextButton")?.GetComponent<Image>();
        if (textButtonImage != null)
        {
            textButtonImage.color = new Color32(0, 122, 204, 255); // Azul claro, por ejemplo
        }

        // Cambiar el color del texto
        var nameText = item.transform.Find("TextButton/NameText")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            nameText.color = Color.white; // Cambia a blanco, por ejemplo
        }
    }

    private void ResetItemStyle(GameObject item)
    {
        // Restaurar el fondo del elemento
        var textButtonImage = item.transform.Find("TextButton")?.GetComponent<Image>();
        if (textButtonImage != null)
        {
            textButtonImage.color = new Color32(0, 0, 0, 255); // Negro
        }

        // Restaurar el color del texto
        var nameText = item.transform.Find("TextButton/NameText")?.GetComponent<TMP_Text>();
        if (nameText != null)
        {
            nameText.color = Color.red; // Rojo, por ejemplo
        }
    }
}
