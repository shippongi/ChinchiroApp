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

        if (a == 1 && b == 1 && c == 1) return "ピンゾロ（最強）";
        if (a == b && b == c) return $"ゾロ目（{a}）";
        if (a == 4 && b == 5 && c == 6) return "シゴロ（強）";
        if (a == 1 && b == 2 && c == 3) return "ヒフミ（弱）";

        if (a == b) return $"目あり：{c}";
        if (b == c) return $"目あり：{a}";
        if (a == c) return $"目あり：{b}";

        return "目無し";
    }

    public static int GetYakuStrength(string yaku)
    {
        if (yaku.Contains("ピンゾロ")) return 100;
        if (yaku.Contains("ゾロ目")) return 90;
        if (yaku.Contains("シゴロ")) return 80;
        if (yaku.Contains("目あり"))
        {
            if (int.TryParse(yaku.Split('：')[1], out int v))
                return 10 + v;
            return 10;
        }
        if (yaku.Contains("ヒフミ")) return 5;
        return 0;
    }
}

