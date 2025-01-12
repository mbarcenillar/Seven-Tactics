using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private ScrollRect eventLogScrollView;
    [SerializeField] private GameObject eventItemPrefab;
    [SerializeField] private Button pauseButton;

    [Header("Marcador")]
    [SerializeField] private TMP_Text myTeamNameText;
    [SerializeField] private TMP_Text rivalTeamNameText;
    [SerializeField] private Button cardButton;

    [Header("Equipo Rival")]
    public string rivalTeamName;
    public string rivalFormation;
    public Dictionary<string, string> rivalPlayersByPosition;
    private AllTeamFormations allTeamFormations;

    private int teamAScore = 0;
    private int teamBScore = 0;
    private float matchTime = 0f;
    private bool isPaused = false;
    private bool isSecondHalf = false;

    private List<TeamFormation> teamFormations;
    private List<TeamFormation> allTeams;

    private List<MatchEvent> matchEvents = new List<MatchEvent>();

    [Header("Match Settings")]
    [SerializeField] private float halfDuration = 1800f; // 30 minutos en segundos
    [SerializeField] private float timeMultiplier = 30f; // Tiempo avanza 30 veces más rápido


    private void Start()
    {
        LoadDefaultFormations();
        SelectRandomRivalTeam();
        
        UpdateScore();
        pauseButton.onClick.AddListener(TogglePause);

        //Cargar equipos
        string myTeamName = PlayerPrefs.GetString("TeamName", "Mi Equipo");
        string opponentTeamName = rivalTeamName;

        myTeamNameText.text = myTeamName;
        rivalTeamNameText.text = opponentTeamName;
    }

    private void LoadDefaultFormations()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "default_formations.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            allTeamFormations = JsonUtility.FromJson<AllTeamFormations>(jsonContent);
        }
    }

    private void SelectRandomRivalTeam()
    {
        //Obtener el nombre del equipo del jugador
        string myTeamName = PlayerPrefs.GetString("TeamName", "");

        //Filtrar los equipos para excluir el del jugador
        List<TeamFormation> availableTeams = allTeamFormations.teamFormations.FindAll(team => team.teamName != myTeamName);

        //Seleccionar un equipo rival aleatorio
        TeamFormation rivalTeam = availableTeams[Random.Range(0, availableTeams.Count)];

        //Asignar los datos del rival
        rivalTeamName = rivalTeam.teamName;
        rivalFormation = rivalTeam.formation;
        rivalPlayersByPosition = new Dictionary<string, string>();

        //Convertir la lista en un diccionario temporal para facilidad
        foreach (var player in rivalTeam.playersByPosition)
        {
            rivalPlayersByPosition[player.position] = player.player;
        }
    }

    private void SetUpTeamsForMatch(string userTeamName, string opponentTeamName)
    {
        //Buscar las alineaciones de ambos equipos
        TeamFormation userTeam = teamFormations.Find(t => t.teamName == userTeamName);
        TeamFormation opponentTeam = teamFormations.Find(t => t.teamName == opponentTeamName);
    }

    private void Update()
    {
        if (!isPaused)
        {
            matchTime += Time.deltaTime * timeMultiplier;

            if (matchTime >= halfDuration && !isSecondHalf)
            {
                EndHalf();
            }
            else if (matchTime >= halfDuration * 2)
            {
                EndMatch();
            }
            UpdateTimer();
            GenerateMatchEvent();
        }
    }

    private void UpdateScore()
    {
        scoreText.text = $"{teamAScore} - {teamBScore}";
    }

    private void UpdateTimer()
    {
        int minutes = Mathf.FloorToInt(matchTime / 60);
        int seconds = Mathf.FloorToInt(matchTime % 60);
        timerText.text = $"Minuto {minutes}:{seconds:D2}";
    }

    private void TogglePause()
    {
        if (isPaused && matchTime >= halfDuration && !isSecondHalf)
        {
            StartSecondHalf();
        }
        else
        {
            isPaused = !isPaused;
        }
    }

    private void EndHalf()
    {
        isPaused = true;
        AddEvent("Final de la primera mitad.");
    }

    private void StartSecondHalf()
    {
        isPaused = false;
        isSecondHalf = true;
        AddEvent("Inicia la segunda mitad.");
    }

    private void EndMatch()
    {
        isPaused = true;
        AddEvent("Fin del partido.");
        
        //Mostrar estadísticas
        Debug.Log($"Resultado final: {myTeamNameText.text} {teamAScore} - {teamBScore} {rivalTeamName}");
        foreach (var matchEvent in matchEvents)
        {
            Debug.Log($"Minuto {matchEvent.time:F1}: {matchEvent.description}");
        }

        // Aquí puedes implementar lógica adicional para mostrar un resumen en la interfaz
    }

    public void AddGoal(string team)
    {
        if (team == "A") teamAScore++;
        if (team == "B") teamBScore++;
        UpdateScore();
        AddEvent($"Gol del equipo {team} en el minuto {Mathf.FloorToInt(matchTime / 60)}.");
    }

    private void AddEvent(string eventText)
    {
        GameObject eventItem = Instantiate(eventItemPrefab, eventLogScrollView.content);
        TMP_Text eventTextComponent = eventItem.GetComponent<TMP_Text>();

        if (eventTextComponent != null)
        {
            eventTextComponent.text = eventText;
        }

        //Limitar el número de eventos visibles
        if (eventLogScrollView.content.childCount > 15)
        {
            Destroy(eventLogScrollView.content.GetChild(0).gameObject);
        }

        //Scroll automático hacia el final
        Canvas.ForceUpdateCanvases();
        eventLogScrollView.verticalNormalizedPosition = 0f;
    }

    private void GenerateMatchEvent()
    {
        //Probabilidad de que ocurra un evento en cada frame
        float eventProbability = 0.0001f; // Ajusta este valor para más o menos eventos

        if (Random.value < eventProbability)
        {
            //Determinar el equipo y el tipo de evento
            string team = Random.value < 0.5f ? myTeamNameText.text : rivalTeamNameText.text;
            string eventType = RandomEventType();

            //Crear el evento
            MatchEvent newEvent = new MatchEvent
            {
                time = matchTime / 60, // Tiempo en minutos
                team = team,
                description = $"Minuto {Mathf.FloorToInt(matchTime / 60)}: {team} {eventType}"
            };

            //Guardar el evento
            matchEvents.Add(newEvent);

            //Mostrar en el registro de eventos
            AddEvent(newEvent.description);

            //Actualizar marcador si es un gol
            if (eventType == "marca un gol")
            {
                if (team == myTeamNameText.text) teamAScore++;
                else teamBScore++;

                UpdateScore();
            }
        }
    }

    private string RandomEventType()
    {
        string[] eventTypes = { "marca un gol", "realiza un tiro a puerta", "hace una falta", "saca un córner" };
        return eventTypes[Random.Range(0, eventTypes.Length)];
    }

    public void AddGoalToMyTeam()
    {
        // Incrementar el marcador de tu equipo
        teamAScore++;

        // Actualizar el marcador en la UI
        UpdateScore();

        // Crear y registrar el evento en la lista de eventos
        string eventText = $"Minuto {Mathf.FloorToInt(matchTime / 60)}: {myTeamNameText.text} usa la carta gol extra y se suma un gol.";
        AddEvent(eventText);

        // Desactiva el botón
        if (cardButton != null)
        {
            cardButton.interactable = false;
        }
    }
}
