using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public DiceRoller dice;
    public TMP_Text resultText;
    public TMP_Text turnText;
    public GameObject rollButton;
    [SerializeField] Button retryButton;

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;

    public bool isInputEnabled { get; private set; } = true;


    public PlayerController playerController;
    public UIManager uiManager;

    public bool CanRollByInput()
    {
        return isInputEnabled && uiManager.IsRollButtonActive();
    }

    void Start()
    {
        UpdateTurnDisplay();
        retryButton.gameObject.SetActive(false);
    }

    public void StartPlayerTurn()
    {
        if (currentTurn != Turn.Player) return;

        isInputEnabled = false;
        uiManager.rollButton.SetActive(false);
        StartCoroutine(RollAndSwitchTurn());
    }

    public void OnSpaceKey()
    {
        if (currentTurn != Turn.Player) return;
        playerController.TryRoll();
    }

    public void OnRetryKey()
    {
        if (!isInputEnabled) return;

        isInputEnabled = true;
        uiManager.OnClickRetry();
        currentTurn = Turn.Player;
        uiManager.UpdateTurnDisplay(true);
        // StartCoroutine(RollAndSwitchTurn());
    }

    IEnumerator RollAndSwitchTurn()
    {
        DiceResult playerResult = new DiceResult();
        DiceResult cpuResult = new DiceResult();

        yield return StartCoroutine(RollAndCheck("あなた", res => playerResult = res));
        yield return new WaitForSeconds(1f);

        currentTurn = Turn.CPU;
        UpdateTurnDisplay();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RollAndCheck("CPU", res => cpuResult = res));

        yield return new WaitForSeconds(1f);
        currentTurn = Turn.Player;
        UpdateTurnDisplay();

        int playerPower = GetYakuStrength(playerResult.yaku);
        int cpuPower = GetYakuStrength(cpuResult.yaku);

        string resultMsg = "";
        if (playerPower > cpuPower)
            resultMsg = "あなたの勝ち！";
        else if (playerPower < cpuPower)
            resultMsg = "CPUの勝ち！";
        else
            resultMsg = "引き分け！";

        if (resultText != null)
            resultText.text += $"\n【結果】{resultMsg}";

        retryButton.gameObject.SetActive(true);
        uiManager.HideRollButton();
        uiManager.ShowRetryButton(true);

        isInputEnabled = true;
    }

    IEnumerator RollAndCheck(string who, System.Action<DiceResult> onComplete)
    {
        List<int> results = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            dice.Roll();
            yield return new WaitForSeconds(2.0f);
            int result = dice.GetTopFaceValue();
            results.Add(result);
        }

        string yaku = GetYakuName(results);

        if (resultText != null)
            resultText.text = $"{who}の出目：{string.Join(",", results)}\n役：{yaku}";

        onComplete?.Invoke(new DiceResult(results, yaku));
    }

    void UpdateTurnDisplay()
    {
        if (turnText != null)
            turnText.text = currentTurn == Turn.Player ? "あなたの番です" : "CPUの番です";

        if (rollButton != null)
            rollButton.SetActive(currentTurn == Turn.Player);
    }

    int GetYakuStrength(string yaku)
    {
        if (yaku.Contains("ピンゾロ")) return 100;
        if (yaku.Contains("ゾロ目")) return 90;
        if (yaku.Contains("シゴロ")) return 80;
        if (yaku.Contains("目あり"))
        {
            string[] parts = yaku.Split('：');
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                return 10 + value;
            return 10;
        }
        if (yaku.Contains("ヒフミ")) return 5;
        return 0;
    }

    string GetYakuName(List<int> results)
    {
        if (results == null || results.Count != 3)
            return "不正な出目";

        results.Sort();

        int a = results[0];
        int b = results[1];
        int c = results[2];

        if (a == 1 && b == 1 && c == 1) return "ピンゾロ（最強）";
        if (a == b && b == c) return $"ゾロ目（{a}）";
        if (a == 4 && b == 5 && c == 6) return "シゴロ（強）";
        if (a == 1 && b == 2 && c == 3) return "ヒフミ（弱）";
        if (a == b && c != a) return $"目あり：{c}";
        if (b == c && a != b) return $"目あり：{a}";
        if (a == c && b != a) return $"目あり：{b}";

        return "目無し";
    }

    struct DiceResult
    {
        public List<int> eyes;
        public string yaku;

        public DiceResult(List<int> eyes, string yaku)
        {
            this.eyes = eyes;
            this.yaku = yaku;
        }
    }
}
