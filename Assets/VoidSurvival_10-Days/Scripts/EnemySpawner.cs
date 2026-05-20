using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int spownCount;
    [SerializeField] private List<SpawnEnemy> Enemies;

    private Bounds bounds;
    // Start is called before the first frame update
    void Start()
    {
        bounds = GetComponent<Collider2D>().bounds;
        for (int i = 0; i < spownCount; i++)
        {
            var maxRange = Enemies.Sum(x => x.spawnRange);
            EnemySpawn(SelectEnemy(Random.Range(1, maxRange + 1)));
        }
    }

    /// <summary>
    /// どのエネミーが出るか決める
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private GameObject SelectEnemy(int x)
    {
        var i = 0;
        foreach (var enemy in Enemies)
        {
            i += enemy.spawnRange;
            if (x <= i) return enemy.enemyPrefab;
        }
        return null;
    }
    /// <summary>
    /// 敵をNavMesh上のランダムな位置にスポーン
    /// </summary>
    /// <param name="enemy"></param>
    private void EnemySpawn(GameObject enemy)
    {
        NavMeshHit navMeshHit;
        var ramdomPsition = new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), 0);
        if (NavMesh.SamplePosition(ramdomPsition, out navMeshHit, 10, NavMesh.AllAreas))
        {
            Instantiate(enemy, navMeshHit.position, Quaternion.identity);
        }
    }

    [System.Serializable]
    class SpawnEnemy
    {
        public GameObject enemyPrefab;
        public int spawnRange;
    }
}
