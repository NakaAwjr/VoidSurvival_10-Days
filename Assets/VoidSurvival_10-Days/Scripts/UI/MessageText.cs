using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageText : Dialog
{
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text text;

    private GameObject _controllears;

    // シングルトンパターンのインスタンス
    public static MessageText Instance { get; private set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // //テスト用
        // List<string> list = new List<string>()
        // {
        //     "aaaaaaa",
        //     "bbbbbb",
        //     "ccccccc"
        // };
        // StartCoroutine(TextMessage(list));
    }

    /// <summary>
    /// メッセージを順番に表示するコルーチン
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public IEnumerator TextMessage(List<string> messages)
    {
        if (messages.Count != 0)
        {
            // メッセージパネルを表示
            OpenDialog();

            foreach (string s in messages)
            {
                text.text = s;
                // 少し待つ
                yield return new WaitForSeconds(0.5f);
                // タッチやクリックが行われるまで待機
                while (!Input.GetMouseButtonDown(0) && !Input.touchCount.Equals(1))
                {
                    yield return null; // フレーム待機
                }
            }

            // メッセージパネルを非表示にする
            CloseDialog();

            // テキストをクリア
            text.text = "";
        }
    }
}
