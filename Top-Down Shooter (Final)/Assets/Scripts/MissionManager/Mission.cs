using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission: ��� �̼��� �⺻ Ŭ���� (�߻� Ŭ����)
public abstract class Mission : ScriptableObject
{
    public string missionName; // �̼� �̸�

    [TextArea]
    public string missionDescription; // �̼� ���� (���� �� �Է� ����)

    // �̼� ���� �޼��� (������ ����Ŭ����(���� �̼�)���� ����)
    public abstract void StartMission();

    // �̼� �Ϸ� ���θ� Ȯ���ϴ� �޼��� (������ ����Ŭ����(���� �̼�)���� ����)
    public abstract bool MissionCompleted();

    // �̼� ������Ʈ �޼��� (������ ����Ŭ����(���� �̼�)���� ����)
    public virtual void UpdateMission()
    {
        
    }
}
