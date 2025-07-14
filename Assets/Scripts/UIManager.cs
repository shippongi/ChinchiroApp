using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text turnText;
    public GameObject rollButton;
    public Button retryButton;

    public void UpdateTurnDisplay(bool isPlayerTurn)
    {
        if (turnText != null)
            turnText.text = isPlayerTurn ? "あなたの番です" : "CPUの番です";

        if (rollButton != null)
            rollButton.SetActive(isPlayerTurn);
    }

    public void ShowRetryButton(bool show)
    {
        retryButton.gameObject.SetActive(show);
    }

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

    public bool IsRollButtonActive()
    {
        return rollButton != null && rollButton.activeSelf;
    }

    public void HideRollButton()
    {
        if (rollButton != null)
            rollButton.SetActive(false);
    }


}
