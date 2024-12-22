using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_SelectedWeaponWindow: 선택된 무기의 정보를 표시하는 UI 관리 클래스
public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData; // 현재 선택된 무기 데이터

    [SerializeField] private Image weaponIcon; // 무기 아이콘 이미지
    [SerializeField] private TextMeshProUGUI weaponInfo; // 무기 정보 텍스트

    private void Start()
    {
        // 초기화: 선택된 무기가 없는 상태로 설정
        weaponData = null;
        UpdateSlotInfo(null);
    }

    // 새로운 무기 데이터를 설정
    public void SetWeaponSlot(Weapon_Data newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(newWeaponData); // 슬롯 UI 업데이트
    }

    // 무기 정보 업데이트
    public void UpdateSlotInfo(Weapon_Data weaponData)
    {
        if (weaponData == null)
        {
            // 무기 데이터가 없을 경우 기본 상태로 설정
            weaponIcon.color = Color.clear; // 아이콘 숨김
            weaponInfo.text = "Select a weapon..."; // 기본 메시지
            return;
        }

        // 무기 데이터가 있을 경우 UI 업데이트
        weaponIcon.color = Color.white; // 아이콘 표시
        weaponIcon.sprite = weaponData.weaponIcon; // 무기 아이콘 설정
        weaponInfo.text = weaponData.weaponInfo; // 무기 정보 텍스트 설정
    }

    // 슬롯이 비어 있는지 확인
    public bool IsEmpty() => weaponData == null;
}
