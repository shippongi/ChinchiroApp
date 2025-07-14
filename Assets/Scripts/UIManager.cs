using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text turnText;
    public TMP_Text moneyText;

    public GameObject rollButton;
    public Button retryButton;
    public Button rematchButton;

    public Button increaseBetButton;
    public Button decreaseBetButton;
    public Button resetBetButton;
    public Button allInBetButton;

    public GameManager gameManager;
    public MoneyManager moneyManager;

    public void UpdateTurnDisplay(bool isPlayerTurn)
    {
        if (turnText != null)
            turnText.text = isPlayerTurn ? "あなたの番です" : "CPUの番です";

        if (rollButton != null)
            rollButton.SetActive(isPlayerTurn);
        
        SetBetButtonsInteractable(isPlayerTurn);
    }

    public void SetBetButtonsInteractable(bool interactable)
    {
        if (increaseBetButton != null) increaseBetButton.interactable = interactable;
        if (decreaseBetButton != null) decreaseBetButton.interactable = interactable;
        if (resetBetButton != null) resetBetButton.interactable = interactable;
        if (allInBetButton != null) allInBetButton.interactable = interactable;
    }

    public void UpdateMoneyDisplay(int playerMoney, int cpuMoney, int betAmount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"あなた:\n{playerMoney:N0}円\nCPU:\n{cpuMoney:N0}円\n掛け金:\n{betAmount:N0}円";
        }
    }

    public void ShowResult(string who, List<int> results, string yaku)
    {
        if (resultText != null)
        {
            resultText.text += $"{who}の出目：{string.Join(",", results)}\n役：{yaku}\n";
        }
    }

    public void ShowRetryButton(bool show) => retryButton?.gameObject.SetActive(show);
    public void ShowRematchButton(bool show) => rematchButton?.gameObject.SetActive(show);
    public void HideRollButton() => rollButton?.SetActive(false);
    public bool IsRollButtonActive() => rollButton != null && rollButton.activeSelf;

    public void OnClickRetry()
    {
        resultText.text = "";
        ShowRetryButton(false);
        rollButton.SetActive(true);
    }

    public void ShowResultText(string text)
    {
        resultText.text += text + "\n";
    }

    public void OnClickIncreaseBet()
    {
        gameManager.ChangeBet(true);
    }

    public void OnClickDecreaseBet()
    {
        gameManager.ChangeBet(false);
    }

    public void OnClickResetBet()
    {
        moneyManager.ResetBet();
        UpdateMoneyDisplay(moneyManager.PlayerMoney, moneyManager.CpuMoney, moneyManager.CurrentBet);
    }

    public void OnClickAllIn()
    {
        moneyManager.AllIn();
        UpdateMoneyDisplay(moneyManager.PlayerMoney, moneyManager.CpuMoney, moneyManager.CurrentBet);
    }


}
