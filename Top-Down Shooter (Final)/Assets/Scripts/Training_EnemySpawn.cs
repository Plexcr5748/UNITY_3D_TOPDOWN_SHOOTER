using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training_EnemySpawn : MonoBehaviour
{
    // ������ �����յ��� �迭�� ����
    public GameObject[] spawnPrefabs;

    // ������ ��ġ�� ����
    public Vector3 spawnPosition;

    // �浹 ���� �Լ�
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� Bullet �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // ������ �迭�� ������� ���� ��� ����
            if (spawnPrefabs.Length > 0)
            {
                // ������ �迭���� �������� �ϳ� ����
                int randomIndex = Random.Range(0, spawnPrefabs.Length);
                GameObject selectedPrefab = spawnPrefabs[randomIndex];

                // �������� ���� ��ġ�� ����
                Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

                Debug.Log($"Spawned prefab: {selectedPrefab.name} at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning("No prefabs assigned to the spawnPrefabs array.");
            }

            // Bullet ������Ʈ�� ����
            Destroy(collision.gameObject);
        }
    }
}
