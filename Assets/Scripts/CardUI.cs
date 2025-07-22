using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] public TMP_Text cardNameText;
    [SerializeField] public TMP_Text descriptionText;
    [SerializeField] public TMP_Text priceText;
    [SerializeField] public Button purchaseButton;

    private CardData cardData;

    public void Setup(CardData data)
    {
        cardData = data;

        // 共通で使う部分
        cardNameText.text = data.cardName;
        descriptionText.text = data.description;

        // 使用可能な場合だけ値設定
        if (priceText != null)
        {
            priceText.text = $"{data.price:N0}円";
        }

        // 使用可能な場合だけボタン設定
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveAllListeners();
            purchaseButton.onClick.AddListener(() => PurchaseCard());
        }

        // cardNameText.text = data.cardName;
        // priceText.text = $"{data.price:N0}円";
        // descriptionText.text = data.description;

        // purchaseButton.onClick.RemoveAllListeners();
        // purchaseButton.onClick.AddListener(() => PurchaseCard());
    }

    private void PurchaseCard()
    {
        GameManager.Instance.PurchaseCard(cardData); // 所持金処理など
    }
}
