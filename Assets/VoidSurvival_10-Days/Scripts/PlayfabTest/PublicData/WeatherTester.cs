using UnityEngine;

// ============================================================
//  WeatherTester.cs   ★動作確認用（PlayFab・データ不要）
//  Inspectorのスライダーで生値を作り WeatherManager に流し込み、
//  天気が変わるのを Console で確認する。確認後は外してOK。
//
//  使い方：適当な空GameObjectに付けて Play → スライダー or スペースキー
// ============================================================

public class WeatherTester : MonoBehaviour
{
    [Header("テスト用の生値（スライダーで動かす）")]
    [Range(0, 90)]      public float radiationSignal = 10f;
    [Range(0, 3)]       public float gyroMagnitude   = 0.2f;
    [Range(0, 100000)]  public float magMagnitude    = 10000f;

    [Header("オンにすると毎フレーム適用し続ける")]
    public bool applyContinuously = false;

    private void Update()
    {
        if (WeatherManager.Instance == null) return;

        if (applyContinuously || Input.GetKeyDown(KeyCode.Space))
            Apply();
    }

    private void Apply()
    {
        var wm = WeatherManager.Instance;
        wm.ApplyRawValues(radiationSignal, gyroMagnitude, magMagnitude);
        Debug.Log($"[Tester] Rad={wm.RadiationLevel} / Gyro={wm.GyroLevel} / Mag={wm.MagLevel}  → 天気: {wm.CurrentWeather}");
    }

    [ContextMenu("いまの値で天気を評価")]
    private void EvaluateFromMenu() => Apply();
}
