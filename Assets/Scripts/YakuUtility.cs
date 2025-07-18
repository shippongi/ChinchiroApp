using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YakuUtility
{
    public static string GetYakuName(List<int> results)
    {
        if (results == null || results.Count != 3)
            return "不正な出目";

        results.Sort();
        int a = results[0], b = results[1], c = results[2];

        if (a == 1 && b == 1 && c == 1) return "ピンゾロ";
        if (a == b && b == c) return $"アラシ({a}）";
        if (a == 4 && b == 5 && c == 6) return "シゴロ";
        if (a == 1 && b == 2 && c == 3) return "ヒフミ";

        if (a == b) return $"目あり：{c}";
        if (b == c) return $"目あり：{a}";
        if (a == c) return $"目あり：{b}";

        return "目無し";
    }

    public static int GetYakuStrength(string yaku)
    {
        if (yaku.Contains("ピンゾロ")) return 100;
        if (yaku.Contains("アラシ")) return 90;
        if (yaku.Contains("シゴロ")) return 80;
        if (yaku.Contains("目あり"))
        {
            if (int.TryParse(yaku.Split('：')[1], out int v))
                return 50 + v;
            return 50;
        }
        if (yaku.Contains("目無し")) return 30;
        if (yaku.Contains("ヒフミ")) return 10;
        return 0;
    }

    public static int GetYakuMultiplier(string yaku)
    {
        if (yaku.Contains("ピンゾロ")) return 5;
        if (yaku.Contains("アラシ")) return 3;
        if (yaku.Contains("シゴロ")) return 2;
        if (yaku.Contains("ヒフミ")) return 2;
        return 1;
    }

    public static int GetFinalMultiplier(string winnerYaku, string loserYaku)
    {
        int winnerMultiplier = GetYakuMultiplier(winnerYaku);
        int loserMultiplier = GetYakuMultiplier(loserYaku);

        if (winnerYaku.Contains("ヒフミ") || loserYaku.Contains("ヒフミ"))
        {
            return Mathf.Max(winnerMultiplier, loserMultiplier);
        }

        return winnerMultiplier;
    }

}

