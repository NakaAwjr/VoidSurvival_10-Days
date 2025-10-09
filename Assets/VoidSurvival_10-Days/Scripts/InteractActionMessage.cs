using System.Collections.Generic;
using UnityEngine;

public class InteractActionMessage : InteractAction
{
    // メッセージを表示するためのサンプルメッセージリスト
    [SerializeField]
    private List<string> messages = new List<string>
        {
            "You have interacted with the object.",
            "This is a sample interaction message."
        };
    /// <summary>
    /// プレイヤーがインタラクトボタンをクリックしたときの処理
    /// </summary>
    public override void Action()
    {
        // メッセージを表示するコルーチンを開始
        StartCoroutine(MessageText.Instance.TextMessage(messages));
    }
}