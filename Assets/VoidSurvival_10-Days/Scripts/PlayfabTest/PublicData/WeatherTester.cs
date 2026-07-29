using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ============================================================
//  WeatherTester.cs
//  実データ(DataManager/HKDataDisplay)のLOW/NORMAL/HIGH状態から
//  Inspectorで選んだ3項目を見て天気を決める簡易ルールベース判定。
//  天気が変わるたびに OnWeatherChanged で通知する（WeatherDisplayが購読）。
// ============================================================

public class WeatherTester : MonoBehaviour
{
    public enum DataSourceType
    {
        Gyro,
        Mag1,
        Mag2,
        Mag3,
        Radiation,
        Temp,
        Voltage,
        Current,
    }

    [Header("実データ連動の天気判定")]
    [SerializeField] private DataManager dataManager;
    [SerializeField] private HKDataDisplay hkDataDisplay;

    [Header("判定に使う3項目（重複しないように選択）")]
    [SerializeField] private DataSourceType[] selectedSources = new DataSourceType[3]
    {
        DataSourceType.Radiation,
        DataSourceType.Mag1,
        DataSourceType.Temp,
    };

    public Weather CurrentRuleBasedWeather { get; private set; } = Weather.Sunny;

    public event Action<Weather> OnWeatherChanged;

    private void LateUpdate()
    {
        if (dataManager == null && hkDataDisplay == null) return;

        var states = CollectSelectedStates();
        if (states.Count == 0) return;

        Weather weather = EvaluateWeather(states);
        if (weather != CurrentRuleBasedWeather)
        {
            CurrentRuleBasedWeather = weather;
            Debug.Log($"[WeatherTester] states=[{string.Join(", ", states)}] → Weather: {weather}");
            OnWeatherChanged?.Invoke(weather);
        }
    }

    private List<string> CollectSelectedStates()
    {
        var states = new List<string>();

        foreach (var source in selectedSources)
        {
            string state = GetState(source);
            if (state != null) states.Add(state);
        }

        return states;
    }

    private string GetState(DataSourceType source)
    {
        switch (source)
        {
            case DataSourceType.Gyro: return dataManager != null ? dataManager.GyroState : null;
            case DataSourceType.Mag1: return dataManager != null ? dataManager.Mag1State : null;
            case DataSourceType.Mag2: return dataManager != null ? dataManager.Mag2State : null;
            case DataSourceType.Mag3: return dataManager != null ? dataManager.Mag3State : null;
            case DataSourceType.Radiation: return dataManager != null ? dataManager.RadiationState : null;
            case DataSourceType.Temp: return hkDataDisplay != null ? hkDataDisplay.TempState : null;
            case DataSourceType.Voltage: return hkDataDisplay != null ? hkDataDisplay.VoltageState : null;
            case DataSourceType.Current: return hkDataDisplay != null ? hkDataDisplay.CurrentState : null;
            default: return null;
        }
    }

    // 2つ以上HIGH→Thunderstorm、1つHIGH→Rainy、HIGHなし かつ 2つ以上LOW→Sunny、それ以外→Cloudy
    private Weather EvaluateWeather(List<string> states)
    {
        int highCount = states.Count(s => s == "HIGH");
        int lowCount = states.Count(s => s == "LOW");

        if (highCount >= 2) return Weather.Thunderstorm;
        if (highCount == 1) return Weather.Rainy;
        if (lowCount >= 2) return Weather.Sunny;
        return Weather.Cloudy;
    }
}
