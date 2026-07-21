using UnityEngine;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System.Linq;

// ============================================================
//  WeatherDataManager.cs （方法B：元DataManagerは触らず、別名で新設）
//  天気システム専用の「データ読み込み＆現在値の公開」係。
//  表示(UI)はしない（テキスト表示は元の DataManager がやるため）。
//  ※元DataManagerとは別に PlayFab から読み込む点に注意（読み込みが2回走る）。
//
//  WeatherManager 側の参照を DataManager → WeatherDataManager に変える必要あり（下の説明参照）。
// ============================================================

public class WeatherDataManager : MonoBehaviour
{
    public static WeatherDataManager Instance { get; private set; }

    private GameMasterDataRoot _rootData;
    private bool _isDataLoaded = false;
    private int _dataStartOffset = 0;

    // 現在時刻のセンサー値（WeatherManager がこれを読む）
    public GyroData CurrentGyro { get; private set; }
    public MagData CurrentMag1 { get; private set; }
    public MagData CurrentMag2 { get; private set; }
    public MagData CurrentMag3 { get; private set; }
    public RadiationData CurrentRadiation { get; private set; }
    public bool IsDataLoaded => _isDataLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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

        CurrentGyro      = _rootData.Gyro?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        CurrentMag1      = _rootData.Mag1?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        CurrentMag2      = _rootData.Mag2?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        CurrentMag3      = _rootData.Mag3?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
        CurrentRadiation = _rootData.Radiation?.Where(d => d.TotalSeconds <= targetTotalSec).OrderByDescending(d => d.TotalSeconds).FirstOrDefault();
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
                        Debug.Log("★[WeatherDataManager] センサーデータ読み込み完了！");
                    }
                }
            }, null);
        }, null);
    }
}
