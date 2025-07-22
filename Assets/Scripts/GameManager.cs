using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public DiceManager diceManager;
    public PlayerController playerController;
    public CardManager cardManager;

    private enum Turn { Player, CPU }
    private Turn currentTurn = Turn.Player;
    private bool isInputEnabled = true;
    private bool isTurnProcessing = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // シーン間をまたぐ場合
    }

    void Start()
    {
        ResetGame();
        if (SceneManager.GetActiveScene().name == "ExtraGameScene") // シーン名に合わせて
        {
            cardManager.ShowCardSelection();
        }

    }

    void ResetGame()
    {
        MoneyManager.Instance.ResetMoney();
        diceManager.ResetAll();

        uiManager.ClearAllTexts();
        uiManager.ShowRetryButton(false);
        uiManager.ShowRematchButton(false);

        currentTurn = Turn.Player;
        isInputEnabled = true;

        uiManager.UpdateTurnDisplay(true);
        uiManager.UpdateMoneyDisplay(
            MoneyManager.Instance.PlayerMoney,
            MoneyManager.Instance.CpuMoney,
            MoneyManager.Instance.CurrentBet
        );
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
        uiManager.ClearAllTexts();

        currentTurn = Turn.Player;
        isInputEnabled = true;

        uiManager.UpdateTurnDisplay(true);
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

        // プレイヤーのターン
        uiManager.UpdateTurnDisplay(true);
        DiceResult playerResult = new DiceResult();
        yield return StartCoroutine(RollAndCheck("あなた", res => playerResult = res));

        yield return new WaitForSeconds(1f);

        // CPUのターン
        currentTurn = Turn.CPU;
        uiManager.UpdateTurnDisplay(false);
        DiceResult cpuResult = new DiceResult();
        yield return StartCoroutine(RollAndCheck("CPU", res => cpuResult = res));

        yield return new WaitForSeconds(1f);

        // 勝敗判定とUI表示
        HandleTurnResult(playerResult, cpuResult);

        currentTurn = Turn.Player;
        uiManager.UpdateTurnDisplay(null); // "結果"表示へ
        isTurnProcessing = false;
    }

    void HandleTurnResult(DiceResult playerResult, DiceResult cpuResult)
    {
        int playerPower = YakuUtility.GetYakuStrength(playerResult.yaku);
        int cpuPower = YakuUtility.GetYakuStrength(cpuResult.yaku);

        string resultMsg;
        bool isDraw = playerPower == cpuPower;

        bool playerWins = playerPower > cpuPower;
        bool cpuWins = cpuPower > playerPower;

        if (isDraw)
        {
            resultMsg = "引き分け！";
            uiManager.ShowMatchResult(resultMsg); // 倍率なし
        }
        else
        {
            resultMsg = playerWins ? "あなたの勝ち！" : "CPUの勝ち！";

            // 倍率の取得（勝者の役に対して）
            string winnerYaku = playerWins ? playerResult.yaku : cpuResult.yaku;
            string loserYaku  = playerWins ? cpuResult.yaku : playerResult.yaku;

            int finalMultiplier = YakuUtility.GetFinalMultiplier(winnerYaku, loserYaku);

            Debug.Log(finalMultiplier);
            uiManager.ShowMatchResultWithMultiplier(resultMsg, finalMultiplier);
        }

        // 所持金の更新
        MoneyManager.Instance.ApplyResult(
            playerPower > cpuPower,
            isDraw,
            playerPower > cpuPower ? playerResult.yaku : cpuResult.yaku,
            playerPower > cpuPower ? cpuResult.yaku : playerResult.yaku
        );
        UpdateMoneyUI();

        // 終了処理
        EndTurn(resultMsg);
    }

    IEnumerator RollAndCheck(string who, System.Action<DiceResult> onComplete)
    {
        int rerollCount = 0;
        const int maxReroll = 2;

        DiceResult currentResult = new DiceResult();
        DiceResult finalResult = new DiceResult();

        // 初期表示のクリア（共通欄）
        uiManager.ResetResultText();

        while (true)
        {
            diceManager.RollAll();
            yield return StartCoroutine(WaitForDiceToStop(diceManager.GetAllDice()));

            var eyes = diceManager.GetAllResults();
            string yaku = YakuUtility.GetYakuName(eyes);
            currentResult = new DiceResult(eyes, yaku);

            // 共通：現在の出目表示（中間表示）
            uiManager.ShowMidRollResult(who, eyes, yaku);

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

        // 最終確定役の保存・表示
        uiManager.ShowFinalYakuResult(who, finalResult.eyes, finalResult.yaku);
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
            MoneyManager.Instance.PlayerMoney,
            MoneyManager.Instance.CpuMoney,
            MoneyManager.Instance.CurrentBet
        );

    }

    public void PurchaseCard(CardData card, bool isPlayer)
    {
        if (isPlayer)
        {
            if (MoneyManager.Instance.PlayerMoney >= card.price)
            {
                MoneyManager.Instance.SubtractMoneyFromPlayer(card.price);
                CardInventory.Instance.AddCardToPlayer(card);
                uiManager.UpdateCardDisplays();
                Debug.Log($"プレイヤーが {card.cardName} を購入。効果: {card.description}");
            }
            else
            {
                Debug.Log("プレイヤーの所持金が足りません");
            }
        }
        else
        {
            if (MoneyManager.Instance.CpuMoney >= card.price)
            {
                MoneyManager.Instance.SubtractMoneyFromCpu(card.price);
                CardInventory.Instance.AddCardToCpu(card);
                uiManager.UpdateCardDisplays();
                Debug.Log($"CPUが {card.cardName} を購入。効果: {card.description}");
            }
            else
            {
                Debug.Log("CPUの所持金が足りません（※基本的に到達しない想定）");
            }
        }
    }

    // public void PurchaseCard(CardData card)
    // {
    //     Debug.Log(MoneyManager.Instance.PlayerMoney);
    //     if (MoneyManager.Instance.PlayerMoney >= card.price)
    //     {
    //         MoneyManager.Instance.SubtractMoneyFromPlayer(card.price);

    //         // 所有カードリストに追加
    //         CardInventory.Instance.AddCardToPlayer(card);
    //         uiManager.UpdateCardDisplays();

    //         Debug.Log($"{card.cardName} を購入しました。効果: {card.description}");
    //     }
    //     else
    //     {
    //         Debug.Log("所持金が足りません");
    //     }
    // }

    void EndTurn(string resultMsg)
    {
        uiManager.ClearResultText();
        uiManager.HideRollButton();
        uiManager.ShowRetryButton(true);

        isInputEnabled = true;

        if (MoneyManager.Instance.IsGameOver())
        {
            uiManager.ShowRematchButton(true);
            uiManager.ShowRetryButton(false);
            uiManager.ShowResultText("所持金が尽きました！ゲーム終了！");
            isInputEnabled = false;
        }
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

    public void ChangeBet(bool increase)
    {
        if (increase)
            MoneyManager.Instance.IncreaseBet();
        else
            MoneyManager.Instance.DecreaseBet();

        UpdateMoneyUI();
    }
}