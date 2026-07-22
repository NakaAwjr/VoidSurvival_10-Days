using UnityEngine;

// ============================================================
//  SensorClassifier.cs
//  生のセンサーデータ（3軸など）を「1つの数値(大きさ)」に変換する
//  純粋なロジック置き場。状態を持たない static クラス。
//  ★地磁気の -1,600,000 のようなエラー値（センチネル値）をここで除外する。
// ============================================================

public static class SensorClassifier
{
    // -1,600,000 のようなエラー値を弾くためのしきい。これより小さければ無効とみなす。
    private const float MagErrorSentinel = -1000000f;

    /// <summary>ジャイロ3軸の大きさ（ベクトルの長さ）。</summary>
    public static float GyroMagnitude(GyroData g)
    {
        if (g == null) return 0f;
        return Mathf.Sqrt(g.gx * g.gx + g.gy * g.gy + g.gz * g.gz);
    }

    /// <summary>
    /// 地磁気3軸の大きさ。エラー値が含まれていたら無効(NaN)を返す。
    /// </summary>
    public static float MagMagnitude(MagData m)
    {
        if (m == null) return float.NaN;
        if (m.mx <= MagErrorSentinel || m.my <= MagErrorSentinel || m.mz <= MagErrorSentinel)
            return float.NaN; // エラー値が混ざっているので無効扱い
        return Mathf.Sqrt(m.mx * m.mx + m.my * m.my + m.mz * m.mz);
    }

    /// <summary>
    /// Mag1/Mag2/Mag3 のうち「有効なものだけ」の平均の大きさ。
    /// 全部無効なら NaN を返す。
    /// </summary>
    public static float AverageMagMagnitude(params MagData[] mags)
    {
        float sum = 0f;
        int count = 0;
        foreach (var m in mags)
        {
            float v = MagMagnitude(m);
            if (!float.IsNaN(v))
            {
                sum += v;
                count++;
            }
        }
        return count == 0 ? float.NaN : sum / count;
    }
}
