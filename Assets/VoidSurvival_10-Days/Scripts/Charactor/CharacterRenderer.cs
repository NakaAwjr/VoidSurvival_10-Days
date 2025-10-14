using UnityEngine;

/// <summary>
/// キャラクターのアニメーションを制御するクラス
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterStatus))]
public abstract class CharacterRenderer : MonoBehaviour
{
    protected CharacterStatus _characterStatus;
    protected Animator _animator;

    /// アニメーションの名前
    private static readonly string[] idleDirections = { "Idle N", "Idle S", "Idle W" };
    private static readonly string[] runDirections = { "Run N", "Run S", "Run W" };
    private static readonly string[] attackDirections = { "Attack N", "Attack S", "Attack W" };
    private static readonly string dieDirection = "Die";

    /// <summary>
    /// 最後に設定された方向のインデックス
    /// 0: 北, 1: 南, 2: 西
    /// </summary>
    public int lastDirection { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        _characterStatus = GetComponent<CharacterStatus>();
        _animator = GetComponent<Animator>();
        _characterStatus.onDie.AddListener(SetDieAnimation);
    }

    /// <summary>
    /// 入力された方向に基づいてアニメーションを切り替える
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (_characterStatus.IsMoovable)
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
            transform.localScale = new Vector3(-Mathf.Sign(direction.x), 1, 1);
            // アニメーションを再生
            _animator.Play(runDirections[lastDirection]);
        }
    }
    /// <summary>
    /// 攻撃アニメーションを設定する
    /// </summary>
    public void SetAttackAnimation()
    {
        if (_characterStatus.IsActive)
        {
            _animator.Play(attackDirections[lastDirection]);
        }
    }

    /// <summary>
    /// 死亡アニメーションを設定する
    /// </summary>
    public void SetDieAnimation()
    {
        _animator.Play(dieDirection);
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
