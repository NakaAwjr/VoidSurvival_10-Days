using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json.Linq;

/// <summary>
/// PlayFab の "HKdata"（温度・電圧・電流の20点）を読み込み、
/// 各カテゴリで変化が大きい列を抜き出して表示する。
/// ・DataManagerと同じ「1日の中での経過分」（GetHour()*60+GetMinute()、日が変わるとリセット）を
/// 　minutesPerSplit（ゲーム内分、Inspectorで調整可）で割って現在の行を求める。
/// ・データ全体を見て HIGH / NORMAL / LOW を判定してテキストに表示。
/// </summary>
public class HKDataDisplay : MonoBehaviour
{
    public static HKDataDisplay Instance { get; private set; }

    [Header("表示先")]
    [SerializeField] private TMP_Text dataValueText;

    [Tooltip("1つのSplit（行）をゲーム内何分間表示するか（例:2なら00:02→2/20, 00:04→3/20）")]
    [SerializeField] private float minutesPerSplit = 5f;

    [Tooltip("データ進行の速度係数。1=通常速度、0.5=2倍遅い、2=2倍速い")]
    [SerializeField] private float speedFactor = 1f;

    [Tooltip("ONで最後の行まで行ったら先頭に戻ってループする。OFFなら最後の行で止まる")]
    [SerializeField] private bool loop = false;

    [Header("各カテゴリで抜き出す本数（変化が大きい順）")]
    [SerializeField] private int tempCount = 2;
    [SerializeField] private int voltCount = 1;
    [SerializeField] private int currCount = 1;

    [Header("状態表示")]
    [SerializeField] private bool showState = true;          // HIGH/NORMAL/LOW を出すか

    // 現在の各カテゴリの状態（LOW/NORMAL/HIGH、複数列ある場合は最も厳しい状態）。
    // WeatherTester等の外部スクリプトから参照する。
    public string TempState { get; private set; } = "NORMAL";
    public string VoltageState { get; private set; } = "NORMAL";
    public string CurrentState { get; private set; } = "NORMAL";

    private JArray _rows;
    private List<string> _tempCols = new List<string>();
    private List<string> _voltCols = new List<string>();
    private List<string> _currCols = new List<string>();
    private Dictionary<string, double[]> _range = new Dictionary<string, double[]>(); // col -> [min,max]

    private bool _isLoaded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "148482";
        LoadHKData();
    }

    void Update()
    {
        if (!_isLoaded || _rows == null || _rows.Count == 0) return;
        if (TimeManager.Instance == null) return;

        ShowRow(GetIndexFromGameTime());
    }

    // DataManagerと同じ「1日の中での経過分」（日が変わるとリセットされる）を基準に、
    // minutesPerSplit分ごとに1行ずつ切り替える。
    private int GetIndexFromGameTime()
    {
        var tm = TimeManager.Instance;
        int gameElapsedMinutes = Mathf.FloorToInt((tm.GetHour() * 60 + tm.GetMinute()) * speedFactor);
        int rawIndex = Mathf.FloorToInt(gameElapsedMinutes / minutesPerSplit);

        if (loop)
            return ((rawIndex % _rows.Count) + _rows.Count) % _rows.Count;
        return Mathf.Clamp(rawIndex, 0, _rows.Count - 1);
    }

    private const int MaxLoginRetries = 3;
    private const float LoginRetryDelay = 1f;

    private void LoadHKData(int retryCount = 0)
    {
        var login = new LoginWithCustomIDRequest { CustomId = "VoidSurvival_User", CreateAccount = false };
        PlayFabClientAPI.LoginWithCustomID(login, _ =>
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), result =>
            {
                if (result.Data != null && result.Data.ContainsKey("HKdata"))
                    ParseHKData(result.Data["HKdata"]);
                else
                    Debug.LogError("[HKDataDisplay] TitleData に 'HKdata' キーがありません。");
            }, err => Debug.LogError("[HKDataDisplay] GetTitleData 失敗: " + err.GenerateErrorReport()));
        }, err =>
        {
            // DataManager等が同時にLoginWithCustomIDを叩くと同じIDでの同時ログインが409 Conflictになることがあるため、
            // 少し待ってリトライする（他のログイン処理と時間をずらせば通常成功する）。
            if (retryCount < MaxLoginRetries)
            {
                Debug.LogWarning($"[HKDataDisplay] Login 失敗（{retryCount + 1}/{MaxLoginRetries}回目）、{LoginRetryDelay}秒後に再試行します: " + err.GenerateErrorReport());
                StartCoroutine(RetryLoadHKData(retryCount + 1));
            }
            else
            {
                Debug.LogError("[HKDataDisplay] Login 失敗（リトライ上限到達）: " + err.GenerateErrorReport());
            }
        });
    }

    private IEnumerator RetryLoadHKData(int retryCount)
    {
        yield return new WaitForSeconds(LoginRetryDelay);
        LoadHKData(retryCount);
    }

    private void ParseHKData(string json)
    {
        JToken root = JToken.Parse(json);
        _rows = (root.Type == JTokenType.Array) ? (JArray)root : root["rows"] as JArray;

        if (_rows == null || _rows.Count == 0)
        {
            Debug.LogError("[HKDataDisplay] 行データが空です。HKdata の形式を確認してください。");
            return;
        }

        var cols = ((JObject)_rows[0]).Properties().Select(p => p.Name).ToList();
        _tempCols = PickMostVariable(cols.Where(c => c.Contains("°C") || c.Contains("℃")).ToList(), tempCount);
        _voltCols = PickMostVariable(cols.Where(c => c.Contains("mV") || c.Contains("(V)")).ToList(), voltCount);
        _currCols = PickMostVariable(cols.Where(c => c.Contains("mA")).ToList(), currCount);

        // 表示する列の min/max を「データ全体（20点）」から求めておく → HIGH/NORMAL/LOW 判定に使う
        _range.Clear();
        foreach (var c in _tempCols.Concat(_voltCols).Concat(_currCols))
        {
            double[] mm = MinMax(c);
            if (mm != null) _range[c] = mm;
        }

        _isLoaded = true;
        Debug.Log($"[HKDataDisplay] 読込完了 行数={_rows.Count} / 温度[{string.Join(", ", _tempCols)}]" +
                  $" 電圧[{string.Join(", ", _voltCols)}] 電流[{string.Join(", ", _currCols)}]");
    }

    private double[] MinMax(string col)
    {
        double min = double.MaxValue, max = double.MinValue; bool any = false;
        foreach (var row in _rows)
        {
            var tok = row[col];
            if (tok == null || tok.Type == JTokenType.Null) continue;
            if (double.TryParse(tok.ToString(), out double v)) { if (v < min) min = v; if (v > max) max = v; any = true; }
        }
        return any ? new double[] { min, max } : null;
    }

    private List<string> PickMostVariable(List<string> candidates, int n)
    {
        return candidates
            .Select(c => { var mm = MinMax(c); return new { c, range = (mm == null) ? 0 : mm[1] - mm[0] }; })
            .OrderByDescending(x => x.range)
            .Take(n).Select(x => x.c).ToList();
    }

    // データ全体の最小〜最大を3分割して LOW / NORMAL / HIGH に判定
    private string Classify(string col, double v)
    {
        if (!_range.TryGetValue(col, out var mm)) return "NORMAL";
        double min = mm[0], max = mm[1], span = max - min;
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

    private void ShowRow(int i)
    {
        var row = (JObject)_rows[i];

        TempState = _tempCols.Count > 0 ? WorstState(_tempCols, row) : "NORMAL";
        VoltageState = _voltCols.Count > 0 ? WorstState(_voltCols, row) : "NORMAL";
        CurrentState = _currCols.Count > 0 ? WorstState(_currCols, row) : "NORMAL";

        if (dataValueText == null) return;

        string s = "<color=#FFFF00>--- HK Data ---</color>\n";

        string date = row["Date"]?.ToString();
        string split = row["Split No"]?.ToString();
        if (!string.IsNullOrEmpty(date)) s += $"{date}  ({split}/{_rows.Count})\n";

        if (_tempCols.Count > 0) { s += "<b>[Temp]</b>\n"; foreach (var c in _tempCols) s += Line(row, c); }
        if (_voltCols.Count > 0) { s += "<b>[Voltage]</b>\n"; foreach (var c in _voltCols) s += Line(row, c); }
        if (_currCols.Count > 0) { s += "<b>[Current]</b>\n"; foreach (var c in _currCols) s += Line(row, c); }

        dataValueText.text = s;
    }

    // 複数列のうち最も厳しい状態（HIGH > NORMAL > LOW）を返す
    private string WorstState(List<string> cols, JObject row)
    {
        string worst = "LOW";
        foreach (var c in cols)
        {
            var tok = row[c];
            if (tok == null || tok.Type == JTokenType.Null || !double.TryParse(tok.ToString(), out double v)) continue;
            string st = Classify(c, v);
            if (Severity(st) > Severity(worst)) worst = st;
        }
        return worst;
    }

    private static int Severity(string state) => state == "HIGH" ? 2 : state == "LOW" ? 0 : 1;

    private string Line(JObject row, string col)
    {
        var tok = row[col];
        string val = Fmt(tok);
        if (showState && tok != null && tok.Type != JTokenType.Null && double.TryParse(tok.ToString(), out double v))
        {
            string st = Classify(col, v);
            return $"{ShortName(col)}: {val} <color={StateColor(st)}>[{st}]</color>\n";
        }
        return $"{ShortName(col)}: {val}\n";
    }

    private string Fmt(JToken tok)
    {
        if (tok == null || tok.Type == JTokenType.Null) return "-";
        if (double.TryParse(tok.ToString(), out double v)) return v.ToString("F2");
        return tok.ToString();
    }

    private string ShortName(string col)
    {
        int p = col.IndexOf('(');
        return (p > 0) ? col.Substring(0, p).Trim() : col.Trim();
    }
}
