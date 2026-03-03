using TMPro;
using UnityEngine;

public class ShowDate : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private string dayFormat = "Day {0}";
    [SerializeField] private string timeFormat = "{0:D2}:{1:D2}";
    // Update is called once per frame
    void Update()
    {
        int day = TimeManager.Instance.GetDay();
        int hour = TimeManager.Instance.GetHour();
        int minute = TimeManager.Instance.GetMinute();
        dateText.text = string.Format(dayFormat, day);
        timeText.text = string.Format(timeFormat, hour, minute);
    }
}
