using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionManager: 게임의 미션을 관리하는 클래스
public class MissionManager : MonoBehaviour
{
    public static MissionManager instance; // 싱글턴 인스턴스

    public Mission currentMission; // 현재 활성화된 미션

    private void Awake()
    {
        // 싱글턴 인스턴스 초기화
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple MissionManager instances found. Destroying the duplicate.");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 현재 활성화된 미션의 상태를 업데이트 (null 확인)
        currentMission?.UpdateMission();
    }

    // 새로운 미션을 설정
    public void SetCurrentMission(Mission newMission)
    {
        if (newMission == null)
        {
            Debug.LogError("Trying to set a null mission.");
            return;
        }

        currentMission = newMission;
        Debug.Log($"Current mission set to: {currentMission.missionName}");
    }

    // 현재 미션 시작
    public void StartMission()
    {
        if (currentMission == null)
        {
            Debug.LogError("No mission set. Cannot start a mission.");
            return;
        }

        currentMission.StartMission();
        Debug.Log($"Mission started: {currentMission.missionName}");
    }

    // 현재 미션이 완료되었는지 확인
    public bool MissionCompleted()
    {
        if (currentMission == null)
        {
            Debug.LogWarning("No mission set. Cannot check completion status.");
            return false;
        }

        return currentMission.MissionCompleted();
    }
}
