using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

/// <summary>
/// 俯瞰2Dアイソメトリックゲーム用の昼夜サイクルコントローラー。
/// ライトの位置は動かさず、Global Light 2D の色と強度だけを時刻で変化させる。
/// TimeManager.OnMinuteChanged を購読して動作する。
/// </summary>
public class SunController : MonoBehaviour
{
    [Header("--- ライト設定 ---")]
    [Tooltip("シーン全体のGlobal Light 2D")]
    [SerializeField] private Light2D globalLight;

    [Header("--- 時刻設定 ---")]
    [Tooltip("日の出の時刻（0〜24）")]
    [SerializeField] private float sunriseHour = 6f;
    [Tooltip("日の入りの時刻（0〜24）")]
    [SerializeField] private float sunsetHour = 18f;

    [Header("--- 明るさ設定 ---")]
    [Tooltip("昼間の最大強度")]
    [SerializeField] private float maxIntensity = 1.0f;
    [Tooltip("夜間の最小強度")]
    [SerializeField] private float minIntensity = 0.05f;
    [Tooltip("明るさの変化カーブ（横軸: 昼間の進行度 0〜1、縦軸: 強度の割合 0〜1）")]
    [SerializeField] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("--- 光の色 ---")]
    [Tooltip("昼間の色")]
    [SerializeField] private Color dayColor = new Color(1f, 0.97f, 0.88f);
    [Tooltip("夜間の色")]
    [SerializeField] private Color nightColor = new Color(0.04f, 0.05f, 0.18f);
    [Tooltip("日の出・日の入り時のオレンジ色")]
    [SerializeField] private Color horizonColor = new Color(1f, 0.5f, 0.15f);
    [Tooltip("日の出・日の入り遷移の長さ（昼間進行度の割合、0.1=10%）")]
    [SerializeField][Range(0.05f, 0.3f)] private float transitionWidth = 0.15f;

    [Header("--- イベント ---")]
    public UnityEvent onSunrise;
    public UnityEvent onSunset;

    // 内部状態
    private bool _isDay;
    private bool _sunriseEventFired;
    private bool _sunsetEventFired;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged.AddListener(UpdateSun);
        }
        else
        {
            Debug.LogWarning("[SunController] TimeManager.Instance が見つかりません。");
        }
        UpdateSun();
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged.RemoveListener(UpdateSun);
        }
    }

    public void UpdateSun()
    {
        if (TimeManager.Instance == null) return;

        float hour = TimeManager.Instance.GetHour() + TimeManager.Instance.GetMinute() / 60f;
        bool isDay = hour >= sunriseHour && hour <= sunsetHour;
        float dayProgress = isDay
            ? (hour - sunriseHour) / (sunsetHour - sunriseHour)
            : 0f;

        ApplyLight(isDay, dayProgress);
        FireEvents(isDay);
    }

    /// <summary>Global Light 2D の強度と色を更新する</summary>
    private void ApplyLight(bool isDay, float dayProgress)
    {
        if (globalLight == null) return;

        // 強度
        float blend = isDay ? intensityCurve.Evaluate(dayProgress) : 0f;
        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, blend);

        // 色
        globalLight.color = CalcSkyColor(isDay, dayProgress);
    }

    /// <summary>
    /// 夜 → オレンジ(日の出) → 昼間 → オレンジ(日の入り) → 夜 の色遷移
    /// </summary>
    private Color CalcSkyColor(bool isDay, float dayProgress)
    {
        if (!isDay) return nightColor;

        float tw = transitionWidth;

        if (dayProgress < tw)
            return Color.Lerp(nightColor, horizonColor, dayProgress / tw);
        else if (dayProgress < tw * 2f)
            return Color.Lerp(horizonColor, dayColor, (dayProgress - tw) / tw);
        else if (dayProgress > 1f - tw)
            return Color.Lerp(horizonColor, nightColor, (dayProgress - (1f - tw)) / tw);
        else if (dayProgress > 1f - tw * 2f)
            return Color.Lerp(dayColor, horizonColor, (dayProgress - (1f - tw * 2f)) / tw);
        else
            return dayColor;
    }

    /// <summary>日の出・日の入りイベントを一度だけ発火する</summary>
    private void FireEvents(bool isDay)
    {
        if (isDay && !_isDay)
        {
            _isDay = true;
            _sunriseEventFired = false;
        }
        else if (!isDay && _isDay)
        {
            _isDay = false;
            _sunsetEventFired = false;
        }

        if (_isDay && !_sunriseEventFired)
        {
            _sunriseEventFired = true;
            onSunrise?.Invoke();
        }
        if (!_isDay && !_sunsetEventFired)
        {
            _sunsetEventFired = true;
            onSunset?.Invoke();
        }
    }
}