using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterStatus), typeof(CharacterRenderer))]
public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attackCollider;
    [Header("攻撃間隔")]
    [SerializeField] private float attackInterval = 1.0f;
    [Header("攻撃範囲")]
    [SerializeField] private Vector2 northOffset = new Vector2(0, 0.5f);
    [SerializeField] private Vector2 northSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 southOffset = new Vector2(0, -0.5f);
    [SerializeField] private Vector2 southSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 westOffset = new Vector2(-0.5f, 0);
    [SerializeField] private Vector2 westSize = new Vector2(1f, 1f);

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
        _characterStatus.GoToActiveStateIfPossible();
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
                attackCollider.offset = northOffset;
                attackCollider.size = northSize;
                break;
            case 1: // 南
                attackCollider.offset = southOffset;
                attackCollider.size = southSize;
                break;
            case 2: // 西
                attackCollider.offset = westOffset;
                attackCollider.size = westSize;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.498f, 0.514f, 0.996f, 0.342f);
        Vector2 transform = this.transform.position;
        Gizmos.DrawCube(southOffset + transform, southSize);
        Gizmos.DrawCube(northOffset + transform, northSize);
        Gizmos.DrawCube(westOffset + transform, westSize);
    }
}
