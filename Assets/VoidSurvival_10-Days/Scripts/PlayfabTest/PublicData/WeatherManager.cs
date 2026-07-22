using System;
using UnityEngine;

// ============================================================
//  WeatherManager.cs   ★このシステムの中心（シングルトン）【方法B用】
//  データ取得元 = WeatherDataManager（元のDataManagerは触らない構成）
//
//  - 全スクリプトから WeatherManager.Instance で現在の天気を読める
//  - 天気が変わった瞬間に OnWeatherChanged で全体に通知
//  - ゲーム内時間が1時間進むごとに再評価
//
//  セットアップ：
//    1. 空GameObjectに付ける
//    2. Inspectorの3欄に SensorThreshold アセットを割り当てる
// ============================================================

[DefaultExecutionOrder(100)] // WeatherDataManager が値を更新した後に評価
public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("しきい値設定 (SensorThreshold アセットを割り当てる)")]
    [SerializeField] private SensorThreshold radiationThreshold;
    [SerializeField] private SensorThreshold gyroThreshold;
    [SerializeField] private SensorThreshold magThreshold;

    public Weather CurrentWeather { get; private set; } = Weather.Sunny;
    public Level RadiationLevel { get; private set; }
    public Level GyroLevel { get; private set; }
    public Level MagLevel { get; private set; }

    public event Action<Weather> OnWeatherChanged;

    private int _lastEvaluatedHour = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 不要なら消してOK
    }

    private void Update()
    {
        if (TimeManager.Instance == null) return;
        // ★方法B：データ元は WeatherDataManager
        if (WeatherDataManager.Instance == null || !WeatherDataManager.Instance.IsDataLoaded) return;

        int hour = TimeManager.Instance.GetHour();
        if (hour == _lastEvaluatedHour) return;
        _lastEvaluatedHour = hour;

        Evaluate();
    }

    /// <summary>WeatherDataManager から現在のセンサー値を取得して評価する（本番経路）。</summary>
    private void Evaluate()
    {
        var dm = WeatherDataManager.Instance; // ★方法B

        float radValue  = (dm.CurrentRadiation != null) ? dm.CurrentRadiation.SignalCount : 0f;
        float gyroValue = SensorClassifier.GyroMagnitude(dm.CurrentGyro);
        float magValue  = SensorClassifier.AverageMagMagnitude(dm.CurrentMag1, dm.CurrentMag2, dm.CurrentMag3);

        ApplyRawValues(radValue, gyroValue, magValue);
    }

    /// <summary>
    /// 生値を受け取って「分類 → 天気合成 → 通知」を行う共通処理。
    /// 本番経路（Evaluate）からも、テスト（WeatherTester）からも呼ばれる。
    /// </summary>
    public void ApplyRawValues(float radValue, float gyroValue, float magValue)
    {
        if (radiationThreshold == null || gyroThreshold == null || magThreshold == null)
        {
            Debug.LogError("[WeatherManager] SensorThreshold が未割り当てです。Inspectorで3つ設定してください。");
            return;
        }

        RadiationLevel = radiationThreshold.Classify(radValue);
        GyroLevel      = gyroThreshold.Classify(gyroValue);
        MagLevel       = float.IsNaN(magValue) ? Level.Low : magThreshold.Classify(magValue);

        Weather next = WeatherCalculator.Calculate(RadiationLevel, GyroLevel, MagLevel);

        if (next != CurrentWeather)
        {
            CurrentWeather = next;
            OnWeatherChanged?.Invoke(next);
        }
    }

    /// <summary>外部から本番経路を手動で再評価したいとき用。</summary>
    public void ForceEvaluate()
    {
        if (WeatherDataManager.Instance != null && WeatherDataManager.Instance.IsDataLoaded)
            Evaluate();
    }
}
