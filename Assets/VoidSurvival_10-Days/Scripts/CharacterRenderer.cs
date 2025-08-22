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

    [SerializeField] protected CharacterStatus characterStatus;
    [SerializeField] private Transform characterTransform;
    private Animator _animator;

    /// ここでは、北、南、西の3方向を定義
    private static readonly string[] idleDirections = { "Idle N", "Idle S", "Idle W" };
    private static readonly string[] runDirections = { "Run N", "Run S", "Run W" };
    private static readonly string[] attackDirections = { "Attack N", "Attack S", "Attack W" };
    private static readonly string[] tillDirections = { "Till N", "Till S", "Till W" };

    /// <summary>
    /// 最後に設定された方向のインデックス
    /// 0: 北, 1: 南, 2: 西
    /// </summary>
    protected int lastDirection = 0;

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
            // 左右反転
            characterTransform.localScale = new Vector3(-Mathf.Sign(direction.x), 1, 1);
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
            return 2;
        }
    }
}
