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

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;
    private bool isInputEnabled = true;

    void Start()
    {
        uiManager.ShowRetryButton(false);
        uiManager.UpdateTurnDisplay(true);
    }

    public bool CanRollByInput() => isInputEnabled && uiManager.IsRollButtonActive();

    public void OnSpaceKey()
    {
        if (currentTurn == Turn.Player)
            playerController.TryRoll();
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

    public void StartPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        isInputEnabled = false;
        uiManager.HideRollButton();
        StartCoroutine(RollAndSwitchTurn());
    }

    IEnumerator RollAndSwitchTurn()
    {
        var playerResult = new DiceResult();
        var cpuResult = new DiceResult();

        yield return StartCoroutine(RollAndCheck("あなた", res => playerResult = res));
        yield return new WaitForSeconds(1f);

        currentTurn = Turn.CPU;
        uiManager.UpdateTurnDisplay(false);
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(RollAndCheck("CPU", res => cpuResult = res));
        yield return new WaitForSeconds(1f);

        int playerPower = YakuUtility.GetYakuStrength(playerResult.yaku);
        int cpuPower = YakuUtility.GetYakuStrength(cpuResult.yaku);
        string result = playerPower > cpuPower ? "あなたの勝ち！" :
                        playerPower < cpuPower ? "CPUの勝ち！" : "引き分け！";

        uiManager.ShowResultText($"\n【結果】{result}");
        uiManager.ShowRetryButton(true);
        isInputEnabled = true;
    }

    IEnumerator RollAndCheck(string who, System.Action<DiceResult> onComplete)
    {
        diceManager.RollAll();
        yield return StartCoroutine(WaitForDiceToStop(diceManager.GetAllDice()));

        var results = diceManager.GetAllResults();
        string yaku = YakuUtility.GetYakuName(results);

        uiManager.ShowResult(who, results, yaku);
        onComplete?.Invoke(new DiceResult(results, yaku));
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

    struct DiceResult
    {
        public List<int> eyes;
        public string yaku;
        public DiceResult(List<int> eyes, string yaku) { this.eyes = eyes; this.yaku = yaku; }
    }
}