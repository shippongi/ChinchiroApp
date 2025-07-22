using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public List<CardData> availableCards;
    public GameObject cardUIPrefab;
    public Transform cardParent;
    public GameObject cardSelectionPanel;
    public UIManager uIManager;

    private List<GameObject> activeCardUIs = new List<GameObject>();

    public static CardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowCardSelection()
    {
        ClearCards();
        cardSelectionPanel.SetActive(true);

        foreach (var card in availableCards)
        {
            GameObject cardObj = Instantiate(cardUIPrefab, cardParent);
            CardUI cardUI = cardObj.GetComponent<CardUI>();
            cardUI.Setup(card);
            activeCardUIs.Add(cardObj);
        }
    }

    public void ClearCards()
    {
        foreach (var obj in activeCardUIs)
        {
            Destroy(obj);
        }
        activeCardUIs.Clear();
    }

    public void OnPlayerCardPurchased(CardData purchasedCard)
    {
        // UIから削除
        GameObject targetUI = activeCardUIs.Find(ui => ui.GetComponent<CardUI>().GetCardData() == purchasedCard);
        if (targetUI != null)
        {
            activeCardUIs.Remove(targetUI);
            Destroy(targetUI);
        }

        // カード一覧からも削除
        availableCards.Remove(purchasedCard);

        // CPUに購入させる
        StartCoroutine(HandleCpuCardPurchase());
        uIManager.UpdateCardDisplays();
    }

    private IEnumerator HandleCpuCardPurchase()
    {
        yield return new WaitForSeconds(1f);

        if (availableCards.Count > 0)
        {
            // ランダムに1枚選択
            CardData cpuCard = availableCards[Random.Range(0, availableCards.Count)];

            MoneyManager.Instance.SubtractMoneyFromCpu(cpuCard.price); // CPUの所持金減少
            CardInventory.Instance.AddCardToCpu(cpuCard);              // CPUのカードに追加
            Debug.Log($"CPUが {cpuCard.cardName} を購入しました。");

            availableCards.Remove(cpuCard);
        }

        HideCardSelectionUI();
    }

    public void HideCardSelectionUI()
    {
        ClearCards();
        cardSelectionPanel.SetActive(false);
    }

}