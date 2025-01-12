using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineUpManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown tacticsDropdown;
    [SerializeField] private Transform formationContainer;
    [SerializeField] private ScrollRect playersListScrollView;
    [SerializeField] private GameObject positionItemPrefab;
    [SerializeField] private GameObject playerItemPrefab;

    private DatabaseManager databaseManager;

    //Diccionario para almacenar jugadores según la posición
    private Dictionary<string, List<(string name, string surname, int attack, int defense, int center, int goalkeeper, int stamina)>> teamPlayers;

    private FormationData currentFormation = new FormationData();

    private List<string> tactics = new List<string> {"1-3-2", "1-4-1", "2-2-2", "2-3-1", "3-2-1"};
    private Dictionary<string, int[]> positionsCount  = new Dictionary<string, int[]>() 
    { 
        { "1-3-2", new int[] { 1, 1, 3, 2 } },
        { "1-4-1", new int[] { 1, 1, 4, 1 } },
        { "2-2-2", new int[] { 1, 2, 2, 2 } },
        { "2-3-1", new int[] { 1, 2, 3, 1 } },
        { "3-2-1", new int[] { 1, 3, 2, 1 } }

    };
    private Dictionary<string, string> selectedPlayers = new Dictionary<string, string>();

    private void Start()
    {

        databaseManager = FindObjectOfType<DatabaseManager>();

        if (databaseManager == null)
        {
            return;
        }

        LoadPlayers();

        //Configurar las tácticas
        tacticsDropdown.ClearOptions();
        tacticsDropdown.AddOptions(tactics);
        tacticsDropdown.onValueChanged.AddListener(UpdateFormation);
        UpdateFormation(tacticsDropdown.value);

        
    }

    private void LoadPlayers()
    {
        //Obtener jugadores desde la base de datos
        string teamName = PlayerPrefs.GetString("TeamName", "Equipo Desconocido");
        var players = databaseManager.GetPlayersByTeamName(teamName);

        //Diccionario para organizar jugadores por posición
        teamPlayers = new Dictionary<string, List<(string name, string surname, int attack, int defense, int center, int goalkeeper, int stamina)>>();

        foreach (var player in players)
        {
            //Puedes usar lógica adicional para clasificar jugadores según sus habilidades
            string position = DeterminePosition(player); // Por ahora, asignamos todos como "General"
            if (!teamPlayers.ContainsKey(position))
            {
                teamPlayers[position] = new List<(string, string, int, int, int, int, int)>();
            }

            teamPlayers[position].Add((player.name, player.surname, player.attack, player.defense, player.center, player.goalkeeper, player.stamina));
        }

        PopulatePlayersList(players);
    }

     private string DeterminePosition((string name, string surname, int attack, int defense, int center, int goalkeeper, int stamina) player)
    {
        //Lógica básica para determinar la posición según las habilidades
        if (player.goalkeeper > 50) return "Portero";
        if (player.defense > player.attack) return "Defensa";
        if (player.center > player.defense) return "Mediocampista";
        return "Delantero";
    }

    private void PopulatePlayersList(List<(string name, string surname, int attack, int defense, int center, int goalkeeper, int stamina)> players)
    {
        var content = playersListScrollView.content;
        foreach (Transform child in formationContainer)
        {
            Debug.Log($"Eliminando: {child.name}");
            Destroy(child.gameObject);
        }

        foreach (var player in players)
        {
            GameObject newItem = Instantiate(playerItemPrefab, content);
            TMP_Text[] texts = newItem.GetComponentsInChildren<TMP_Text>();

            texts[0].text = $"{player.name} {player.surname}";
            texts[1].text = $"{player.attack}";
            texts[2].text = $"{player.defense}";
            texts[3].text = $"{player.center}";
            texts[4].text = $"{player.goalkeeper}";
            texts[5].text = $"{player.stamina}";
        }
    }

    private void UpdateFormation(int tacticIndex)
    {
        //Limpiar formación anterior
        
        foreach (Transform child in formationContainer)
        {
            Debug.Log($"Eliminando: {child.name}");
            Destroy(child.gameObject);
        }
        
        
        //Configurar la formación según la táctica seleccionada
        string selectedTactic = tacticsDropdown.options[tacticIndex].text;

        if (!positionsCount.ContainsKey(selectedTactic))
        {
            return;
        }

        int[] positionCounts = positionsCount[selectedTactic];

        //Generar posiciones
        for (int i = 0; i < positionCounts.Length; i++)
        {
            int count = positionCounts[i];
            string positionName = GetPositionName(i);

            for (int j = 0; j < count; j++)
            {
                GameObject newPosition = Instantiate(positionItemPrefab, formationContainer);
                newPosition.SetActive(true);

                //Asignar un nombre único al objeto
                string uniqueName = $"{positionName} {j + 1}";
                newPosition.name = uniqueName;

                //Configurar el texto o dropdown de la posición
                TMP_Text positionText = newPosition.GetComponentInChildren<TMP_Text>();
                if (positionText != null)
                {
                    positionText.text = $"{positionName} {j + 1}";
                }

                TMP_Dropdown dropdown = newPosition.GetComponentInChildren<TMP_Dropdown>();
                if (dropdown != null)
                {
                    PopulateDropdownWithPlayers(dropdown);
                }
            }
        }
    }

    private string GetPositionName(int index)
    {
        switch (index)
        {
            case 0: return "Portero";
            case 1: return "Defensa";
            case 2: return "Medio";
            case 3: return "Delantero";
            default: return "Posición";
        }
    }

    private void PopulateDropdownWithPlayers(TMP_Dropdown dropdown)
{
    //Limpia las opciones actuales del dropdown
    dropdown.ClearOptions();

    //Genera la lista de opciones con el nombre completo de los jugadores
    List<string> playerOptions = new List<string>();
    
    foreach (var kvp in teamPlayers)
    {
        foreach (var player in kvp.Value)
        {
            string fullName = $"{player.name} {player.surname}";
            playerOptions.Add(fullName);
        }
    }

    //Agrega las opciones al dropdown
    dropdown.AddOptions(playerOptions);
}

public void SaveFormationAndPlayers()
{
    //Crear un nuevo objeto FormationData
    FormationData formationData = new FormationData
    {
        formationName = tacticsDropdown.options[tacticsDropdown.value].text,
        playersByPosition = new List<PositionPlayer>()
    };

    //Recorrer los elementos en formationContainer para obtener las posiciones y los jugadores seleccionados
    foreach (Transform positionTransform in formationContainer)
    {
        TMP_Text positionText = positionTransform.GetComponentInChildren<TMP_Text>();
        TMP_Dropdown playerDropdown = positionTransform.GetComponentInChildren<TMP_Dropdown>();

        if (positionText != null && playerDropdown != null)
        {
            string position = positionText.text;
            string selectedPlayer = playerDropdown.options[playerDropdown.value].text;

            //formationData.playersByPosition[position] = selectedPlayer;
            formationData.playersByPosition.Add(new PositionPlayer
            {
                position = position,
                player = selectedPlayer
            });
        }
    }

    //Convertir a JSON y guardar en PlayerPrefs
    string json = JsonUtility.ToJson(formationData);
    PlayerPrefs.SetString("SavedFormation", json);
    PlayerPrefs.Save();
}
}
