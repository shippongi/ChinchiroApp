using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public TMP_Text cardNameText;
    public TMP_Text priceText;
    public TMP_Text descriptionText;
    public Button purchaseButton;

    private CardData cardData;

    public void Setup(CardData data)
    {
        cardData = data;
        cardNameText.text = data.cardName;
        priceText.text = $"{data.price:N0}円";
        descriptionText.text = data.description;

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => PurchaseCard());
    }

    private void PurchaseCard()
    {
        GameManager.Instance.PurchaseCard(cardData); // 所持金処理など
    }
}
