using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<CardData> availableCards;
    public GameObject cardUIPrefab;
    public Transform cardParent;

    public void ShowCardSelection()
    {
        ClearCards();

        foreach (var card in availableCards)
        {
            GameObject cardObj = Instantiate(cardUIPrefab, cardParent);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card);
        }
    }

    public void ClearCards()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
    }
}