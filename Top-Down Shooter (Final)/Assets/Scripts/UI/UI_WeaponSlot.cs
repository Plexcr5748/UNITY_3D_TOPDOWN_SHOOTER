using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_WeaponSlot: 무기 슬롯 UI를 관리하는 클래스
public class UI_WeaponSlot : MonoBehaviour
{
    public Image weaponIcon; // 무기 아이콘 이미지
    public TextMeshProUGUI ammoText; // 탄약 정보를 표시하는 텍스트

    private void Awake()
    {
        // 무기 아이콘과 탄약 텍스트 초기화
        weaponIcon = GetComponentInChildren<Image>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // 무기 슬롯 UI 업데이트 메서드
    public void UpdateWeaponSlot(Weapon myWeapon, bool activeWeapon)
    {
        if (myWeapon == null)
        {
            // 무기가 없는 경우 슬롯을 초기화
            weaponIcon.color = Color.clear; // 아이콘 숨기기
            ammoText.text = ""; // 텍스트 비우기
            return;
        }

        // 현재 무기가 활성 상태인지에 따라 색상 설정
        Color newColor = activeWeapon ? Color.white : new Color(1, 1, 1, .35f);

        weaponIcon.color = newColor; // 무기 아이콘 색상 설정
        weaponIcon.sprite = myWeapon.weaponData.weaponIcon; // 무기 아이콘 이미지 설정

        // 탄약 정보 텍스트 업데이트
        ammoText.text = myWeapon.bulletsInMagazine + "/" + myWeapon.totalReserveAmmo;
        ammoText.color = Color.white; // 텍스트 색상 설정
    }
}
