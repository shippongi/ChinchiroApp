using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private Dice[] diceArray;

    public void RollAll() => diceArray.ToList().ForEach(d => d.Roll());

    public List<int> GetAllResults() => diceArray.Select(d => d.GetValue()).ToList();

    public List<Dice> GetAllDice() => diceArray.ToList();

    public void ResetAll() => diceArray.ToList().ForEach(d => d.ResetPosition());
}