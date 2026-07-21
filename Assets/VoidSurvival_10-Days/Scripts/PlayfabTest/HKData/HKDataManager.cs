using UnityEngine;
using TMPro;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System.Linq;

public class HKDataManager : MonoBehaviour
{
    public enum DisplayMode
    {
        StepThrough,    // 5秒ごとにデータを1件ずつ順に表示（20個を再生）
        GameTimeSynced  // ゲーム内時刻に対応するデータを表示（元の挙動）※更新は5秒ごと
    }

    [Header("表示先")]
    [SerializeField] private TMP_Text dataValueText;

    [Header("動作設定")]
    [SerializeField] private DisplayMode displayMode = DisplayMode.StepThrough;
    [SerializeField] private float updateInterval = 5f; // ← 5秒に1回だけ変化
    [SerializeField] private bool loop = true;          // StepThrough：最後まで行ったら先頭へ

    [Header("表示する項目 =「特徴的なもの」だけ ON")]
    [SerializeField] private bool showGyro = true;
    [SerializeField] private bool showMag1 = false;
    [SerializeField] private bool showMag2 = false;
    [SerializeField] private bool showMag3 = true;
    [SerializeField] private bool showRadiation = true;

    private GameMasterDataRoot _rootData;
    private bool _isDataLoaded = false;
    private int _dataStartOffset = 0;

    private float _timer = 0f;
    private int _stepIndex = 0;

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "148482";
        LoadDataFromPlayFab();
    }

    void Update()
    {
        if (!_isDataLoaded || _rootData == null) return;

        // 5秒たまるまで何もしない → 5秒に1回だけ RefreshDisplay を呼ぶ
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer -= updateInterval;

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        GyroData gyro = null;
        MagData m1 = null, m2 = null, m3 = null;
        RadiationData rad = null;

        if (displayMode == DisplayMode.GameTimeSynced)
        {
            if (TimeManager.Instance == null) return;

            int gameElapsedMinutes = (TimeManager.Instance.GetHour() * 60) + TimeManager.Instance.GetMinute();
            int targetTotalSec = _dataStartOffset + gameElapsedMinutes;

            gyro = _rootData.Gyro?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
            m1 = _rootData.Mag1?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
            m2 = _rootData.Mag2?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
            m3 = _rootData.Mag3?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
            rad = _rootData.Radiation?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        }
        else // StepThrough：5秒ごとに次のデータへ
        {
            int count = _rootData.Gyro?.Count ?? 0;
            if (count == 0) return;

            gyro = GetAt(_rootData.Gyro, _stepIndex);
            m1 = GetAt(_rootData.Mag1, _stepIndex);
            m2 = GetAt(_rootData.Mag2, _stepIndex);
            m3 = GetAt(_rootData.Mag3, _stepIndex);
            rad = GetAt(_rootData.Radiation, _stepIndex);

            _stepIndex++;
            if (_stepIndex >= count)
                _stepIndex = loop ? 0 : count - 1;
        }

        UpdateUI(gyro, m1, m2, m3, rad);
    }

    // リストの長さが違っても安全に取り出す（短いリストは末尾を保持）
    private T GetAt<T>(List<T> list, int i) where T : class
    {
        if (list == null || list.Count == 0) return null;
        return list[Mathf.Clamp(i, 0, list.Count - 1)];
    }

    private void UpdateUI(GyroData gyro, MagData m1, MagData m2, MagData m3, RadiationData rad)
    {
        if (dataValueText == null) return;

        string displayText = "<color=#FFFF00>--- Sensor Data ---</color>\n";

        if (showGyro && gyro != null)
            displayText += $"<b>[Gyro]</b>\nGX: {gyro.gx:F2} GY: {gyro.gy:F2} GZ: {gyro.gz:F2}\n";
        if (showMag1 && m1 != null)
            displayText += $"<b>[Mag1]</b>\nMX: {m1.mx:F0} MY: {m1.my:F0} MZ: {m1.mz:F0}\n";
        if (showMag2 && m2 != null)
            displayText += $"<b>[Mag2]</b>\nMX: {m2.mx:F0} MY: {m2.my:F0} MZ: {m2.mz:F0}\n";
        if (showMag3 && m3 != null)
            displayText += $"<b>[Mag3]</b>\nMX: {m3.mx:F0} MY: {m3.my:F0} MZ: {m3.mz:F0}\n";
        if (showRadiation && rad != null)
            displayText += $"<b>[Radiation]</b>\nSignal: {rad.SignalCount} Noise: {rad.NoiseCount}";

        dataValueText.text = displayText;
    }

    private void LoadDataFromPlayFab()
    {
        var loginRequest = new LoginWithCustomIDRequest { CustomId = "VoidSurvival_User", CreateAccount = false };
        PlayFabClientAPI.LoginWithCustomID(loginRequest, loginRes => {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result => {
                if (result.Data.ContainsKey("GameMasterData"))
                {
                    string jsonStr = result.Data["GameMasterData"];
                    _rootData = JsonConvert.DeserializeObject<GameMasterDataRoot>(jsonStr);
                    if (_rootData != null && _rootData.Gyro != null && _rootData.Gyro.Count > 0)
                    {
                        _dataStartOffset = _rootData.Gyro[0].TotalSeconds;
                        _isDataLoaded = true;
                        _timer = updateInterval; // 読み込み直後にすぐ1回表示させる
                        Debug.Log("★全センサーデータ（Mag3つ含む）読み込み完了！");
                    }
                }
            }, null);
        }, null);
    }
}
