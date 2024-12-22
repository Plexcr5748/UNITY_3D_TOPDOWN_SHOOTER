using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UI_MissionSelection: 미션 선택 화면의 UI를 관리하는 클래스
public class UI_MissionSelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionDesciprtion; // 미션 설명 텍스트

    // 미션 설명 업데이트 메서드
    public void UpdateMissionDesicription(string text)
    {
        missionDesciprtion.text = text; // 전달받은 텍스트를 미션 설명 텍스트에 설정
    }
}
