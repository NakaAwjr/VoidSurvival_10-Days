using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStatus))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    /// <summary>
    /// 衝突判定したいレイヤー(プレイヤーと敵以外の障害物)
    /// </summary>
    [SerializeField] LayerMask raycastLayerMask;
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float detectionRadius = 5.0f;
    [SerializeField] private float detectionInterval = 1.0f;

    private EnemyStatus _status;
    private EnemyRenderer _renderer;
    private NavMeshAgent _navMeshAgent;
    private RaycastHit2D[] _hits = new RaycastHit2D[10];
    private Vector3 _position;

    // Start is called before the first frame update
    void Start()
    {
        _status = GetComponent<EnemyStatus>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _renderer = GetComponent<EnemyRenderer>();
        _navMeshAgent.speed = moveSpeed;
        _position = transform.position;
        //StartCoroutine(DetectionCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.SetDirection(_navMeshAgent.velocity.normalized);
    }

    /// <summary>
    /// プレイヤーを検知したときの処理(トリガーイベント)
    /// </summary>
    public void OnDetectObject(Collider2D other)
    {
        if (!_status.IsMovable)
        {
            _navMeshAgent.isStopped = true;
            return;
        }
        var _player = other.GetComponent<PlayerStatus>();
        if (_player != null)
        {
            Vector2 _positionDiff = _player.transform.position - transform.position;
            var _distance = _positionDiff.magnitude;
            Vector2 _direction = _positionDiff.normalized;
            var _hitCount = Physics2D.RaycastNonAlloc(transform.position, _direction, _hits, _distance, raycastLayerMask);
            if (_hitCount == 0)
            {
                // プレイヤーに向かって移動
                //StopCoroutine(DetectionCoroutine());
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_player.transform.position);
            }
            else
            {
                // プレイヤーが障害物の向こう側にいる場合は停止
                //StartCoroutine(DetectionCoroutine());
                _navMeshAgent.isStopped = true;
                Debug.Log(_hits[0].transform.name);
            }
        }
    }

    //ランダムに移動する
    private void GoToNextPoint()
    {
        if (!_status.IsMovable) return;
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * detectionRadius;
            randomDirection += _position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, detectionRadius, 1))
            {
                _navMeshAgent.SetDestination(hit.position);
            }
        }
    }
    private IEnumerator DetectionCoroutine()
    {
        while (true)
        {
            GoToNextPoint();
            yield return new WaitForSeconds(detectionInterval);
        }
    }
}
