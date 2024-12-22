using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_KeyFind: 키를 찾는 미션 클래스
[CreateAssetMenu(fileName = "Find Key - Mission", menuName = "Missions/Key - Mission")]
public class Mission_KeyFind : Mission
{
    [SerializeField] private GameObject key; // 미션에서 사용할 키 오브젝트
    private bool keyFound; // 키 발견 여부

    // 미션 시작 메서드
    public override void StartMission()
    {
        // 키를 주웠을 때 호출되는 이벤트 등록
        MissionObject_Key.OnKeyPickedUp += PickUpKey;

        // 미션 UI 업데이트
        UI.instance.inGameUI.UpdateMissionInfo("Find a key-holder. Retrieve the key.");

        // 랜덤한 적을 선택하여 키를 소유하게 설정
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key); // 적에게 키 부여
        enemy.MakeEnemyVIP(); // 적을 VIP(특별 대상)로 설정
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        return keyFound; // 키를 발견했는지 반환
    }

    // 키를 주웠을 때 호출되는 메서드
    private void PickUpKey()
    {
        keyFound = true; // 키 발견 상태 업데이트

        // 이벤트 해제
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;

        // 미션 UI 업데이트
        UI.instance.inGameUI.UpdateMissionInfo("You've got the key! \n Get to the evacuation point.");
    }
}
