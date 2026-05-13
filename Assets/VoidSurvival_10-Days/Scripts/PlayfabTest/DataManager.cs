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

    private GameMasterDataRoot _rootData;
    private bool _isDataLoaded = false;
    private int _dataStartOffset = 0; 

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "148482";
        LoadDataFromPlayFab();
    }

    void Update()
    {
        if (!_isDataLoaded || _rootData == null || TimeManager.Instance == null) return;

        int gameElapsedMinutes = (TimeManager.Instance.GetHour() * 60) + TimeManager.Instance.GetMinute();
        int targetTotalSec = _dataStartOffset + gameElapsedMinutes;

        // 全てのセンサーデータを現在時刻に合わせて検索
        var cGyro = _rootData.Gyro?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag1 = _rootData.Mag1?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag2 = _rootData.Mag2?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cMag3 = _rootData.Mag3?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        var cRad  = _rootData.Radiation?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();

        UpdateUI(cGyro, cMag1, cMag2, cMag3, cRad);
    }

    private void UpdateUI(GyroData gyro, MagData m1, MagData m2, MagData m3, RadiationData rad)
    {
        if (dataValueText == null) return;

        string displayText = "<color=#FFFF00>--- Sensor Data ---</color>\n";

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
                        Debug.Log("★全センサーデータ（Mag3つ含む）読み込み完了！");
                    }
                }
            }, null);
        }, null);
    }
}