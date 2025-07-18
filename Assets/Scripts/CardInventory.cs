using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    public static CardInventory Instance { get; private set; }

    private List<CardData> playerCards = new List<CardData>();
    private List<CardData> cpuCards = new List<CardData>();

    public IReadOnlyList<CardData> PlayerCards => playerCards.AsReadOnly();
    public IReadOnlyList<CardData> CpuCards => cpuCards.AsReadOnly();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // ゲーム間で保持したい場合
    }

    public void AddCardToPlayer(CardData card)
    {
        playerCards.Add(card);
    }

    public void AddCardToCpu(CardData card)
    {
        cpuCards.Add(card);
    }

    public void UsePlayerCard(CardData card)
    {
        if (playerCards.Contains(card))
        {
            playerCards.Remove(card);
            ApplyCardEffect(card, isPlayer: true);
        }
    }

    public void UseCpuCard(CardData card)
    {
        if (cpuCards.Contains(card))
        {
            cpuCards.Remove(card);
            ApplyCardEffect(card, isPlayer: false);
        }
    }

    private void ApplyCardEffect(CardData card, bool isPlayer)
    {
        // ここでカードの効果を適用
        Debug.Log($"{(isPlayer ? "プレイヤー" : "CPU")}がカード {card.cardName} を使用した");

        // 例：DiceManager.Instance.ForceDiceRange(1, 3); など
    }

    public void ResetInventory()
    {
        playerCards.Clear();
        cpuCards.Clear();
    }

}
