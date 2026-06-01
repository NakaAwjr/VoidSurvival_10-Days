using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLording : Dialog
{
    public static SceneLording Instance { get; private set; }

    protected void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        gameObject.SetActive(false);
    }

    public override void OpenDialog()
    {
        base.OpenDialog();
        // TimeManagerのタイマーを停止
        TimeManager.Instance.PauseTimer();
    }
    public override void CloseDialog()
    {
        base.CloseDialog();
        // TimeManagerのタイマーを再開
        TimeManager.Instance.ResumeTimer();
    }
}
