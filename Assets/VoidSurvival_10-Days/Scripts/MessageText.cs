using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MessageText : MonoBehaviour
{
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text text;

    private GameObject _controllears;

    // シングルトンパターンのインスタンス
    public static MessageText Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // メッセージパネルを非表示にする
        messagePanel.SetActive(false);

        // コントローラーを探す
        _controllears = GameObject.Find("Controllers");

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
            messagePanel.SetActive(true);

            // コントローラーを無効化
            if (_controllears != null)
            {
                _controllears.SetActive(false);
            }

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
            messagePanel.SetActive(false);

            // テキストをクリア
            text.text = "";

            // コントローラーを再度有効化
            if (_controllears != null)
            {
                _controllears.SetActive(true);
            }
        }
    }
}
