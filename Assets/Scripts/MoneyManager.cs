using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField] private int startingMoney = 1_000_000;
    [SerializeField] private int initialBet = 10000;
    [SerializeField] private int minBet = 10000;
    [SerializeField] private int maxBet = 100000;
    [SerializeField] private int betStep = 5000;

    [SerializeField] private bool enableDoubleRule = true;

    [System.Serializable]
    public class YakuMultiplier
    {
        public string yakuKeyword;
        public int multiplier;
    }

    [SerializeField]
    private List<YakuMultiplier> yakuMultipliers = new List<YakuMultiplier>
    {
        new YakuMultiplier { yakuKeyword = "ピンゾロ", multiplier = 5 },
        new YakuMultiplier { yakuKeyword = "ゾロ目", multiplier = 3 },
        new YakuMultiplier { yakuKeyword = "シゴロ", multiplier = 2 },
        new YakuMultiplier { yakuKeyword = "目あり", multiplier = 1 },
        new YakuMultiplier { yakuKeyword = "ヒフミ", multiplier = 2 },
        new YakuMultiplier { yakuKeyword = "目無し", multiplier = 1 }
    };

   private int GetMultiplier(string yaku)
    {
        foreach (var entry in yakuMultipliers)
        {
            if (yaku.Contains(entry.yakuKeyword))
            {
                return entry.multiplier;
            }
        }
        return 1;
    } 

    public int PlayerMoney { get; private set; }
    public int CpuMoney { get; private set; }
    public int CurrentBet { get; private set; }

    public bool IsDoubleRuleEnabled => enableDoubleRule;

    // public int CurrentPlayerMoney { get; internal set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

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


    public void ApplyResult(bool playerWon, bool isDraw, string winnerYaku, string loserYaku)
    {
        if (isDraw) return;

        int winnerMultiplier = GetMultiplier(winnerYaku);
        int loserMultiplier = GetMultiplier(loserYaku);

        int finalMultiplier = YakuUtility.GetFinalMultiplier(winnerYaku, loserYaku);
        int change = CurrentBet * finalMultiplier;

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

    public void SubtractMoneyFromPlayer(int amount)
    {
        PlayerMoney -= amount;
        if (PlayerMoney < 0) PlayerMoney = 0;
    }

    public void SubtractMoneyFromCpu(int amount)
    {
        CpuMoney -= amount;
        if (CpuMoney < 0) CpuMoney = 0;
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
