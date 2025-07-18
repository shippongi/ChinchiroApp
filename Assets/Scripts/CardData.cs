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
    HighRollOnly, // 4〜6しか出ない
    FixOneTo1, //ダイス一つを1に固定
    FixOneTo6, //ダイス一つを6に固定
    RerollOnce, // 振り直し回数を増加する
    DoubleBet, //掛け金の結果を倍にする
    NoPayment, //支払いをなしにできる
    Reflection, //勝敗結果を逆転させる
    None // スキップなど
}
