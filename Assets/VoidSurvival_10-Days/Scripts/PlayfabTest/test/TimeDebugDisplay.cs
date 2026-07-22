using UnityEngine;
using TMPro;

// ============================================================
//  TimeDebugDisplay.cs   ★時間が進んでいるか確認する用
//  Day と HH:MM を TMPテキストに表示する。
//  起動時にタイマーを自動スタートする機能つき（テスト用）。
//
//  使い方：
//    1. 空GameObjectに付ける（または既存のオブジェクトに付ける）
//    2. Inspectorの Time Text に、表示用の TextMeshPro テキストを割り当てる
//    3. Play → 数字が動けば時間が進んでいる証拠
// ============================================================

public class TimeDebugDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    [Header("起動時にタイマーを自動スタートする（テスト用）")]
    [SerializeField] private bool autoStartTimer = true;

    private void Start()
    {
        // ★これが無いと TimeManager の時間は 0 のまま進まない
        if (autoStartTimer && TimeManager.Instance != null)
            TimeManager.Instance.ResumeTimer();
    }

    private void Update()
    {
        if (TimeManager.Instance == null || timeText == null) return;

        int day = TimeManager.Instance.GetDay();
        int h   = TimeManager.Instance.GetHour();
        int m   = TimeManager.Instance.GetMinute();

        // 例：Day 1   06:23
        timeText.text = $"Day {day}   {h:00}:{m:00}";
    }
}
