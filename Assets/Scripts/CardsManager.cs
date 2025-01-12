using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject cardsPanel;
    [SerializeField] private ScrollRect cardsScrollView;
    [SerializeField] private TMP_Text cardDescriptionText;
    [SerializeField] private Button useCardButton;
    [SerializeField] private GameObject cardItemPrefab;

    [Header("Cartas Disponibles")]
    [SerializeField] private List<MatchCard> availableCards;

    private MatchCard selectedCard;

    private void Start()
    {
        PopulateCardsList();

        useCardButton.onClick.AddListener(() => UseCard());
    }

    private void PopulateCardsList()
    {
        foreach (var card in availableCards)
        {
            GameObject cardItem = Instantiate(cardItemPrefab, cardsScrollView.content);
            TMP_Text cardNameText = cardItem.GetComponentInChildren<TMP_Text>();
            if (cardNameText != null)
            {
                cardNameText.text = card.cardName;
            }

            Button cardButton = cardItem.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(() => SelectCard(card));
            }
        }
    }

    private void SelectCard(MatchCard card)
    {
        selectedCard = card;
        cardDescriptionText.text = $"{card.cardName}\n\n{card.description}";
        useCardButton.interactable = true;
    }

    private void UseCard()
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("No hay una carta seleccionada.");
            return;
        }

        // Aplicar el efecto de la carta
        ApplyCardEffect(selectedCard);

        // Mostrar un evento en el partido
        //FindObjectOfType<MatchManager>().AddEvent($"Se ha usado la carta: {selectedCard.cardName}. Resultado: {selectedCard.description}");

        // Eliminar la carta después de usarla (opcional)
        availableCards.Remove(selectedCard);
        PopulateCardsList();

        // Reiniciar selección
        selectedCard = null;
        cardDescriptionText.text = "Selecciona una carta para ver su descripción.";
        useCardButton.interactable = false;
    }

    private void ApplyCardEffect(MatchCard card)
    {
        // Lógica para aplicar el efecto según el tipo de carta
        switch (card.effectType)
        {
            case "Mejora Defensa":
                Debug.Log($"Defensa mejorada en {card.effectValue} puntos.");
                break;
            case "Aumenta Ataque":
                Debug.Log($"Ataque aumentado en {card.effectValue} puntos.");
                break;
            default:
                Debug.LogWarning("Tipo de carta desconocido.");
                break;
        }
    }
}
