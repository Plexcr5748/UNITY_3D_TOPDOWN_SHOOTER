using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UI_MissionSelection: �̼� ���� ȭ���� UI�� �����ϴ� Ŭ����
public class UI_MissionSelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionDesciprtion; // �̼� ���� �ؽ�Ʈ

    // �̼� ���� ������Ʈ �޼���
    public void UpdateMissionDesicription(string text)
    {
        missionDesciprtion.text = text; // ���޹��� �ؽ�Ʈ�� �̼� ���� �ؽ�Ʈ�� ����
    }
}
