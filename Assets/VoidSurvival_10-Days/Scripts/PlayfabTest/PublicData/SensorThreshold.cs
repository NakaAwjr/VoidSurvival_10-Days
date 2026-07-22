using UnityEngine;

// ============================================================
//  SensorThreshold.cs
//  「値 → 低/中/高」の境界値を持つ設定アセット（ScriptableObject）。
//  コードを触らず Inspector 上で数値だけ調整できるのが利点。
//  使い方：Projectビューで右クリック →
//          Create → VoidSurvival → SensorThreshold
//  でアセットを3つ作る（放射線用・ジャイロ用・地磁気用）。
// ============================================================

[CreateAssetMenu(menuName = "VoidSurvival/SensorThreshold", fileName = "NewSensorThreshold")]
public class SensorThreshold : ScriptableObject
{
    [Tooltip("この値以上で「中(Mid)」になる")]
    public float midThreshold = 1f;

    [Tooltip("この値以上で「高(High)」になる")]
    public float highThreshold = 2f;

    /// <summary>値を Low / Mid / High の3段階に変換する。</summary>
    public Level Classify(float value)
    {
        if (value >= highThreshold) return Level.High;
        if (value >= midThreshold)  return Level.Mid;
        return Level.Low;
    }
}
