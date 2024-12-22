using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Mission_CarDelivery: 차량을 특정 지점으로 배달하는 미션 클래스
[CreateAssetMenu(fileName = "Car delivery - Mission", menuName = "Missions/Car delivery - Mission")]
public class Mission_CarDelivery : Mission
{
    private bool carWasDelivered; // 차량 배달 여부

    // 미션 시작 메서드
    public override void StartMission()
    {
        // 차량 배달 존 활성화
        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

        // 미션 UI 정보 업데이트
        string missionText = "Find a functional vehicle.";
        string missionDetails = "Deliver it to the evacuation point.";
        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);

        // 초기화
        carWasDelivered = false;
        MissionObject_CarToDeliver.OnCarDelivery += CarDeliveryCompleted; // 이벤트 등록

        // 모든 차량에 미션 관련 컴포넌트 추가
        Car_Controller[] cars = FindObjectsOfType<Car_Controller>();
        foreach (var car in cars)
        {
            car.AddComponent<MissionObject_CarToDeliver>();
        }
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }

    // 차량 배달 완료 시 호출되는 메서드
    private void CarDeliveryCompleted()
    {
        carWasDelivered = true; // 배달 상태 업데이트
        MissionObject_CarToDeliver.OnCarDelivery -= CarDeliveryCompleted; // 이벤트 해제

        // 미션 UI 정보 업데이트
        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
    }
}
