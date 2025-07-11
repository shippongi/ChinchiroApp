using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text turnText;
    public GameObject rollButton;

    public Transform topFace, bottomFace, frontFace, backFace, leftFace, rightFace;

    Rigidbody rb;

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateTurnDisplay();
    }

    public void RollDice()
    {
        if (currentTurn != Turn.Player) return;

        StartCoroutine(RollAndSwitchTurn());
    }

    IEnumerator RollAndSwitchTurn()
    {
        yield return StartCoroutine(RollAndCheck("あなた"));
        yield return new WaitForSeconds(1f);

        currentTurn = Turn.CPU;
        UpdateTurnDisplay();

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(RollAndCheck("CPU"));
        yield return new WaitForSeconds(1f);

        currentTurn = Turn.Player;
        UpdateTurnDisplay();
    }

    IEnumerator RollAndCheck(string who)
    {
        List<int> results = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            Roll();
            yield return new WaitForSeconds(2.0f); // 停止待ち
            int result = GetDiceValueByY();
            results.Add(result);

            // 出目1個ずつ表示（オプション）
            Debug.Log($"{who}の{i+1}投目：{result}");
        }

        // 3つの出目をまとめて表示
        string resultStr = string.Join(",", results);
        // if (resultText != null)
        //     resultText.text = $"{who}の出目：{resultStr}";

        string yaku = GetYakuName(results);

        if (resultText != null)
            resultText.text = $"{who}の出目：{resultStr}\n役：{yaku}";

        yield break;
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

}
