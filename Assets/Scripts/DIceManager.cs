using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private Dice[] diceArray;

    [Header("初期位置設定")]
    [SerializeField] private Vector3 basePosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private float spacing = 1.0f;

    void Start()
    {
        int count = diceArray.Length;
        float startX = basePosition.x - ((count - 1) * spacing / 2f);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(startX + i * spacing, basePosition.y, basePosition.z);
            diceArray[i].SetInitialPosition(pos);
        }
    }

    public void RollAll() => diceArray.ToList().ForEach(d => d.Roll());

    public List<int> GetAllResults() => diceArray.Select(d => d.GetValue()).ToList();

    public List<Dice> GetAllDice() => diceArray.ToList();

    public void ResetAll() => diceArray.ToList().ForEach(d => d.ResetPosition());
}