using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("敵情報(デフォルト)")]
    [SerializeField] private int spawnCount;
    [SerializeField] private List<SpawnEnemy> spawnEnemies;
    [Header("敵情報(天気ごと)")]
    [SerializeField] private List<WeatherSpawnCount> weatherSpawnCounts;
    [SerializeField] private List<WeatherSpawnEnemy> weatherSpawnEnemies;

    [Header("沸き範囲")]
    [SerializeField] private Vector2 minXY;
    [SerializeField] private Vector2 maxXY;

    // Start is called before the first frame update
    void Start()
    {
        int count = spawnCount;
        List<SpawnEnemy> enemies = spawnEnemies;
        foreach (var value in weatherSpawnCounts)
        {
            if (EqualityComparer<Weather>.Default.Equals(WeatherTester.Instance.CurrentRuleBasedWeather, value.weather))
            {
                count = value.spawnCount;
            }
        }
        foreach (var value in weatherSpawnEnemies)
        {
            if (EqualityComparer<Weather>.Default.Equals(WeatherTester.Instance.CurrentRuleBasedWeather, value.weather))
            {
                enemies = value.Enemies;
            }
        }
        for (int i = 0; i < count; i++)
        {
            var maxRange = enemies.Sum(x => x.spawnRange);
            EnemySpawn(SelectEnemy(Random.Range(1, maxRange + 1), enemies));
        }
    }

    /// <summary>
    /// どのエネミーが出るか決める
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private GameObject SelectEnemy(int x, List<SpawnEnemy> enemies)
    {
        var i = 0;
        foreach (var enemy in enemies)
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
        var randomPosition = new Vector3(Random.Range(minXY.x, maxXY.x), Random.Range(minXY.y, maxXY.y), 0);
        if (NavMesh.SamplePosition(randomPosition, out navMeshHit, 10, NavMesh.AllAreas))
        {
            Instantiate(enemy, navMeshHit.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.6f);
        Vector2 transform = this.transform.position;
        Gizmos.DrawLine(minXY, new Vector2(minXY.x, maxXY.y));
        Gizmos.DrawLine(minXY, new Vector2(maxXY.x, minXY.y));
        Gizmos.DrawLine(maxXY, new Vector2(minXY.x, maxXY.y));
        Gizmos.DrawLine(maxXY, new Vector2(maxXY.x, minXY.y));
    }

    [System.Serializable]
    class SpawnEnemy
    {
        public GameObject enemyPrefab;
        public int spawnRange;
    }
    [System.Serializable]
    class WeatherSpawnCount
    {
        public Weather weather;
        public int spawnCount;
    }
    [System.Serializable]
    class WeatherSpawnEnemy
    {
        public Weather weather;
        public List<SpawnEnemy> Enemies;
    }
}
