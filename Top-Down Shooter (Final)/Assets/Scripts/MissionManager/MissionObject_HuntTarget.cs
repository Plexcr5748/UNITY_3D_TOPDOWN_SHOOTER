using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_HuntTarget: ��� �̼��� ����� �Ǵ� ��ü Ŭ����
public class MissionObject_HuntTarget : MonoBehaviour
{
    // ����� ������� �� ȣ��Ǵ� �̺�Ʈ
    public static event Action OnTargetKilled;

    // ����� ������� �� �̺�Ʈ�� ȣ���ϴ� �޼���
    public void InvokeOnTargetKilled()
    {
        OnTargetKilled?.Invoke(); // �̺�Ʈ�� ��ϵǾ� �ִٸ� ȣ��
    }
}
