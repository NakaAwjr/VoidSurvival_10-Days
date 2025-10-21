using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    [Header("一日が何分か")]
    [SerializeField] private float minutesPerDay = 30f;
    [Header("何日までの制限か")]
    [SerializeField] private int maxDays = 10;

    public static TimeManager Instance => _instance;
    private static TimeManager _instance;

    public float ElapsedTime => _elapsedTime;
    private float _elapsedTime = 0f;
    private IEnumerator _timerCoroutine;

    /// <summary>
    /// 日が変わったときに発火するイベント
    /// </summary>
    public UnityEvent OnDayChanged = new UnityEvent();
    private int _lastDay = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _timerCoroutine = StartTimer();
    }

    void Update()
    {
        int currentDay = GetDay();
        if (currentDay != _lastDay)
        {
            _lastDay = currentDay;
            OnDayChanged.Invoke();
        }
    }

    /// <summary>
    /// 現在の時間の「時」を取得する
    /// </summary>
    /// <returns></returns>
    public int GetHour()
    {
        float totalMinutes = _elapsedTime / 60f * 1440f / minutesPerDay;
        int hour = (int)(totalMinutes % 1440) / 60;
        return hour;
    }
    /// <summary>
    /// 現在の時間の「分」を取得する
    /// </summary>
    public int GetMinute()
    {
        float totalMinutes = _elapsedTime / 60f * 1440f / minutesPerDay;
        int minute = (int)(totalMinutes % 1440) % 60;
        return minute;
    }
    /// <summary>
    /// 現在の「日」を取得する
    /// </summary>
    /// <returns></returns>
    public int GetDay()
    {
        float totalMinutes = _elapsedTime / 60f * 1440f / minutesPerDay;
        int day = (int)(totalMinutes / 1440) + 1;
        return day;
    }
    /// <summary>
    /// 残り日数を取得する
    /// </summary>
    public int GetRestDays()
    {
        return maxDays - GetDay();
    }

    /// <summary>
    /// タイマー開始，_isPausedがtrueになるまで時間を進める
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartTimer()
    {
        while (true)
        {
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// タイマーを一時停止する
    /// </summary>
    public void PauseTimer()
    {
        StopCoroutine(_timerCoroutine);
    }
    /// <summary>
    /// タイマーを再開する
    /// </summary>
    public void ResumeTimer()
    {
        StopCoroutine(_timerCoroutine);
        StartCoroutine(_timerCoroutine);
    }

    public void FromSaveData(float time)
    {
        _elapsedTime = time;
        _lastDay = GetDay();
    }
}
