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
        Roll();

        yield return new WaitForSeconds(2.0f); // 転がり待ち（調整可能）

        int result = GetDiceValueByY();
        Debug.Log(who + " の出目：" + result);

        if (resultText != null)
            resultText.text = $"{who}の出目：{result}";

        yield break;
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
