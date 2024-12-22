using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_MissionSelectionButton: 미션 선택 버튼 UI를 관리하는 클래스
public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI; // 미션 선택 UI 참조
    private TextMeshProUGUI myText; // 버튼의 텍스트

    [SerializeField] private Mission myMission; // 버튼에 연결된 미션 데이터

    private void OnValidate()
    {
        // 버튼 이름을 미션 이름으로 설정 (에디터에서 확인 가능)
        gameObject.name = "Button - Select Mission: " + myMission.missionName;
    }

    public override void Start()
    {
        // 기본 초기화 및 추가 설정
        base.Start();
        missionUI = GetComponentInParent<UI_MissionSelection>(); // 부모의 미션 선택 UI 참조
        myText = GetComponentInChildren<TextMeshProUGUI>(); // 버튼 텍스트 참조
        myText.text = myMission.missionName; // 버튼에 미션 이름 표시
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 버튼 위에 올려졌을 때
        base.OnPointerEnter(eventData);
        missionUI.UpdateMissionDesicription(myMission.missionDescription); // 미션 설명 업데이트
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 벗어났을 때
        base.OnPointerExit(eventData);
        missionUI.UpdateMissionDesicription("Choose a mission"); // 기본 메시지로 복구
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 버튼이 클릭되었을 때
        base.OnPointerDown(eventData);
        MissionManager.instance.SetCurrentMission(myMission); // 선택한 미션 설정
    }
}
