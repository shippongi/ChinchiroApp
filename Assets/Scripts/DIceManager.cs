using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DIceManager : MonoBehaviour
{
    public Dice[] diceArray = new Dice[3];

    public void RollAll()
    {
        foreach (var dice in diceArray)
        {
            dice.Roll();
        }
    }

    public List<int> GetAllResults()
    {
        List<int> results = new List<int>();
        foreach (var dice in diceArray)
        {
            results.Add(dice.GetValueByY());
        }
        return results;
    }

    public void ResetAll()
    {
        foreach (var dice in diceArray)
        {
            dice.ResetPosition();
        }
    }

    public List<Dice> GetAllDice()
    {
        return diceArray.ToList();
    }

}
