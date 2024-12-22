using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_CarToDeliver: ����ؾ� �� ������ ��Ÿ���� Ŭ����
public class MissionObject_CarToDeliver : MonoBehaviour
{
    // ������ ���������� ��޵Ǿ��� �� ȣ��Ǵ� �̺�Ʈ
    public static event Action OnCarDelivery;

    // ���� ��� �Ϸ� �̺�Ʈ ȣ�� �޼���
    public void InvokeOnCarDelivery()
    {
        OnCarDelivery?.Invoke(); // �̺�Ʈ�� ��ϵǾ� �ִٸ� ȣ��
    }
}
