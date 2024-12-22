using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_Training: 훈련 미션을 담당하는 클래스
[CreateAssetMenu(fileName = "New Training Mission", menuName = "Missions/Training - Mission")]
public class Mission_Traning : Mission
{
    // 플레이어가 워프될 위치를 지정
    public Vector3 warpPosition;

    // 미션 시작 시 호출
    public override void StartMission()
    {
        // 플레이어를 지정된 위치로 워프
        WarpPlayerToPosition(warpPosition);
    }

    // 미션 업데이트 시 호출
    public override void UpdateMission()
    {
        // 훈련 미션 텍스트와 컨트롤 설명을 UI에 업데이트
        string missionText = "This is the training room.";
        string missionDetails =
            "Control \r\n" +
            "Move : W/A/S/D \r\n" +
            "Shoot : Left Click \r\n" +
            "Precision Aim : Right Click \r\n" +
            "Running : Shift \r\n" +
            "Use Item/Interaction Car : F \r\n" +
            "Pause : ESC \r\n" +
            "If you want to create an item or enemy, shoot a model.";

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        // ESC 키를 누르면 미션 완료로 간주
        return Input.GetKeyDown(KeyCode.Escape);
    }

    // 플레이어를 지정된 위치로 워프시키는 함수
    public void WarpPlayerToPosition(Vector3 targetPosition)
    {
        // "Player" 태그를 가진 게임 오브젝트를 검색
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 플레이어 위치를 지정된 위치로 이동
            player.transform.position = targetPosition;
            Debug.Log("Player warped to: " + targetPosition);

            // 플레이어의 체력을 최대치로 설정
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.maxHealth = 100000;
                playerHealth.currentHealth = 100000;
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with the 'Player' tag found.");
        }
    }
}
