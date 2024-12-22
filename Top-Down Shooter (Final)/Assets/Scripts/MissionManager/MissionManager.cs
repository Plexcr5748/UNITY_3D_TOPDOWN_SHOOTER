using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionManager: ������ �̼��� �����ϴ� Ŭ����
public class MissionManager : MonoBehaviour
{
    public static MissionManager instance; // �̱��� �ν��Ͻ�

    public Mission currentMission; // ���� Ȱ��ȭ�� �̼�

    private void Awake()
    {
        // �̱��� �ν��Ͻ� �ʱ�ȭ
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple MissionManager instances found. Destroying the duplicate.");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // ���� Ȱ��ȭ�� �̼��� ���¸� ������Ʈ (null Ȯ��)
        currentMission?.UpdateMission();
    }

    // ���ο� �̼��� ����
    public void SetCurrentMission(Mission newMission)
    {
        if (newMission == null)
        {
            Debug.LogError("Trying to set a null mission.");
            return;
        }

        currentMission = newMission;
        Debug.Log($"Current mission set to: {currentMission.missionName}");
    }

    // ���� �̼� ����
    public void StartMission()
    {
        if (currentMission == null)
        {
            Debug.LogError("No mission set. Cannot start a mission.");
            return;
        }

        currentMission.StartMission();
        Debug.Log($"Mission started: {currentMission.missionName}");
    }

    // ���� �̼��� �Ϸ�Ǿ����� Ȯ��
    public bool MissionCompleted()
    {
        if (currentMission == null)
        {
            Debug.LogWarning("No mission set. Cannot check completion status.");
            return false;
        }

        return currentMission.MissionCompleted();
    }
}
