using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField] private SaveDialog saveDialog;
    [SerializeField] private GameObject player;

    private void Awake()
    {
        // フレームレート設定
        Application.targetFrameRate = 61;
    }
    private void Start()
    {
        player.SetActive(false);
        MainUI.Instance.CloseMainUI();
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            saveDialog.OpenDialog();
        }
    }

    /// <summary>
    /// タイトル経由のデータロード
    /// </summary>
    public void FirstLoadGame()
    {
        saveDialog.Load();
        Debug.Log("Load Game");
        player.SetActive(true);
        MainUI.Instance.OpenMainUI();
    }
}
