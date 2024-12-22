using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_CarDeliveryZone: ���� ��� ������ ��Ÿ���� Ŭ����
public class MissionObject_CarDeliveryZone : MonoBehaviour
{
    // Ʈ���� ������ �ٸ� ��ü�� �������� �� ȣ��
    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���ſ� ������ ��ü�� Car_Controller�� ������ �ִ��� Ȯ��
        Car_Controller car = other.GetComponent<Car_Controller>();

        if (car != null)
        {
            // ������ MissionObject_CarToDeliver�� ������ �ִٸ� ��� �̺�Ʈ ȣ��
            car.GetComponent<MissionObject_CarToDeliver>()?.InvokeOnCarDelivery();
        }
    }
}
