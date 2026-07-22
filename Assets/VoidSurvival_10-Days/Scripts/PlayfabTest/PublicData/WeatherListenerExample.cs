using UnityEngine;

// ============================================================
//  WeatherListenerExample.cs
//  「他のスクリプトから天気を使う側」のお手本。
//  これを参考に、敵スポーン・BGM切替・空の色変更などを各スクリプトで実装する。
//  どんなGameObjectに付けてもOK。
// ============================================================

public class WeatherListenerExample : MonoBehaviour
{
    private void Start()
    {
        // 起動時点の天気を一度反映しておく（途中から購読しても現状に追いつける）
        if (WeatherManager.Instance != null)
        {
            HandleWeatherChanged(WeatherManager.Instance.CurrentWeather);
            WeatherManager.Instance.OnWeatherChanged += HandleWeatherChanged;
        }
    }

    private void OnDestroy()
    {
        // 購読解除を忘れるとエラーやメモリリークの原因になるので必ず外す
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= HandleWeatherChanged;
    }

    // 天気が変わるたびに呼ばれる
    private void HandleWeatherChanged(Weather weather)
    {
        switch (weather)
        {
            case Weather.Sunny:
                Debug.Log("天気：晴れ → 敵少なめ / 明るいBGM");
                break;
            case Weather.Cloudy:
                Debug.Log("天気：曇り");
                break;
            case Weather.Rainy:
                Debug.Log("天気：雨 → 視界悪化 / 移動速度ダウン など");
                break;
            case Weather.Thunderstorm:
                Debug.Log("天気：雷雨 → 敵強化 / 放射線ダメージ増 など");
                break;
        }

        // 現在のレベルも個別に読める：
        // var rad = WeatherManager.Instance.RadiationLevel; // Low/Mid/High
    }
}
