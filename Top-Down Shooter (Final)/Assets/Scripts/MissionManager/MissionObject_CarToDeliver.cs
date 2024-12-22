using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_CarToDeliver: 배달해야 할 차량을 나타내는 클래스
public class MissionObject_CarToDeliver : MonoBehaviour
{
    // 차량이 성공적으로 배달되었을 때 호출되는 이벤트
    public static event Action OnCarDelivery;

    // 차량 배달 완료 이벤트 호출 메서드
    public void InvokeOnCarDelivery()
    {
        OnCarDelivery?.Invoke(); // 이벤트가 등록되어 있다면 호출
    }
}
