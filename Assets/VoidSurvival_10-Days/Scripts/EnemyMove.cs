using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private EnemyRenderer _renderer;
    private NavMeshAgent _navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _renderer = GetComponentInChildren<EnemyRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _navMeshAgent.SetDestination(playerController.transform.position);
        _renderer.SetDirection(_navMeshAgent.velocity.normalized);
    }
}
