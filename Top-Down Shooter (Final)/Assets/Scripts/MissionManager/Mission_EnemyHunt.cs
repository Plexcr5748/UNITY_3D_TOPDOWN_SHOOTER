using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Mission_EnemyHunt: 지정된 적을 제거하는 미션 클래스
[CreateAssetMenu(fileName = "Hunt - Mission", menuName = "Missions/Hunt - Mission")]
public class Mission_EnemyHunt : Mission
{
    public int amountToKill = 12; // 제거해야 할 적의 수
    public EnemyType enemyType; // 타겟 적의 유형

    private int killsToGo; // 남은 적의 수

    // 미션 시작 메서드
    public override void StartMission()
    {
        killsToGo = amountToKill; // 초기화
        UpdateMissionUI(); // UI 업데이트

        MissionObject_HuntTarget.OnTargetKilled += EliminateTarget; // 적 제거 이벤트 등록

        List<Enemy> validEnemies = new List<Enemy>();

        // 적 유형에 따라 타겟 선택
        if (enemyType == EnemyType.Random)
        {
            validEnemies = LevelGenerator.instance.GetEnemyList();
        }
        else
        {
            foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
            {
                if (enemy.enemyType == enemyType)
                {
                    validEnemies.Add(enemy);
                }
            }
        }

        // 지정된 수만큼 적에게 타겟 컴포넌트 추가
        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    // 타겟 제거 시 호출되는 메서드
    private void EliminateTarget()
    {
        killsToGo--; // 남은 적 수 감소
        UpdateMissionUI(); // UI 업데이트

        if (killsToGo <= 0)
        {
            // 미션 완료 시 미션 정보 업데이트 및 이벤트 해제
            UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
            MissionObject_HuntTarget.OnTargetKilled -= EliminateTarget;
        }
    }

    // 미션 UI 업데이트
    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + amountToKill + " enemies with signal disruptor.";
        string missionDetaiils = "Targets left: " + killsToGo;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetaiils);
    }
}
