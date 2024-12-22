using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // 스폰할 적 프리팹 배열
    [SerializeField] private float spawnRadius = 5f; // 스폰 위치 근처 반경
    [SerializeField] private LayerMask groundLayer; // 지면 레이어 설정
    [SerializeField] private float spawnDelay = 8f; // 일반 몬스터 재스폰 딜레이
    [SerializeField] private float bossSpawnChance = 0.1f; // 보스 몬스터 스폰 확률 (10%)
    [SerializeField] private float bossRespawnDelay = 30f; // 보스 처치 후 재스폰 딜레이
    [SerializeField] private float bossSpawnDelayAfterDefence = 20f; // 방어전 시작 후 보스 스폰 지연 시간
    public Mission_LastDefence lastDefence;
    public bool EnemyRespawn = false;
    public int currentEnemyCount = 0; // 현재 스폰된 적 수
    public int maxEnemies = 3; // 각 스폰 포인트에서 최대 스폰 가능 적 수

    private static bool bossSpawned = false; // 전체 스포너에서 보스가 스폰되었는지 여부
    private bool bossSpawningAllowed = false; // 보스 스폰 허용 여부

    private void Start()
    {
        EnemyRespawn = false;
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        if (currentEnemyCount >= maxEnemies)
        {
            Debug.Log("Spawn limit reached for this spawner.");
            return; // 스폰 제한에 도달하면 중단
        }

        // 보스 몬스터 스폰 여부 결정
        bool spawnBoss = !bossSpawned && bossSpawningAllowed && Random.value <= bossSpawnChance;

        // 적 선택
        GameObject enemyToSpawn;
        if (spawnBoss)
        {
            // 보스 몬스터 스폰
            enemyToSpawn = System.Array.Find(enemyPrefabs, e => e.CompareTag("Enemy_Boss"));
            if (enemyToSpawn == null)
            {
                Debug.LogWarning("No boss monster found in enemyPrefabs!");
                return;
            }
            bossSpawned = true; // 보스 몬스터가 스폰되었음을 기록
        }
        else
        {
            // 일반 적 스폰
            enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (enemyToSpawn.CompareTag("Enemy_Boss")) // 일반 스폰에서 보스 선택 방지
                return;
        }

        // 랜덤 위치 설정 (스폰 포인트 주변)
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Raycast로 지면 확인
        if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            randomPosition = hit.point; // 충돌 지점이 지면

            // 적 스폰
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, randomPosition, Quaternion.identity);

            if (lastDefence.defenceBegun == true)
            {
                spawnedEnemy.GetComponent<Enemy>().aggresionRange = 500;
            }

            // 현재 스폰된 적 수 증가
            currentEnemyCount++;

            // 적이 파괴될 때 카운트를 감소시키기 위해 이벤트 연결
            spawnedEnemy.GetComponent<Enemy_Health>().OnEnemyDestroyed += () =>
            {
                HandleEnemyDestroyed(spawnBoss); // 보스 여부 전달
            };

            Debug.Log($"Enemy spawned at: {randomPosition}");
        }
        else
        {
            Debug.LogWarning("Failed to find ground for enemy spawn.");
        }
    }

    private void HandleEnemyDestroyed(bool wasBoss)
    {
        if (lastDefence.defenceBegun == false)
            return;

        currentEnemyCount--;

        if (wasBoss)
        {
            bossSpawned = false; // 보스 스폰 가능 상태로 변경
            StartCoroutine(SpawnBossWithDelay());
        }
        else
        {
            StartCoroutine(SpawnNormalEnemyWithDelay());
        }
    }

    private IEnumerator SpawnBossWithDelay()
    {
        yield return new WaitForSeconds(bossRespawnDelay); // 보스 몬스터 재스폰 딜레이
        if (bossSpawningAllowed) // 보스 스폰이 허용된 상태일 때만 재스폰
        {
            SpawnEnemies();
        }
    }

    private IEnumerator SpawnNormalEnemyWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay); // 일반 몬스터 재스폰 딜레이
        if (lastDefence.defenceBegun) // 방어전이 진행 중일 때만 재스폰
        {
            SpawnEnemies();
        }
    }

    private IEnumerator EnableBossSpawningWithDelay()
    {
        yield return new WaitForSeconds(bossSpawnDelayAfterDefence); // 방어전 시작 후 보스 스폰 지연 시간
        bossSpawningAllowed = true;
        Debug.Log("Boss spawning is now allowed.");
    }

    private void Update()
    {
        if (lastDefence.defenceBegun && !bossSpawningAllowed)
        {
            // 방어전 시작 후 일정 시간 후 보스 스폰 허용
            StartCoroutine(EnableBossSpawningWithDelay());
        }

        if (currentEnemyCount < maxEnemies && EnemyRespawn == true)
        {
            SpawnEnemies();
        }
    }
}
