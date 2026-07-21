// ============================================================
//  WeatherCalculator.cs
//  「各レベル → 天気」を決める純粋ロジック。
//  UnityEngine に依存しないので、将来 別プロジェクトやサーバーにも流用できる。
//  バランス調整はここ（点数の重み・境界）を触ればOK。
// ============================================================

public static class WeatherCalculator
{
    /// <summary>
    /// 放射線・ジャイロ・地磁気の3レベルを点数化して天気を決める。
    /// 放射線を一番重く（×2）している。点数の範囲は 0〜8。
    /// </summary>
    public static Weather Calculate(Level radiation, Level gyro, Level mag)
    {
        int score = (int)radiation * 2  // 放射線：0,2,4（重み2）
                  + (int)gyro           // ジャイロ：0,1,2
                  + (int)mag;           // 地磁気：0,1,2   → 合計 0〜8

        if (score >= 7) return Weather.Thunderstorm; // 雷雨
        if (score >= 5) return Weather.Rainy;        // 雨
        if (score >= 2) return Weather.Cloudy;       // 曇り
        return Weather.Sunny;                        // 晴れ
    }

    // ----------------------------------------------------------------
    // ＜ルールベースにしたい場合の例＞
    // 「放射線が High なら他に関係なく雷雨」のような演出を作り込みたいときは、
    // 上を消してこちらのような書き方にする：
    //
    // public static Weather Calculate(Level radiation, Level gyro, Level mag)
    // {
    //     if (radiation == Level.High) return Weather.Thunderstorm;
    //     if (radiation == Level.Mid && mag == Level.High) return Weather.Rainy;
    //     if (gyro == Level.High || mag == Level.Mid) return Weather.Cloudy;
    //     return Weather.Sunny;
    // }
    // ----------------------------------------------------------------
}
