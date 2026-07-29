using UnityEngine;
using TMPro;

// ============================================================
//  WeatherDisplay.cs   ★現在の天気を画面に表示する
//  WeatherTester（DataManager連携のルールベース判定）の
//  OnWeatherChanged を購読し、変化のたびに更新。
//
//  ★日本語が □（豆腐）になる場合：
//    使っているTMPフォントに日本語が含まれていません。
//    ・すぐ確認したい → useJapanese のチェックを外す（英語表記になる）
//    ・日本語を出したい → 日本語フォントアセットを作って割り当てる
//                        （Window > TextMeshPro > Font Asset Creator）
// ============================================================

public class WeatherDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text weatherText;
    [SerializeField] private WeatherTester weatherTester;

    [Header("日本語で表示する（フォントが日本語対応のときだけON）")]
    [SerializeField] private bool useJapanese = false; // ★まずは英語(false)で確認

    [Header("天気名に色を付ける")]
    [SerializeField] private bool useColor = true;

    private void Start()
    {
        if (weatherTester != null)
        {
            Refresh(weatherTester.CurrentRuleBasedWeather);
            weatherTester.OnWeatherChanged += Refresh;
        }
        else if (weatherText != null)
        {
            weatherText.text = "Weather: (no WeatherTester)";
        }
    }

    private void OnDestroy()
    {
        if (weatherTester != null)
            weatherTester.OnWeatherChanged -= Refresh;
    }

    private void Refresh(Weather weather)
    {
        if (weatherText == null) return;

        string label  = useJapanese ? ToJapanese(weather) : ToEnglish(weather);
        string prefix = useJapanese ? "天気: " : "Weather: ";

        if (useColor)
        {
            string hex = ToColorHex(weather);
            weatherText.text = $"{prefix}<color={hex}>{label}</color>";
        }
        else
        {
            weatherText.text = $"{prefix}{label}";
        }
    }

    private string ToJapanese(Weather w)
    {
        switch (w)
        {
            case Weather.Sunny:        return "晴れ";
            case Weather.Cloudy:       return "曇り";
            case Weather.Rainy:        return "雨";
            case Weather.Thunderstorm: return "雷雨";
            default:                   return "不明";
        }
    }

    private string ToEnglish(Weather w)
    {
        switch (w)
        {
            case Weather.Sunny:        return "Sunny";
            case Weather.Cloudy:       return "Cloudy";
            case Weather.Rainy:        return "Rainy";
            case Weather.Thunderstorm: return "Thunderstorm";
            default:                   return "Unknown";
        }
    }

    private string ToColorHex(Weather w)
    {
        switch (w)
        {
            case Weather.Sunny:        return "#FFD23F";
            case Weather.Cloudy:       return "#AAAAAA";
            case Weather.Rainy:        return "#4FA8FF";
            case Weather.Thunderstorm: return "#B36BFF";
            default:                   return "#FFFFFF";
        }
    }
}
