using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Mission_CarDelivery: ������ Ư�� �������� ����ϴ� �̼� Ŭ����
[CreateAssetMenu(fileName = "Car delivery - Mission", menuName = "Missions/Car delivery - Mission")]
public class Mission_CarDelivery : Mission
{
    private bool carWasDelivered; // ���� ��� ����

    // �̼� ���� �޼���
    public override void StartMission()
    {
        // ���� ��� �� Ȱ��ȭ
        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

        // �̼� UI ���� ������Ʈ
        string missionText = "Find a functional vehicle.";
        string missionDetails = "Deliver it to the evacuation point.";
        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);

        // �ʱ�ȭ
        carWasDelivered = false;
        MissionObject_CarToDeliver.OnCarDelivery += CarDeliveryCompleted; // �̺�Ʈ ���

        // ��� ������ �̼� ���� ������Ʈ �߰�
        Car_Controller[] cars = FindObjectsOfType<Car_Controller>();
        foreach (var car in cars)
        {
            car.AddComponent<MissionObject_CarToDeliver>();
        }
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }

    // ���� ��� �Ϸ� �� ȣ��Ǵ� �޼���
    private void CarDeliveryCompleted()
    {
        carWasDelivered = true; // ��� ���� ������Ʈ
        MissionObject_CarToDeliver.OnCarDelivery -= CarDeliveryCompleted; // �̺�Ʈ ����

        // �̼� UI ���� ������Ʈ
        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
    }
}
