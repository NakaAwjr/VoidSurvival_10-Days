using System.Collections.Generic;
using System.Linq;
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

    // ============================================================
    //  ここから：実データ(DataManager/HKDataDisplay)のLOW/NORMAL/HIGH状態
    //  から、選んだ3つを見て天気を決める簡易ルールベース判定。
    //  ちょうど3つチェックしてください。
    // ============================================================

    [Header("実データ連動の天気判定（8項目からちょうど3つ選ぶ）")]
    [SerializeField] private DataManager dataManager;
    [SerializeField] private HKDataDisplay hkDataDisplay;

    [SerializeField] private bool useGyro;
    [SerializeField] private bool useMag1;
    [SerializeField] private bool useMag2;
    [SerializeField] private bool useMag3;
    [SerializeField] private bool useRadiation;
    [SerializeField] private bool useTemp;
    [SerializeField] private bool useVoltage;
    [SerializeField] private bool useCurrent;

    public string CurrentRuleBasedWeather { get; private set; } = "Sunny";

    private void LateUpdate()
    {
        if (dataManager == null && hkDataDisplay == null) return;

        var states = CollectSelectedStates();
        if (states.Count == 0) return;

        string weather = EvaluateWeather(states);
        if (weather != CurrentRuleBasedWeather)
        {
            CurrentRuleBasedWeather = weather;
            Debug.Log($"[WeatherTester] states=[{string.Join(", ", states)}] → Weather: {weather}");
        }
    }

    private List<string> CollectSelectedStates()
    {
        var states = new List<string>();

        if (useGyro && dataManager != null) states.Add(dataManager.GyroState);
        if (useMag1 && dataManager != null) states.Add(dataManager.Mag1State);
        if (useMag2 && dataManager != null) states.Add(dataManager.Mag2State);
        if (useMag3 && dataManager != null) states.Add(dataManager.Mag3State);
        if (useRadiation && dataManager != null) states.Add(dataManager.RadiationState);
        if (useTemp && hkDataDisplay != null) states.Add(hkDataDisplay.TempState);
        if (useVoltage && hkDataDisplay != null) states.Add(hkDataDisplay.VoltageState);
        if (useCurrent && hkDataDisplay != null) states.Add(hkDataDisplay.CurrentState);

        return states;
    }

    // 2つ以上HIGH→Thunderstorm、1つHIGH→Rain、HIGHなし かつ 2つ以上LOW→Sunny、それ以外→Cloudy
    private string EvaluateWeather(List<string> states)
    {
        int highCount = states.Count(s => s == "HIGH");
        int lowCount = states.Count(s => s == "LOW");

        if (highCount >= 2) return "Thunderstorm";
        if (highCount == 1) return "Rain";
        if (lowCount >= 2) return "Sunny";
        return "Cloudy";
    }
}
