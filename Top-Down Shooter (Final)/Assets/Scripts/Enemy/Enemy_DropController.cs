using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal.Profiling.Memory.Experimental;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

// 적의 아이템 드롭을 제어하는 클래스
public class Enemy_DropController : MonoBehaviour
{
    [SerializeField] private GameObject missionObjectKey; // 미션 키 아이템
    [SerializeField] private GameObject Heal; // 체력 회복 아이템
    [SerializeField] private GameObject Ammo; // 탄약 아이템
    private MissionManager missionManager; // 미션 관리 매니저
    private Enemy Enemy; // 적 객체 참조

    // 초기화
    private void Start()
    {
        Enemy = GetComponent<Enemy>();
        missionManager = MissionManager.instance;
    }

    // 새로운 미션 키를 설정
    public void GiveKey(GameObject newKey) => missionObjectKey = newKey;

    // 아이템 드롭 처리
    public void DropItems()
    {
        // Mission_KeyFind 미션 진행 중일 경우 키 드롭
        if (missionManager.currentMission is Mission_KeyFind keyFindMission && missionObjectKey != null)
        {
            CreateItem(missionObjectKey); // 키 아이템 생성
            missionObjectKey = null; // 드롭 후 키 아이템 초기화
            return; // 키 아이템 드롭 후 다른 아이템은 드롭하지 않음
        }

        // 원거리 적일 경우
        if (Enemy.enemyType == EnemyType.Range)
        {
            int randomNumber = Random.Range(1, 101); // 1 ~ 100 사이 랜덤 값 생성
            if (randomNumber <= 30) // 30 이하일 경우 탄약 드롭
            {
                CreateItem(Ammo);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true; // 충돌 감지 트리거 활성화
            }
            else if (30 < randomNumber && randomNumber <= 60) // 31 ~ 60 사이일 경우 힐킷 드롭
            {
                CreateItem(Heal);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true;
            }
            return;
        }
        // 근접 적일 경우
        else if (Enemy.enemyType == EnemyType.Melee)
        {
            int randomNumber = Random.Range(1, 101); // 1 ~ 100 사이 랜덤 값 생성
            if (randomNumber <= 50) // 50 이하일 경우 힐킷 드롭
            {
                CreateItem(Heal);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true;
            }
            return;
        }
    }

    // 아이템 생성
    private void CreateItem(GameObject go)
    {
        GameObject newItem = Instantiate(go, transform.position + Vector3.up, Quaternion.identity); // 아이템 생성
    }

    // 탄약 아이템 생성 및 부모에서 분리
    private void CreateAmmo(GameObject go)
    {
        GameObject newAmmo = Instantiate(go, transform.position, Quaternion.identity);

        foreach (Transform child in newAmmo.transform)
        {
            child.SetParent(null); // 자식 오브젝트를 부모에서 분리
        }
    }
}
