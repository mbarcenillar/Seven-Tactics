using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticPanelManager : MonoBehaviour
{
    [Header("My Team Panel")]
    [SerializeField] private TMP_Text myTeamNameText;
    [SerializeField] private Transform myTeamContent;

    [Header("Rival Team Panel")]
    [SerializeField] private TMP_Text rivalTeamNameText;
    [SerializeField] private Transform rivalTeamContent;

    [Header("UI Elements")]
    [SerializeField] private GameObject playerItemPrefab;

    private DatabaseManager databaseManager;
    private FormationData formationData;
    private MatchManager matchManager;

    private Dictionary<string, string> playersByPosition = new Dictionary<string, string>();

    private void Start()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        matchManager = FindObjectOfType<MatchManager>();

        LoadAndDisplayFormation();
        LoadRivalTeam();

        //Cargar equipos
        string myTeamName = PlayerPrefs.GetString("TeamName", "Mi Equipo");
        string opponentTeamName = matchManager.rivalTeamName;

        myTeamNameText.text = myTeamName;
        rivalTeamNameText.text = opponentTeamName;
    }

    public void LoadAndDisplayFormation()
    {
        string savedFormationJson = PlayerPrefs.GetString("SavedFormation", null);

        FormationData formationData = JsonUtility.FromJson<FormationData>(savedFormationJson);

        //Limpiar panel anterior
        foreach (Transform child in myTeamContent)
        {
            Destroy(child.gameObject);
        }

        //Mostrar los jugadores en el panel
        foreach (var entry in formationData.playersByPosition)
        {
            GameObject playerItem = Instantiate(playerItemPrefab, myTeamContent);
            playerItem.SetActive(true);

            TMP_Text positionText = playerItem.transform.Find("PositionText").GetComponent<TMP_Text>();
            TMP_Text playerNameText = playerItem.transform.Find("PlayerNameText").GetComponent<TMP_Text>();

            if (positionText != null)
            {
                positionText.text = entry.position;
            }

            if (playerNameText != null)
            {
                playerNameText.text = entry.player;
            }
        }
    }

    private void LoadRivalTeam()
    {
        //Limpiar el contenido anterior del panel rival
        foreach (Transform child in rivalTeamContent)
        {
            Destroy(child.gameObject);
        }

        //Mostrar jugadores del equipo rival en el panel
        foreach (var entry in matchManager.rivalPlayersByPosition)
        {
            //Crear el prefab para cada jugador
            GameObject playerItem = Instantiate(playerItemPrefab, rivalTeamContent);
            playerItem.SetActive(true);

            //Asignar posición y nombre del jugador al prefab
            TMP_Text positionText = playerItem.transform.Find("PositionText").GetComponent<TMP_Text>();
            TMP_Text playerNameText = playerItem.transform.Find("PlayerNameText").GetComponent<TMP_Text>();

            if (positionText != null)
            {
                positionText.text = entry.Key;
            }

            if (playerNameText != null)
            {
                playerNameText.text = entry.Value;
            }
        }
    }
}
