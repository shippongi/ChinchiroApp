using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text turnText;
    public GameObject rollButton;

    public Transform topFace, bottomFace, frontFace, backFace, leftFace, rightFace;

    Rigidbody rb;

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;

    [SerializeField] Button retryButton;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateTurnDisplay();
        retryButton.gameObject.SetActive(false);
        // StartCoroutine(RollAndSwitchTurn());
    }

    public void RollDice()
    {
        if (currentTurn != Turn.Player) return;

        rollButton.SetActive(false);

        StartCoroutine(RollAndSwitchTurn());
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

        // 勝敗判定！
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
            resultText.text += $"\n\n【結果】{resultMsg}";

        retryButton.gameObject.SetActive(true);
    }

    IEnumerator RollAndCheck(string who, System.Action<DiceResult> onComplete)
    {
        List<int> results = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            Roll();
            yield return new WaitForSeconds(2.0f);
            int result = GetDiceValueByY();
            results.Add(result);
        }

        string yaku = GetYakuName(results);

        if (resultText != null)
            resultText.text = $"{who}の出目：{string.Join(",", results)}\n役：{yaku}";

        onComplete?.Invoke(new DiceResult(results, yaku));

        yield break;
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

    string GetYakuName(List<int> results)
    {
        if (results == null || results.Count != 3)
            return "不正な出目";

        results.Sort(); // 昇順に並べると判定が楽になる

        int a = results[0];
        int b = results[1];
        int c = results[2];

        // ピンゾロ
        if (a == 1 && b == 1 && c == 1) return "ピンゾロ（最強）";

        // ゾロ目
        if (a == b && b == c) return $"ゾロ目（{a}）";

        // シゴロ
        if (a == 4 && b == 5 && c == 6) return "シゴロ（強）";

        // ヒフミ
        if (a == 1 && b == 2 && c == 3) return "ヒフミ（弱）";

        // 目あり（2つが同じ数字）
        if (a == b && c != a) return $"目あり：{c}";
        if (b == c && a != b) return $"目あり：{a}";
        if (a == c && b != a) return $"目あり：{b}";

        // その他は目無し
        return "目無し";
    }

    int GetYakuStrength(string yaku)
    {
        if (yaku.Contains("ピンゾロ")) return 100;
        if (yaku.Contains("ゾロ目")) return 90;
        if (yaku.Contains("シゴロ")) return 80;
        if (yaku.Contains("目あり"))
        {
            // 目あり：X → Xの数値を取り出して強さに
            string[] parts = yaku.Split('：');
            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                return 10 + value;
            return 10; // fallback
        }
        if (yaku.Contains("ヒフミ")) return 5;
        return 0; // 目無しや不正など
    }

    public void Roll()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = new Vector3(0, 2, 0);

        Vector3 randomDir = Random.onUnitSphere;
        float force = Random.Range(5f, 10f);
        rb.AddForce(Vector3.up * 5f + randomDir * force, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Impulse);
    }

    int GetDiceValueByY()
    {
        var faces = new (Transform t, int value)[]
        {
            (topFace, 2),
            (bottomFace, 5),
            (frontFace, 6),
            (backFace, 1),
            (leftFace, 3),
            (rightFace, 4)
        };

        Transform maxT = null;
        float maxY = float.MinValue;
        int result = -1;

        foreach (var (t, value) in faces)
        {
            if (t.position.y > maxY)
            {
                maxY = t.position.y;
                result = value;
                maxT = t;
            }
        }

        return result;
    }

    void UpdateTurnDisplay()
    {
        if (turnText != null)
            turnText.text = currentTurn == Turn.Player ? "あなたの番です" : "CPUの番です";

        if (rollButton != null)
            rollButton.SetActive(currentTurn == Turn.Player);
    }

    public void OnClickRetry()
    {
        retryButton.gameObject.SetActive(false);
        resultText.text = "";
        currentTurn = Turn.Player;      // プレイヤーの番に戻す
        UpdateTurnDisplay();            // DiceRollボタン表示状態更新
    }

}
