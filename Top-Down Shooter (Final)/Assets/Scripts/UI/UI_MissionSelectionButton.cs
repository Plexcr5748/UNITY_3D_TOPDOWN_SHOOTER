using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_MissionSelectionButton: �̼� ���� ��ư UI�� �����ϴ� Ŭ����
public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI; // �̼� ���� UI ����
    private TextMeshProUGUI myText; // ��ư�� �ؽ�Ʈ

    [SerializeField] private Mission myMission; // ��ư�� ����� �̼� ������

    private void OnValidate()
    {
        // ��ư �̸��� �̼� �̸����� ���� (�����Ϳ��� Ȯ�� ����)
        gameObject.name = "Button - Select Mission: " + myMission.missionName;
    }

    public override void Start()
    {
        // �⺻ �ʱ�ȭ �� �߰� ����
        base.Start();
        missionUI = GetComponentInParent<UI_MissionSelection>(); // �θ��� �̼� ���� UI ����
        myText = GetComponentInChildren<TextMeshProUGUI>(); // ��ư �ؽ�Ʈ ����
        myText.text = myMission.missionName; // ��ư�� �̼� �̸� ǥ��
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� ��ư ���� �÷����� ��
        base.OnPointerEnter(eventData);
        missionUI.UpdateMissionDesicription(myMission.missionDescription); // �̼� ���� ������Ʈ
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� ��ư���� ����� ��
        base.OnPointerExit(eventData);
        missionUI.UpdateMissionDesicription("Choose a mission"); // �⺻ �޽����� ����
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // ��ư�� Ŭ���Ǿ��� ��
        base.OnPointerDown(eventData);
        MissionManager.instance.SetCurrentMission(myMission); // ������ �̼� ����
    }
}
