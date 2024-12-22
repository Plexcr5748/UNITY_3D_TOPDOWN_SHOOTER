using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_CarDeliveryZone: 차량 배달 구역을 나타내는 클래스
public class MissionObject_CarDeliveryZone : MonoBehaviour
{
    // 트리거 영역에 다른 객체가 진입했을 때 호출
    private void OnTriggerEnter(Collider other)
    {
        // 트리거에 진입한 객체가 Car_Controller를 가지고 있는지 확인
        Car_Controller car = other.GetComponent<Car_Controller>();

        if (car != null)
        {
            // 차량이 MissionObject_CarToDeliver를 가지고 있다면 배달 이벤트 호출
            car.GetComponent<MissionObject_CarToDeliver>()?.InvokeOnCarDelivery();
        }
    }
}
