using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("基本UI")]
    public TMP_Text turnText;
    public TMP_Text moneyText;

    [Header("保持カードリスト")]
    [SerializeField] private Transform playerCardParent;
    [SerializeField] private Transform cpuCardParent;
    [SerializeField] private GameObject cardUIPrefab;

    [Header("ログと役表示")]
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text matchResultText; 

    [Header("確定役表示（保持）")]
    [SerializeField] TMP_Text playerFinalYakuText;
    [SerializeField] TMP_Text cpuFinalYakuText;

    [Header("ボタンUI")]
    public GameObject rollButton;
    public Button retryButton;
    public Button rematchButton;

    [Header("ベット関連")]
    public Button increaseBetButton;
    public Button decreaseBetButton;
    public Button resetBetButton;
    public Button allInBetButton;

    [Header("参照")]
    public GameManager gameManager;
    public MoneyManager moneyManager;

    // === 基本更新 ===
    public void UpdateTurnDisplay(bool? isPlayerTurn)
    {
        if (turnText == null) return;

        if (isPlayerTurn == true)
            turnText.text = "あなたの番です";
        else if (isPlayerTurn == false)
            turnText.text = "CPUの番です";
        else
            turnText.text = "結果";
    }

    public void UpdateMoneyDisplay(int playerMoney, int cpuMoney, int betAmount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"あなた:\n{playerMoney:N0}円\nCPU:\n{cpuMoney:N0}円\n掛け金:\n{betAmount:N0}円";
        }
    }

    public void SetBetButtonsInteractable(bool interactable)
    {
        increaseBetButton.interactable = interactable;
        decreaseBetButton.interactable = interactable;
        resetBetButton.interactable = interactable;
        allInBetButton.interactable = interactable;
    }

    public void UpdateCardDisplays()
    {
        // ① 既存のカードUIを全削除
        foreach (Transform child in playerCardParent) Destroy(child.gameObject);
        foreach (Transform child in cpuCardParent) Destroy(child.gameObject);

        // ② プレイヤーのカード表示
        foreach (CardData card in CardInventory.Instance.GetPlayerCards())
        {
            GameObject cardObj = Instantiate(cardUIPrefab, playerCardParent);
            cardObj.GetComponent<CardUI>().Setup(card);
        }

        // ③ CPUのカード表示
        foreach (CardData card in CardInventory.Instance.GetCpuCards())
        {
            GameObject cardObj = Instantiate(cardUIPrefab, cpuCardParent);
            cardObj.GetComponent<CardUI>().Setup(card);
        }
    }

    // === ロール中の役表示 ===
    public void ShowMidRollResult(string who, List<int> eyes, string yaku)
    {
        string text = $"{who}の出目：{string.Join(",", eyes)}\n役：{yaku}";

        // ログへ追記
        resultText.text += text + "\n";

        // 中間表示欄に反映
        if (who == "あなた")
            resultText.text = text;
        else if (who == "CPU")
            resultText.text = text;
    }

    // === 確定役の表示 ===
    public void ShowFinalYakuResult(string who, List<int> eyes, string yaku)
    {
        string text = $"{who}の出目：{string.Join(",", eyes)}\n役：{yaku}";

        if (who == "あなた")
            playerFinalYakuText.text = text;
        else if (who == "CPU")
            cpuFinalYakuText.text = text;
    }

    // === 勝敗結果の表示 ===
    public void ShowMatchResult(string msg)
    {
        if (matchResultText != null)
            matchResultText.text = $"【結果】{msg}";
    }

    public void ShowMatchResultWithMultiplier(string resultText, int multiplier)
    {
        if (multiplier > 1)
        {
            matchResultText.text = $"{resultText}（×{multiplier}倍）";
        }
        else
        {
            matchResultText.text = resultText;
        }

        matchResultText.gameObject.SetActive(true);
    }

    public void ShowResultText(string text)
    {
        if (resultText != null)
            resultText.text += text + "\n";
    }

    // === 表示制御 ===
    public void SetResultSectionActive(string who, bool active)
    {
        if (who == "あなた" && resultText != null)
            resultText.gameObject.SetActive(active);
        else if (who == "CPU" && resultText != null)
            resultText.gameObject.SetActive(active);
    }

    // === ボタン系 ===
    public void ShowRetryButton(bool show) => retryButton?.gameObject.SetActive(show);
    public void ShowRematchButton(bool show) => rematchButton?.gameObject.SetActive(show);
    public void HideRollButton() => rollButton?.SetActive(false);
    public void ShowRollButton(bool show) => rollButton?.SetActive(show);
    public bool IsRollButtonActive() => rollButton != null && rollButton.activeSelf;

    // === ベット変更 ===
    public void OnClickIncreaseBet() => gameManager.ChangeBet(true);
    public void OnClickDecreaseBet() => gameManager.ChangeBet(false);
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

    public void SetBetChangeInteractable(bool interactable)
    {
        increaseBetButton.interactable = interactable;
        decreaseBetButton.interactable = interactable;
        resetBetButton.interactable = interactable;
        allInBetButton.interactable = interactable;
    }

    // === 表示リセット ===
    public void ClearAllTexts()
    {
        resultText.text = "";
        playerFinalYakuText.text = "";
        cpuFinalYakuText.text = "";
        matchResultText.text = "";
    }

    public void ResetResultText()
    {
        resultText.text = "";
        matchResultText.text = "";
    }

    public void ClearResultText()
    {
        resultText.text = "";
    }

    public void OnClickRetry()
    {
        ClearResultText();           // ログをクリア
        ResetResultText();           // 現在の中間役・勝敗をクリア
        ShowRetryButton(false);      // リトライボタンを非表示
        ShowRollButton(true);        // プレイヤーの再ロールを許可
    }

    // === 特定の出目ログ除去（使用しないなら削除可） ===
    public void RemoveResultSection(string who)
    {
        if (resultText == null) return;

        var lines = resultText.text.Split('\n');
        resultText.text = string.Join("\n", lines
            .Where(line => !line.StartsWith($"{who}の出目：") && !line.StartsWith("役："))
            .ToArray());
    }
}