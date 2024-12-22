using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training_EnemySpawn : MonoBehaviour
{
    // 스폰할 프리팹들을 배열로 설정
    public GameObject[] spawnPrefabs;

    // 스폰할 위치를 설정
    public Vector3 spawnPosition;

    // 충돌 감지 함수
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 Bullet 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 프리팹 배열이 비어있지 않은 경우 실행
            if (spawnPrefabs.Length > 0)
            {
                // 프리팹 배열에서 랜덤으로 하나 선택
                int randomIndex = Random.Range(0, spawnPrefabs.Length);
                GameObject selectedPrefab = spawnPrefabs[randomIndex];

                // 프리팹을 스폰 위치에 생성
                Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

                Debug.Log($"Spawned prefab: {selectedPrefab.name} at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning("No prefabs assigned to the spawnPrefabs array.");
            }

            // Bullet 오브젝트를 삭제
            Destroy(collision.gameObject);
        }
    }
}
