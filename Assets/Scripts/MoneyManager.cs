using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private int startingMoney = 1_000_000;
    [SerializeField] private int initialBet = 10000;
    [SerializeField] private int minBet = 10000;
    [SerializeField] private int maxBet = 100000;
    [SerializeField] private int betStep = 5000;

    public int PlayerMoney { get; private set; }
    public int CpuMoney { get; private set; }
    public int CurrentBet { get; private set; }

    void Awake()
    {
        PlayerMoney = startingMoney;
        CpuMoney = startingMoney;
        CurrentBet = initialBet;
    }

    public void IncreaseBet()
    {
        if (CurrentBet + betStep <= maxBet)
            CurrentBet += betStep;
    }

    public void DecreaseBet()
    {
        if (CurrentBet - betStep >= minBet)
            CurrentBet -= betStep;
    }

    public void ResetBet()
    {
        CurrentBet = initialBet;
    }

    public void AllIn()
    {
        CurrentBet = maxBet;
    }


    public void ApplyResult(bool playerWon, bool isDraw)
    {
        if (isDraw) return;

        int change = CurrentBet;

        if (playerWon)
        {
            PlayerMoney += change;
            CpuMoney -= change;
        }
        else
        {
            PlayerMoney -= change;
            CpuMoney += change;
        }
    }

    public bool IsGameOver()
    {
        return PlayerMoney <= 0 || CpuMoney <= 0;
    }

    public void ResetMoney()
    {
        PlayerMoney = startingMoney;
        CpuMoney = startingMoney;
        CurrentBet = initialBet;
    }
    
}
