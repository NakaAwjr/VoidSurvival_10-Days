using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// キャラクターのアニメーションを制御するクラス
/// </summary>
[RequireComponent(typeof(AnimatorController))]
public abstract class CharacterRenderer : MonoBehaviour
{
    public enum ActionType
    {
        Attack,
        Till
    }

    [SerializeField] private CharacterStatus characterStatus;
    private Animator _animator;

    /// <summary>
    /// 待機状態の方向を定義する配列
    /// ここでは、北、南、東、西の4方向を定義
    /// </summary>
    private static readonly string[] idleDirections = { "Idle N", "Idle S", "Idle E", "Idle W" };
    /// <summary>
    /// 移動状態の方向を定義する配列
    /// ここでは、北、南、東、西の4方向を定義
    /// </summary>
    private static readonly string[] runDirections = { "Run N", "Run S", "Run E", "Run W" };
    private static readonly string[] attackDirections = { "Attack N", "Attack S", "Attack E", "Attack W" };
    private static readonly string[] tillDirections = { "Till N", "Till S", "Till E", "Till W" };

    /// <summary>
    /// 最後に設定された方向のインデックス
    /// 0: 北, 1: 南, 2: 東, 3: 西
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
        if (characterStatus.IsMoovable)
        {
            // 入力の大きさが小さい場合は待機状態を使用
            if (direction.magnitude < 0.01f)
            {
                _animator.Play(idleDirections[lastDirection]);
                return;
            }

            // 入力の方向に基づいてアニメーションを設定
            lastDirection = DirectionToIndex(direction);
            // アニメーションを再生
            _animator.Play(runDirections[lastDirection]);
        }
    }
    /// <summary>
    /// アクションアニメーションを設定する
    /// アクションタイプに応じて異なるアニメーションを再生
    /// デフォルトは攻撃アニメーション
    /// </summary>
    public virtual void SetActionAnimation(ActionType actionType = ActionType.Attack)
    {
        if (characterStatus.IsActive)
        {
            _animator.Play(attackDirections[lastDirection]);
        }
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
            return dir.x > 0 ? 2 : 3; // 東または西
        }
    }
}
