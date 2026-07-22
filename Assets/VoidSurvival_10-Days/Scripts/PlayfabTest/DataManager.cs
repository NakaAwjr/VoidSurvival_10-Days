using UnityEngine;
using TMPro;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System.Linq;

public class DataManager : MonoBehaviour
{
    [SerializeField] private TMP_Text dataValueText;

    [Tooltip("データ進行の速度係数。1=通常速度、0.5=2倍遅い、2=2倍速い")]
    [SerializeField] private float speedFactor = 0.5f;

    [Tooltip("ONで、生の数値の代わりにHIGH/NORMAL/LOW状態表示に切り替える")]
    [SerializeField] private bool useStateDisplay = false;

    [Tooltip("ONで最後のデータまで行ったら先頭に戻ってループする。OFFなら最後のデータで止まる")]
    [SerializeField] private bool loop = false;

    // 現在のセンサー状態（LOW/NORMAL/HIGH）。WeatherTester等の外部スクリプトから参照する。
    public string GyroState { get; private set; } = "NORMAL";
    public string Mag1State { get; private set; } = "NORMAL";
    public string Mag2State { get; private set; } = "NORMAL";
    public string Mag3State { get; private set; } = "NORMAL";
    public string RadiationState { get; private set; } = "NORMAL";

    private GameMasterDataRoot _rootData;
    private bool _isDataLoaded = false;
    private int _dataStartOffset = 0;
    private int _dataSpanSec = 0; // Gyroデータの最初〜最後のTotalSeconds差（ループの周期）

    // 状態判定用（データ全体から求めた範囲）
    private double _gyroMagMin, _gyroMagMax;
    private double _mag1MagMin, _mag1MagMax;
    private double _mag2MagMin, _mag2MagMax;
    private double _mag3MagMin, _mag3MagMax;
    private List<RadiationData> _radSorted;

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "148482";
        LoadDataFromPlayFab();
    }

    void Update()
    {
        if (!_isDataLoaded || _rootData == null || TimeManager.Instance == null) return;

        int gameElapsedMinutes = (TimeManager.Instance.GetHour() * 60) + TimeManager.Instance.GetMinute();
        int rawElapsedSec = Mathf.FloorToInt(gameElapsedMinutes * speedFactor);

        if (loop && _dataSpanSec > 0)
            rawElapsedSec %= (_dataSpanSec + 1);

        int targetTotalSec = _dataStartOffset + rawElapsedSec;

        // 全てのセンサーデータを現在時刻に合わせて検索
        var cGyro = _rootData.Gyro?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag1 = _rootData.Mag1?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag2 = _rootData.Mag2?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag3 = _rootData.Mag3?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cRad = _rootData.Radiation?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();

        RadiationData prevRad = null;
        if (cRad != null && _radSorted != null)
        {
            int idx = _radSorted.IndexOf(cRad);
            if (idx > 0) prevRad = _radSorted[idx - 1];
        }

        UpdateStates(cGyro, cMag1, cMag2, cMag3, cRad, prevRad);
        UpdateUI(cGyro, cMag1, cMag2, cMag3, cRad, prevRad);
    }

    // 表示モード(useStateDisplay)に関わらず、他スクリプトが参照できるよう常に状態を更新しておく
    private void UpdateStates(GyroData gyro, MagData m1, MagData m2, MagData m3, RadiationData rad, RadiationData prevRad)
    {
        if (gyro != null) GyroState = Classify(Magnitude(gyro), _gyroMagMin, _gyroMagMax);
        if (m1 != null) Mag1State = Classify(Magnitude(m1), _mag1MagMin, _mag1MagMax);
        if (m2 != null) Mag2State = Classify(Magnitude(m2), _mag2MagMin, _mag2MagMax);
        if (m3 != null) Mag3State = Classify(Magnitude(m3), _mag3MagMin, _mag3MagMax);

        if (rad != null)
            RadiationState = (prevRad == null) ? "LOW" : ClassifyChange(Mathf.Abs(rad.SignalCount - prevRad.SignalCount));
    }

    private void UpdateUI(GyroData gyro, MagData m1, MagData m2, MagData m3, RadiationData rad, RadiationData prevRad)
    {
        if (dataValueText == null) return;

        string displayText = "<color=#FFFF00>--- Sensor Data ---</color>\n";

        if (useStateDisplay)
        {
            if (gyro != null)
                displayText += $"<b>[Gyro]</b>\nValue: {Magnitude(gyro):F2} <color={StateColor(GyroState)}>[{GyroState}]</color>\n";
            if (m1 != null)
                displayText += $"<b>[Mag1]</b>\nValue: {Magnitude(m1):F2} <color={StateColor(Mag1State)}>[{Mag1State}]</color>\n";
            if (m2 != null)
                displayText += $"<b>[Mag2]</b>\nValue: {Magnitude(m2):F2} <color={StateColor(Mag2State)}>[{Mag2State}]</color>\n";
            if (m3 != null)
                displayText += $"<b>[Mag3]</b>\nValue: {Magnitude(m3):F2} <color={StateColor(Mag3State)}>[{Mag3State}]</color>\n";
            if (rad != null)
            {
                int diff = (prevRad == null) ? 0 : Mathf.Abs(rad.SignalCount - prevRad.SignalCount);
                displayText += $"<b>[Radiation]</b>\nSignal: {rad.SignalCount} (Δ{diff})  Noise: {rad.NoiseCount} <color={StateColor(RadiationState)}>[{RadiationState}]</color>";
            }
        }
        else
        {
            if (gyro != null)
                displayText += $"<b>[Gyro]</b>\nGX: {gyro.gx:F2} GY: {gyro.gy:F2} GZ: {gyro.gz:F2}\n";

            if (m1 != null)
                displayText += $"<b>[Mag1]</b>\nMX: {m1.mx:F0} MY: {m1.my:F0} MZ: {m1.mz:F0}\n";
            if (m2 != null)
                displayText += $"<b>[Mag2]</b>\nMX: {m2.mx:F0} MY: {m2.my:F0} MZ: {m2.mz:F0}\n";
            if (m3 != null)
                displayText += $"<b>[Mag3]</b>\nMX: {m3.mx:F0} MY: {m3.my:F0} MZ: {m3.mz:F0}\n";

            if (rad != null)
                displayText += $"<b>[Radiation]</b>\nSignal: {rad.SignalCount} Noise: {rad.NoiseCount}";
        }

        dataValueText.text = displayText;
    }

    private static float Magnitude(GyroData g) => Mathf.Sqrt(g.gx * g.gx + g.gy * g.gy + g.gz * g.gz);
    private static float Magnitude(MagData m) => Mathf.Sqrt(m.mx * m.mx + m.my * m.my + m.mz * m.mz);

    // データ全体の最小〜最大を3分割して LOW / NORMAL / HIGH に判定（HKDataDisplayと同じ考え方）
    private string Classify(double v, double min, double max)
    {
        double span = max - min;
        if (span <= 0) return "NORMAL";
        if (v < min + span / 3.0) return "LOW";
        if (v > min + span * 2.0 / 3.0) return "HIGH";
        return "NORMAL";
    }

    private string StateColor(string s)
    {
        if (s == "HIGH") return "#FF6B6B";
        if (s == "LOW") return "#6BB5FF";
        return "#9EE09E"; // NORMAL
    }

    // Signal Countの前回値との差分から LOW(差分0) / NORMAL(差分1) / HIGH(差分2以上) を判定
    private string ClassifyChange(int diff)
    {
        if (diff <= 0) return "LOW";
        if (diff == 1) return "NORMAL";
        return "HIGH";
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
                        _dataSpanSec = _rootData.Gyro[_rootData.Gyro.Count - 1].TotalSeconds - _dataStartOffset;
                        ComputeStateRanges();
                        _isDataLoaded = true;
                        Debug.Log("★全センサーデータ（Mag3つ含む）読み込み完了！");
                    }
                }
            }, null);
        }, null);
    }

    // HIGH/NORMAL/LOW判定・Radiationの変化判定に使う範囲を、データ全体から一度だけ求めておく
    private void ComputeStateRanges()
    {
        if (_rootData.Gyro != null && _rootData.Gyro.Count > 0)
        {
            var mags = _rootData.Gyro.Select(Magnitude).ToList();
            _gyroMagMin = mags.Min(); _gyroMagMax = mags.Max();
        }
        if (_rootData.Mag1 != null && _rootData.Mag1.Count > 0)
        {
            var mags = _rootData.Mag1.Select(Magnitude).ToList();
            _mag1MagMin = mags.Min(); _mag1MagMax = mags.Max();
        }
        if (_rootData.Mag2 != null && _rootData.Mag2.Count > 0)
        {
            var mags = _rootData.Mag2.Select(Magnitude).ToList();
            _mag2MagMin = mags.Min(); _mag2MagMax = mags.Max();
        }
        if (_rootData.Mag3 != null && _rootData.Mag3.Count > 0)
        {
            var mags = _rootData.Mag3.Select(Magnitude).ToList();
            _mag3MagMin = mags.Min(); _mag3MagMax = mags.Max();
        }

        if (_rootData.Radiation != null && _rootData.Radiation.Count > 0)
            _radSorted = _rootData.Radiation.OrderBy(r => r.TotalSeconds).ToList();
    }
}