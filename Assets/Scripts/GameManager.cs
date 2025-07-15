using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public DiceManager diceManager;
    public PlayerController playerController;
    public MoneyManager moneyManager;

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;
    private bool isInputEnabled = true;
    private bool isTurnProcessing = false;

    void Start()
    {
        ResetGame();
        uiManager.ShowRetryButton(false);
        uiManager.UpdateTurnDisplay(true);
        UpdateMoneyUI();
    }

    void ResetGame()
    {
        moneyManager.ResetMoney();
        diceManager.ResetAll();
        uiManager.OnClickRetry();
        currentTurn = Turn.Player;
        isInputEnabled = true;

        uiManager.ShowRetryButton(false);
        uiManager.ShowRematchButton(false);
        uiManager.UpdateTurnDisplay(true);
        uiManager.UpdateMoneyDisplay(moneyManager.PlayerMoney, moneyManager.CpuMoney, moneyManager.CurrentBet);
    }

    public bool CanRollByInput() => isInputEnabled && uiManager.IsRollButtonActive();

    public void OnSpaceKey()
    {
        if (!isInputEnabled || currentTurn != Turn.Player) return;

        playerController.TryRoll();
        uiManager.ShowRollButton(false);
    }

    public void OnRetryKey()
    {
        if (!isInputEnabled) return;

        isInputEnabled = false;
        diceManager.ResetAll();
        uiManager.OnClickRetry();
        currentTurn = Turn.Player;
        uiManager.UpdateTurnDisplay(true);
        isInputEnabled = true;
    }

    public void OnRematchKey()
    {
        ResetGame();
    }

    public void StartPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        isInputEnabled = false;
        uiManager.HideRollButton();
        uiManager.SetBetButtonsInteractable(false);
        StartCoroutine(RollAndSwitchTurn());
    }

    IEnumerator RollAndSwitchTurn()
    {
        if (isTurnProcessing) yield break;
        isTurnProcessing = true;

        DiceResult playerResult = new DiceResult();
        DiceResult cpuResult = new DiceResult();

        yield return StartCoroutine(RollAndCheck("あなた", res => playerResult = res));
        yield return new WaitForSeconds(1f);

        currentTurn = Turn.CPU;
        uiManager.UpdateTurnDisplay(false);

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RollAndCheck("CPU", res => cpuResult = res));

        yield return new WaitForSeconds(1f);
        currentTurn = Turn.Player;
        uiManager.UpdateTurnDisplay(true);

        int playerPower = YakuUtility.GetYakuStrength(playerResult.yaku);
        int cpuPower = YakuUtility.GetYakuStrength(cpuResult.yaku);

        HandleTurnResult(playerResult, cpuResult);

        isTurnProcessing = false;
    }

    void HandleTurnResult(DiceResult playerResult, DiceResult cpuResult)
    {
        int playerPower = YakuUtility.GetYakuStrength(playerResult.yaku);
        int cpuPower = YakuUtility.GetYakuStrength(cpuResult.yaku);

        string resultMsg;
        bool isDraw = playerPower == cpuPower;

        if (isDraw)
        {
            resultMsg = "引き分け！";
        }
        else if (playerPower > cpuPower)
        {
            resultMsg = "あなたの勝ち！";
        }
        else
        {
            resultMsg = "CPUの勝ち！";
        }

        uiManager.ShowMatchResult(resultMsg);

        moneyManager.ApplyResult(
            playerPower > cpuPower,
            isDraw,
            playerPower > cpuPower ? playerResult.yaku : cpuResult.yaku,
            playerPower > cpuPower ? cpuResult.yaku : playerResult.yaku
        );
        UpdateMoneyUI();
        EndTurn(resultMsg);

        if (moneyManager.IsGameOver())
        {
            uiManager.ShowRematchButton(true);
            uiManager.ShowRetryButton(false);
            uiManager.ShowResultText("所持金が尽きました！ゲーム終了！");
            isInputEnabled = false;
        }
        else
        {
            uiManager.ShowRetryButton(true);
            isInputEnabled = true;
        }
    }

    IEnumerator RollAndCheck(string who, System.Action<DiceResult> onComplete)
    {
        int rerollCount = 0;
        const int maxReroll = 2;

        DiceResult currentResult = new DiceResult();
        DiceResult finalResult = new DiceResult();

        uiManager.RemoveResultSection(who);

        while (true)
        {
            diceManager.RollAll();
            yield return StartCoroutine(WaitForDiceToStop(diceManager.GetAllDice()));

            var eyes = diceManager.GetAllResults();
            string yaku = YakuUtility.GetYakuName(eyes);

            currentResult = new DiceResult(eyes, yaku);

            uiManager.ShowYakuResult(who, eyes, yaku);

            if (yaku == "目無し" && rerollCount < maxReroll)
            {
                rerollCount++;

                if (who == "あなた")
                {
                    uiManager.ShowRollButton(true);
                    uiManager.SetBetChangeInteractable(false);
                    isInputEnabled = true;

                    yield return new WaitUntil(() => !uiManager.IsRollButtonActive());

                    isInputEnabled = false;
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }

                continue;
            }
            else
            {
                finalResult = currentResult;
                break;
            }
        }

        onComplete?.Invoke(finalResult);
    }

    IEnumerator WaitForDiceToStop(List<Dice> diceList, float threshold = 0.05f, float duration = 1.0f)
    {
        float timer = 0f;
        while (true)
        {
            bool allStopped = diceList.All(d => d.IsStopped(threshold));
            timer = allStopped ? timer + Time.deltaTime : 0f;

            if (timer >= duration) yield break;
            yield return null;
        }
    }

    void UpdateMoneyUI()
    {
        uiManager.UpdateMoneyDisplay(
            moneyManager.PlayerMoney,
            moneyManager.CpuMoney,
            moneyManager.CurrentBet);
    }

    void EndTurn(string resultMsg)
    {
        uiManager.ClearResultText();

        uiManager.ShowResultText($"\n【結果】{resultMsg}");
        uiManager.HideRollButton();
        uiManager.ShowRetryButton(true);
        isInputEnabled = true;
    }

    struct DiceResult
    {
        public List<int> eyes;
        public string yaku;
        public DiceResult(List<int> eyes, string yaku) { this.eyes = eyes; this.yaku = yaku; }
    }

    public void ChangeBet(bool increase)
    {
        if (increase)
            moneyManager.IncreaseBet();
        else
            moneyManager.DecreaseBet();

        UpdateMoneyUI();
    }

}
