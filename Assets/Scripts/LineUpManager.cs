using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineUpManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown tacticDropdown;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private Transform tacticDisplayContainer;
    [SerializeField] private GameObject PlayerTacticDropdownPrefab;
    [SerializeField] private GameObject positionHeaderPrefab;

    private Dictionary<string, int[]> tactics = new Dictionary<string, int[]>() 
    { 
        { "1-3-2", new int[] { 1, 1, 3, 2 } },
        { "1-4-1", new int[] { 1, 1, 4, 1 } },
        { "2-2-2", new int[] { 1, 2, 2, 2 } },
        { "2-3-1", new int[] { 1, 2, 3, 1 } },
        { "3-2-1", new int[] { 1, 3, 2, 1 } }

    };
    private List<(string Name, string Position)> players = new List<(string, string)>
    {
        ("Juan Pérez", "Portero"),
        ("Carlos Gómez", "Defensa"),
        ("Luis Rodríguez", "Defensa"),
        ("Pedro Sánchez", "Defensa"),
        ("Miguel Torres", "Defensa"),
        ("Miguel Rodríguez", "Defensa"),
        ("Carlos Torres", "Defensa"),
        ("Andrés López", "Centrocampista"),
        ("Fernando Díaz", "Centrocampista"),
        ("Jorge Ramírez", "Centrocampista"),
        ("Samuel Martín", "Centrocampista"),
        ("Samuel Ramírez", "Centrocampista"),
        ("Andrés Martín", "Centrocampista"),
        ("Diego Morales", "Delantero"),
        ("Samuel Morales", "Delantero"),
        ("Diego Pérez", "Delantero"),
        ("Alejandro Díaz", "Delantero")
    };

    private void Start()
    {
        // Configurar las opciones del dropdown de tácticas
        tacticDropdown.ClearOptions();
        tacticDropdown.AddOptions(new List<string>(tactics.Keys));

        // Configurar el evento de cambio de táctica
        tacticDropdown.onValueChanged.AddListener(UpdatePositionFields);

        // Mostrar la táctica inicial
        UpdatePositionFields(0);

        // Cargar la lista de jugadores
        LoadPlayerList();
    }

    private void LoadPlayerList()
    {
        // Limpiar lista actual
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        // Añadir jugadores a la lista
        foreach (var (name, position) in players)
        {
            GameObject newItem = Instantiate(playerItemPrefab, playerListContainer);
            TMP_Text text = newItem.GetComponent<TMP_Text>();
            text.text = $"{name} - {position}";
        }
    }

    private void UpdatePositionFields(int tacticIndex)
    {
        // Limpiar los campos actuales
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in tacticDisplayContainer)
        {
            children.Add(child.gameObject);
        }

        // Destruir los hijos de la lista
        foreach (GameObject child in children)
        {
            Destroy(child);
        }

        // Obtener la táctica seleccionada
    string selectedTactic = tacticDropdown.options[tacticIndex].text;
    if (!tactics.TryGetValue(selectedTactic, out int[] formation))
    {
        Debug.LogWarning("Táctica no encontrada.");
        return;
    }

    // Crear encabezados y campos dinámicamente
    string[] positions = { "Portero", "Defensa", "Centrocampista", "Delantero" };
    for (int i = 0; i < formation.Length; i++)
    {
        // Crear encabezado
        GameObject headerObject = Instantiate(positionHeaderPrefab, tacticDisplayContainer);
        TMP_Text headerText = headerObject.GetComponent<TMP_Text>();
        headerText.text = positions[i].ToUpper();

        // Crear desplegables para cada posición
        for (int j = 0; j < formation[i]; j++)
        {
            GameObject dropdownObject = Instantiate(PlayerTacticDropdownPrefab, tacticDisplayContainer);

            // Configurar las opciones del desplegable
            TMP_Dropdown dropdown = dropdownObject.GetComponent<TMP_Dropdown>();
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(GetPlayersForPosition(positions[i]));
            }
        }
    }
    }

    private List<string> GetPlayersForPosition(string position)
    {
        List<string> playerOptions = new List<string>();
        foreach (var player in players)
        {
            if (player.Position == position)
            {
                playerOptions.Add(player.Name);
            }
        }
        return playerOptions;
    }
}
