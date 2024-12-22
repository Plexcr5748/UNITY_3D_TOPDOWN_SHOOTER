using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_Timer: 제한 시간 내에 완료해야 하는 미션 클래스
[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Missions/Timer - Mission")]
public class Mission_Timer : Mission
{
    public float time = 300; // 제한 시간 (초 단위)
    private float currentTime; // 남은 시간
    private bool missionStart = false; // 미션 시작 여부

    // 미션 시작 메서드
    public override void StartMission()
    {
        currentTime = time; // 남은 시간을 초기화
        missionStart = true; // 미션 시작 상태 설정
    }

    // 미션 업데이트 메서드
    public override void UpdateMission()
    {
        if (!missionStart)
            return; // 미션이 시작되지 않았으면 아무 작업도 하지 않음

        currentTime -= Time.deltaTime; // 매 프레임마다 남은 시간 감소

        if (currentTime <= 0)
        {
            missionStart = false; // 미션 종료
            GameManager.instance.GameOver(); // 게임 오버 처리

            // 플레이어 체력을 0으로 설정
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player_Health playerHealth = player.GetComponent<Player_Health>();
                if (playerHealth != null)
                {
                    playerHealth.Die();
                }
            }
            else
            {
                Debug.LogWarning("No GameObject with the 'Player' tag found.");
            }
        }

        // 남은 시간을 UI에 표시
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
        string missionText = "Get to evacuation point before plane take off.";
        string missionDetails = "Time left: " + timeText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        return currentTime > 0 && missionStart; // 미션이 시작되었고 시간이 남아 있으면 완료되지 않은 상태
    }

    // 미션 초기화 메서드
    public void ResetMission()
    {
        missionStart = false; // 미션 시작 상태 초기화
        currentTime = time; // 시간을 다시 초기화
    }
}
