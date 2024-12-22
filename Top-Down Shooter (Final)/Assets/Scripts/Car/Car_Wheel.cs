// �ڵ��� ������ ���۰� ������ �����ϴ� Ŭ����

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ������ ��Ÿ���� ������ (����, �ķ�)
public enum AxelType { Front, Back }

[RequireComponent(typeof(WheelCollider))] // WheelCollider�� �ʼ��� �䱸
public class Car_Wheel : MonoBehaviour
{
    public AxelType axelType; // ������ �� Ÿ��
    public WheelCollider cd { get; private set; } // ������ ���� ó���� ����ϴ� WheelCollider
    public TrailRenderer trail { get; private set; } // ������ ����� Ʈ���� ȿ��
    public GameObject model; // ������ �ð��� ��

    private float defaultSideStiffnes; // �⺻ ���� ������

    private void Awake()
    {
        cd = GetComponent<WheelCollider>(); // WheelCollider ������Ʈ ��������
        trail = GetComponentInChildren<TrailRenderer>(); // TrailRenderer ������Ʈ ��������

        trail.emitting = false; // �ʱ⿡�� Ʈ���� ��Ȱ��ȭ

        // ���� ���� �������� �ʾҴٸ� �⺻ �𵨷� ����
        if (model == null)
            model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaultStiffnes(float newValue)
    {
        // �⺻ ���� �������� �����ϰ� �ʱ�ȭ
        defaultSideStiffnes = newValue;
        RestoreDefaultStiffnes();
    }

    public void RestoreDefaultStiffnes()
    {
        // �⺻ ���� �������� WheelCollider�� ����
        WheelFrictionCurve sidewayFriction = cd.sidewaysFriction;
        sidewayFriction.stiffness = defaultSideStiffnes;
        cd.sidewaysFriction = sidewayFriction;
    }
}
