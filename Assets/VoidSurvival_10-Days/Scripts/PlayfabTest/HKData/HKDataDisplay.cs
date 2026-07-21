using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json.Linq;

/// <summary>
/// PlayFab の "HKdata"（温度・電圧・電流の20点）を読み込み、
/// 各カテゴリで変化が大きい列を抜き出して 5秒ごとに1点ずつ表示する。
/// ・時間ずれ対策：実時間の絶対グリッドで発火（timeScale やフレーム落ちの影響を受けない）
/// ・データ全体を見て HIGH / NORMAL / LOW を判定してテキストに表示。
/// </summary>
public class HKDataDisplay : MonoBehaviour
{
    [Header("表示先")]
    [SerializeField] private TMP_Text dataValueText;

    [Header("動作設定")]
    [SerializeField] private float updateInterval = 5f;      // 何秒ごとに切り替えるか
    [SerializeField] private bool useUnscaledTime = true;    // true=実時間(timeScaleの影響を受けない)
    [SerializeField] private bool loop = true;               // 最後まで行ったら先頭へ戻る

    [Header("各カテゴリで抜き出す本数（変化が大きい順）")]
    [SerializeField] private int tempCount = 2;
    [SerializeField] private int voltCount = 1;
    [SerializeField] private int currCount = 1;

    [Header("状態表示")]
    [SerializeField] private bool showState = true;          // HIGH/NORMAL/LOW を出すか

    private JArray _rows;
    private List<string> _tempCols = new List<string>();
    private List<string> _voltCols = new List<string>();
    private List<string> _currCols = new List<string>();
    private Dictionary<string, double[]> _range = new Dictionary<string, double[]>(); // col -> [min,max]

    private bool _isLoaded = false;
    private float _nextTime = 0f;
    private int _index = 0;

    private float Now => useUnscaledTime ? Time.unscaledTime : Time.time;

    void Start()
    {
        PlayFabSettings.staticSettings.TitleId = "148482";
        LoadHKData();
    }

    void Update()
    {
        if (!_isLoaded || _rows == null || _rows.Count == 0) return;
        if (Now < _nextTime) return;

        _nextTime += updateInterval;                 // グリッドに固定 → ドリフトしない
        if (_nextTime < Now) _nextTime = Now + updateInterval; // 大きく遅れたら引き直し(多重発火防止)

        ShowRow(_index);
        _index++;
        if (_index >= _rows.Count) _index = loop ? 0 : _rows.Count - 1;
    }

    private void LoadHKData()
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
        }, err => Debug.LogError("[HKDataDisplay] Login 失敗: " + err.GenerateErrorReport()));
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
        _nextTime = Now; // 読み込み直後に即1回表示
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
        if (dataValueText == null) return;
        var row = (JObject)_rows[i];

        string s = "<color=#FFFF00>--- HK Data ---</color>\n";

        string date = row["Date"]?.ToString();
        string split = row["Split No"]?.ToString();
        if (!string.IsNullOrEmpty(date)) s += $"{date}  ({split}/{_rows.Count})\n";

        if (_tempCols.Count > 0) { s += "<b>[Temp]</b>\n"; foreach (var c in _tempCols) s += Line(row, c); }
        if (_voltCols.Count > 0) { s += "<b>[Voltage]</b>\n"; foreach (var c in _voltCols) s += Line(row, c); }
        if (_currCols.Count > 0) { s += "<b>[Current]</b>\n"; foreach (var c in _currCols) s += Line(row, c); }

        dataValueText.text = s;
    }

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
