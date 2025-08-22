using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rigidbody;
    private CharacterRenderer _characterRenderer;
    private CharacterStatus _characterStatus;

    /// <summary>
    /// インタラクトアクションを実行するためのインターフェース
    /// </summary>
    private IInteractAction _interactAction;

    /// <summary>
    /// ジョイスティックからの入力を格納する変数
    /// </summary>
    private Vector2 _moveInput;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _characterRenderer = GetComponentInChildren<CharacterRenderer>();
        _characterStatus = GetComponent<CharacterStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        // ジョイスティックからの入力を取得
        _moveInput.x = CrossPlatformInputManager.GetAxis("Horizontal");
        _moveInput.y = CrossPlatformInputManager.GetAxis("Vertical");

        // 正規化し、速度を一定にする
        _moveInput.Normalize();

        if (_characterStatus.IsMoovable)
        {
            // キャラクターのアニメーションを更新
            _characterRenderer.SetDirection(_moveInput);

            // プレイヤーを動かす
            _rigidbody.velocity = _moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// プレイヤーがインタラクトボタンをクリックしたときの処理
    /// </summary>
    public void ActInteract()
    {
        if (_interactAction != null)
        {
            _interactAction.InteractAction();
        }
        else
        {
            //ItemManager.Instance.quickItems[ItemManager.Instance.selectedQuickItemIndex]?.Use();
            _characterRenderer.SetActionAnimation(CharacterRenderer.ActionType.Attack);
            _characterStatus.GoToActiveStatefPossible();
        }
    }

    // トリガーイベント
    #region Collider Events
    public void OnTriggerEnter2D(Collider2D other)
    {
        _interactAction = other.GetComponent<IInteractAction>();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        _interactAction = null;
    }
    #endregion
}
