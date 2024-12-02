using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text teamNameText;
    [SerializeField] private TMP_Text currentRoundText;

    [Header("Panels")]
    [SerializeField] private GameObject lineupPanel;
    [SerializeField] private GameObject standingsPanel;
    [SerializeField] private GameObject matchPanel;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons")]
    [SerializeField] private Button lineupButton;
    [SerializeField] private Button standingsButton;
    [SerializeField] private Button matchButton;
    [SerializeField] private Button statsButton;
    [SerializeField] private Button optionsButton;

     private void Start()
    {
        // Configurar botones
        lineupButton.onClick.AddListener(() => ShowPanel(lineupPanel));
        standingsButton.onClick.AddListener(() => ShowPanel(standingsPanel));
        matchButton.onClick.AddListener(() => ShowPanel(matchPanel));
        statsButton.onClick.AddListener(() => ShowPanel(statsPanel));
        optionsButton.onClick.AddListener(() => ShowPanel(optionsPanel));

        // Cargar datos iniciales
        LoadTeamInfo();
    }

    private void LoadTeamInfo()
    {
        // Cargar datos del equipo (aquí puedes conectarlo con tu base de datos)
        string teamName = PlayerPrefs.GetString("TeamName", "Mi Equipo");
        int points = PlayerPrefs.GetInt("TeamPoints", 0);
        int currentRound = PlayerPrefs.GetInt("CurrentRound", 1);

        teamNameText.text = teamName;
        currentRoundText.text = $"Jornada: {currentRound}";
    }

    private void ShowPanel(GameObject panel)
    {
        // Ocultar todos los paneles
        lineupPanel.SetActive(false);
        standingsPanel.SetActive(false);
        matchPanel.SetActive(false);
        statsPanel.SetActive(false);
        optionsPanel.SetActive(false);

        // Mostrar el panel seleccionado
        panel.SetActive(true);
    }
}
