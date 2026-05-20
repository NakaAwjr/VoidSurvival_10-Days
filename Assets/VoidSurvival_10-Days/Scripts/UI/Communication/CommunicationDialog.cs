using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CommunicationDialog : Dialog
{
    [SerializeField] private RadioHandle freqHandle;
    [SerializeField] private RadioHandle dirHandle;
    // [SerializeField] private RadioHandle lenHandle;
    [SerializeField] private RadioWaveUI radioWaveUI;
    private Canvas canvas;

    private bool isSuccessful = false;

    // 正解の値
    private int targetFreq, targetDir, targetLen;
    // 現在の値
    // public float currentFreq, currentDir, currentLen;

    // 通信成功の許容範囲
    private float threshold = 3.0f;

    // 通信回数管理
    private int dailyLimit = 5;
    private int successCount = 0;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        GenerateNewTarget();
        gameObject.SetActive(false);
    }
    public override void OpenDialog()
    {
        base.OpenDialog();
        MainUI.Instance.AddBackStack();
        GenerateNewTarget();
    }
    public override void CloseDialog()
    {
        base.CloseDialog();
        MainUI.Instance.RemoveBackStack();
    }
    private void Update()
    {
        float signalStrength = GetSignalStrength();
        radioWaveUI.signalStrength = signalStrength;

        if (isSuccessful) return;
        // 通信成功判定
        if (IsWithinThreshold(freqHandle.currentValue, targetFreq, threshold) &&
            IsWithinThreshold(dirHandle.currentValue, targetDir, threshold))
        {
            StartCoroutine(OnSuccessfulCommunication());
        }
    }

    /// <summary>
    /// 新しい目標値を生成する
    /// </summary>
    private void GenerateNewTarget()
    {
        int _targetFreq = Random.Range(1, 101);
        int _targetDir = Random.Range(1, 101);
        int _targetLen = Random.Range(1, 101);
        if (IsWithinThreshold(_targetFreq, freqHandle.currentValue, threshold) ||
            IsWithinThreshold(_targetDir, dirHandle.currentValue, threshold))
        {
            // もし現在の値に近い場合は再生成
            GenerateNewTarget();
            return;
        }
        targetFreq = _targetFreq;
        targetDir = _targetDir;
        targetLen = _targetLen;
    }
    /// <summary>
    /// 値が目標値の閾値内にあるかどうか
    /// </summary>
    /// <param name="value"></param>
    /// <param name="target"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private bool IsWithinThreshold(float value, float target, float threshold)
    {
        return Mathf.Min(Mathf.Abs(value - target), 100 - Mathf.Abs(value - target)) <= threshold;
    }
    /// <summary>
    /// 現在の信号強度を取得する
    /// </summary>
    /// <returns></returns>
    private float GetSignalStrength()
    {
        // float diff = Mathf.Abs(targetFreq - currentFreq)
        //            + Mathf.Abs(targetDir - currentDir)
        //            + Mathf.Abs(targetLen - currentLen);

        // // 最大300（100*3）から0までの値を、1.0（最強）〜0.0（最弱）に変換
        // return Mathf.Clamp01(1.0f - (diff / 150.0f));
        float diff = Mathf.Min(Mathf.Abs(targetFreq - freqHandle.currentValue), 100 - Mathf.Abs(targetFreq - freqHandle.currentValue));
        diff += Mathf.Min(Mathf.Abs(targetDir - dirHandle.currentValue), 100 - Mathf.Abs(targetDir - dirHandle.currentValue));
        // diff += Mathf.Min(Mathf.Abs(targetLen - lenHandle.currentValue), 100 - Mathf.Abs(targetLen - lenHandle.currentValue));
        return Mathf.Clamp01(1.0f - (diff / 150.0f));
    }
    /// <summary>
    /// 通信成功時の処理
    /// </summary>
    private IEnumerator OnSuccessfulCommunication()
    {
        isSuccessful = true;
        successCount++;
        freqHandle.isDragable = false;
        dirHandle.isDragable = false;
        if (successCount >= dailyLimit)
        {
            // 1日の通信回数上限に達した場合の処理
            Debug.Log("Daily communication limit reached.");
            // CloseDialog();
        }
        else
        {
            Debug.Log("Successful communication! Generating new target...");
            yield return MessageText.Instance.TextMessage(new List<string> {
                "Successful communication established!",
                $"Remaining communication attempts: {dailyLimit - successCount}"
            });
            GenerateNewTarget();
            freqHandle.isDragable = true;
            dirHandle.isDragable = true;
            isSuccessful = false;
        }
    }
}
