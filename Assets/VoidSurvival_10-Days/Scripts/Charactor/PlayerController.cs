using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rigidbody;
    private PlayerRenderer _renderer;
    private PlayerStatus _status;

    /// <summary>
    /// インタラクトアクションを実行するためのインターフェース
    /// </summary>
    private InteractAction _interactAction;

    /// <summary>
    /// ジョイスティックからの入力を格納する変数
    /// </summary>
    private Vector2 _moveInput;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<PlayerRenderer>();
        _status = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        // ジョイスティックからの入力を取得
        _moveInput.x = CrossPlatformInputManager.GetAxis("Horizontal");
        _moveInput.y = CrossPlatformInputManager.GetAxis("Vertical");

        // 正規化し、速度を一定にする
        _moveInput.Normalize();

        if (_status.IsMoovable)
        {
            // キャラクターのアニメーションを更新
            _renderer.SetDirection(_moveInput);

            // プレイヤーを動かす
            _rigidbody.velocity = _moveInput * moveSpeed;
        }
    }

    /// <summary>
    /// プレイヤーがインタラクトボタンをクリックしたときの処理
    /// インタラクトアクション(話しかけるなど)が設定されていればそれを実行し、なければインタラクトアイテムを使用する
    /// </summary>
    public void ActInteract()
    {
        if (_interactAction != null)
        {
            _interactAction.Action();
        }
        else
        {
            ItemManager.Instance.quickItems[ItemManager.Instance.selectedQuickItemIndex]?.Item.Use(gameObject);
        }
    }

    // トリガーイベント
    #region Collider Events
    public void OnTriggerEnter2D(Collider2D other)
    {
        _interactAction = other.GetComponent<InteractAction>();
        _interactAction?.ShowInteractTarget();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        _interactAction?.HideInteractTarget();
        _interactAction = null;
    }
    #endregion
}
