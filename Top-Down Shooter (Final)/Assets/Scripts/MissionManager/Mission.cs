using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission: 모든 미션의 기본 클래스 (추상 클래스)
public abstract class Mission : ScriptableObject
{
    public string missionName; // 미션 이름

    [TextArea]
    public string missionDescription; // 미션 설명 (여러 줄 입력 가능)

    // 미션 시작 메서드 (구현은 서브클래스(각각 미션)에서 정의)
    public abstract void StartMission();

    // 미션 완료 여부를 확인하는 메서드 (구현은 서브클래스(각각 미션)에서 정의)
    public abstract bool MissionCompleted();

    // 미션 업데이트 메서드 (구현은 서브클래스(각각 미션)에서 정의)
    public virtual void UpdateMission()
    {
        
    }
}
