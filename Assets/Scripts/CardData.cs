using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Chinchiro/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int price;
    public string description;
    public CardEffectType effectType;
}
public enum CardEffectType
{
    LowRollOnly, // 1〜3しか出ない
    HighRollOnly, // 4〜6しか出ない（例）
    RerollOnce, // 1回だけ振り直せる（例）
    None // スキップなど
}
