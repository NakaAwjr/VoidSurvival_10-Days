using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus))]
public class CharactorAttack : MonoBehaviour
{
    /// <summary>
    /// 攻撃間隔
    /// </summary>
    [SerializeField] private float attackInterval = 1.0f;
    [SerializeField] private Collider2D attackCollider;
    private CharacterStatus _characterStatus;
    private CharacterRenderer _characterRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _characterStatus = GetComponent<CharacterStatus>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        attackCollider.enabled = false;
    }

    /// <summary>
    /// 攻撃可能なら攻撃状態に遷移し、攻撃アニメーションを再生する
    /// </summary>
    public void AttackIfPossible()
    {
        if (!_characterStatus.IsActive) return;
        _characterRenderer.SetAttackAnimation();
        _characterStatus.GoToActiveStatefPossible();
    }
    /// <summary>
    /// 攻撃範囲に入ったときに攻撃可能なら攻撃する(敵専用)
    /// </summary>
    /// <param name="other"></param>
    public void OnAttackRangeEnter(Collider2D other)
    {
        AttackIfPossible();
    }
    /// <summary>
    /// 攻撃が始まったときに呼ばれる(アニメーションイベント)
    /// </summary>
    public void OnAttackStart()
    {
        attackCollider.enabled = true;
        Debug.Log("Attack Start");
        // 攻撃方向に応じて攻撃コライダーの位置を調整
        switch (_characterRenderer.lastDirection)
        {
            case 0: // 北
                attackCollider.offset = new Vector2(0, 0.5f);
                break;
            case 1: // 南
                attackCollider.offset = new Vector2(0, -0.5f);
                break;
            case 2: // 西
                attackCollider.offset = new Vector2(-0.5f, 0);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 攻撃が終わったときに呼ばれる(アニメーションイベント)
    /// </summary>
    public void OnAttackFinished()
    {
        attackCollider.enabled = false;
        Debug.Log("Attack Finished");
        StartCoroutine(CooldownCoroutine());
    }
    /// <summary>
    /// attackColliderが何かに当たったときに呼ばれる(トリガーイベント)
    /// </summary>
    /// <param name="other"></param>
    public void OnAttackHit(Collider2D other)
    {
        var targetStatus = other.GetComponent<CharacterStatus>();
        if (targetStatus != null)
        {
            targetStatus.Damage(_characterStatus.Power);
        }
    }
    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(attackInterval);
        _characterStatus.GoToIdleStateIfPossible();
    }
}
