using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_HuntTarget: 사냥 미션의 대상이 되는 객체 클래스
public class MissionObject_HuntTarget : MonoBehaviour
{
    // 대상이 사망했을 때 호출되는 이벤트
    public static event Action OnTargetKilled;

    // 대상이 사망했을 때 이벤트를 호출하는 메서드
    public void InvokeOnTargetKilled()
    {
        OnTargetKilled?.Invoke(); // 이벤트가 등록되어 있다면 호출
    }
}
