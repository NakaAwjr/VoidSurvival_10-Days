using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class MessageText : Dialog, IPointerClickHandler
{
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text text;

    private bool isClicked = false;

    // シングルトンパターンのインスタンス
    public static MessageText Instance { get; private set; }

    // Start is called before the first frame update
    protected void Awake()
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
        CloseDialog();
        // //テスト用
        // List<string> list = new List<string>()
        // {
        //     "aaaaaaa",
        //     "bbbbbb",
        //     "ccccccc"
        // };
        // StartCoroutine(TextMessage(list));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        isClicked = true;
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
                // クリックされるまで待つ
                yield return new WaitUntil(() => isClicked);
                isClicked = false;
            }

            // メッセージパネルを非表示にする
            CloseDialog();

            // テキストをクリア
            text.text = "";
        }
    }
}
