using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // ������ �� ������ �迭
    [SerializeField] private float spawnRadius = 5f; // ���� ��ġ ��ó �ݰ�
    [SerializeField] private LayerMask groundLayer; // ���� ���̾� ����
    [SerializeField] private float spawnDelay = 8f; // �Ϲ� ���� �罺�� ������
    [SerializeField] private float bossSpawnChance = 0.1f; // ���� ���� ���� Ȯ�� (10%)
    [SerializeField] private float bossRespawnDelay = 30f; // ���� óġ �� �罺�� ������
    [SerializeField] private float bossSpawnDelayAfterDefence = 20f; // ����� ���� �� ���� ���� ���� �ð�
    public Mission_LastDefence lastDefence;
    public bool EnemyRespawn = false;
    public int currentEnemyCount = 0; // ���� ������ �� ��
    public int maxEnemies = 3; // �� ���� ����Ʈ���� �ִ� ���� ���� �� ��

    private static bool bossSpawned = false; // ��ü �����ʿ��� ������ �����Ǿ����� ����
    private bool bossSpawningAllowed = false; // ���� ���� ��� ����

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
            return; // ���� ���ѿ� �����ϸ� �ߴ�
        }

        // ���� ���� ���� ���� ����
        bool spawnBoss = !bossSpawned && bossSpawningAllowed && Random.value <= bossSpawnChance;

        // �� ����
        GameObject enemyToSpawn;
        if (spawnBoss)
        {
            // ���� ���� ����
            enemyToSpawn = System.Array.Find(enemyPrefabs, e => e.CompareTag("Enemy_Boss"));
            if (enemyToSpawn == null)
            {
                Debug.LogWarning("No boss monster found in enemyPrefabs!");
                return;
            }
            bossSpawned = true; // ���� ���Ͱ� �����Ǿ����� ���
        }
        else
        {
            // �Ϲ� �� ����
            enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (enemyToSpawn.CompareTag("Enemy_Boss")) // �Ϲ� �������� ���� ���� ����
                return;
        }

        // ���� ��ġ ���� (���� ����Ʈ �ֺ�)
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Raycast�� ���� Ȯ��
        if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, groundLayer))
        {
            randomPosition = hit.point; // �浹 ������ ����

            // �� ����
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, randomPosition, Quaternion.identity);

            if (lastDefence.defenceBegun == true)
            {
                spawnedEnemy.GetComponent<Enemy>().aggresionRange = 500;
            }

            // ���� ������ �� �� ����
            currentEnemyCount++;

            // ���� �ı��� �� ī��Ʈ�� ���ҽ�Ű�� ���� �̺�Ʈ ����
            spawnedEnemy.GetComponent<Enemy_Health>().OnEnemyDestroyed += () =>
            {
                HandleEnemyDestroyed(spawnBoss); // ���� ���� ����
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
            bossSpawned = false; // ���� ���� ���� ���·� ����
            StartCoroutine(SpawnBossWithDelay());
        }
        else
        {
            StartCoroutine(SpawnNormalEnemyWithDelay());
        }
    }

    private IEnumerator SpawnBossWithDelay()
    {
        yield return new WaitForSeconds(bossRespawnDelay); // ���� ���� �罺�� ������
        if (bossSpawningAllowed) // ���� ������ ���� ������ ���� �罺��
        {
            SpawnEnemies();
        }
    }

    private IEnumerator SpawnNormalEnemyWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay); // �Ϲ� ���� �罺�� ������
        if (lastDefence.defenceBegun) // ������� ���� ���� ���� �罺��
        {
            SpawnEnemies();
        }
    }

    private IEnumerator EnableBossSpawningWithDelay()
    {
        yield return new WaitForSeconds(bossSpawnDelayAfterDefence); // ����� ���� �� ���� ���� ���� �ð�
        bossSpawningAllowed = true;
        Debug.Log("Boss spawning is now allowed.");
    }

    private void Update()
    {
        if (lastDefence.defenceBegun && !bossSpawningAllowed)
        {
            // ����� ���� �� ���� �ð� �� ���� ���� ���
            StartCoroutine(EnableBossSpawningWithDelay());
        }

        if (currentEnemyCount < maxEnemies && EnemyRespawn == true)
        {
            SpawnEnemies();
        }
    }
}
