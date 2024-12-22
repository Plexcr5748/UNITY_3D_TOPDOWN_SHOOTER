using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

// UI_WeaponSelection: 무기 선택 UI를 관리하는 클래스
public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn; // 다음으로 전환할 UI
    public UI_SelectedWeaponWindow[] selectedWeapon; // 선택된 무기를 표시하는 슬롯 배열

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText; // 경고 메시지 텍스트
    [SerializeField] private float disaperaingSpeed = .25f; // 경고 메시지 사라지는 속도
    private float currentWarningAlpha; // 현재 경고 메시지 투명도
    private float targetWarningAlpha; // 목표 경고 메시지 투명도

    private void Start()
    {
        // 슬롯 초기화
        selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        // 경고 메시지 투명도 업데이트
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disaperaingSpeed;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    // 무기 선택 확인
    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn); // 다음 UI로 전환
            UI.instance.StartLevelGeneration(); // 레벨 생성 시작
        }
        else
        {
            ShowWarningMessage("Select at least one weapon."); // 경고 메시지 표시
        }
    }

    // 최소 하나 이상의 무기가 선택되었는지 확인
    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;

    // 선택된 무기의 데이터를 반환
    public List<Weapon_Data> SelectedWeaponData()
    {
        List<Weapon_Data> selectedData = new List<Weapon_Data>();

        foreach (UI_SelectedWeaponWindow weapon in selectedWeapon)
        {
            if (weapon.weaponData != null)
                selectedData.Add(weapon.weaponData);
        }

        return selectedData;
    }

    // 빈 슬롯 찾기
    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        foreach (UI_SelectedWeaponWindow slot in selectedWeapon)
        {
            if (slot.IsEmpty())
                return slot;
        }

        return null; // 빈 슬롯이 없을 경우 null 반환
    }

    // 특정 타입의 무기를 가진 슬롯 찾기
    public UI_SelectedWeaponWindow FindSlowWithWeaponOfType(Weapon_Data weaponData)
    {
        foreach (UI_SelectedWeaponWindow slot in selectedWeapon)
        {
            if (slot.weaponData == weaponData)
                return slot;
        }

        return null; // 해당 무기를 가진 슬롯이 없을 경우 null 반환
    }

    // 경고 메시지 표시
    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white; // 텍스트를 보이게 설정
        warningText.text = message; // 메시지 업데이트

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0; // 메시지를 서서히 사라지게 설정
    }
}
