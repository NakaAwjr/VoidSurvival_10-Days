using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(AnimatorController))]
public class CharacterRenderer : MonoBehaviour
{
    private Animator _animator;

    /// <summary>
    /// 待機状態の方向を定義する配列
    /// ここでは、北、南、東の3方向を定義
    /// </summary>
    private static readonly string[] idleDirections = { "Idle N", "Idle S", "Idle E" };
    /// <summary>
    /// 移動状態の方向を定義する配列
    /// ここでは、北、南、東の3方向を定義
    /// </summary>
    private static readonly string[] runDirections = { "Run N", "Run S", "Run E" };

    /// <summary>
    /// 最後に設定された方向のインデックス
    /// 0: 北, 1: 南, 2: 東
    /// </summary>
    private int lastDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 入力された方向に基づいてアニメーションを切り替える
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        // 入力の大きさが小さい場合は待機状態を使用
        if (direction.magnitude < 0.01f)
        {
            _animator.Play(idleDirections[lastDirection]);
            return;
        }

        // 入力の方向に基づいてアニメーションを設定
        lastDirection = DirectionToIndex(direction);
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1); // 水平方向の反転
        _animator.Play(runDirections[lastDirection]);
    }

    /// <summary>
    /// 入力方向に基づいてインデックスを計算するヘルパー関数
    /// </summary>
    private int DirectionToIndex(Vector2 dir)
    {
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            return dir.y > 0 ? 0 : 1; // 北または南
        }
        else
        {
            return 2; // 東
        }
    }
}
