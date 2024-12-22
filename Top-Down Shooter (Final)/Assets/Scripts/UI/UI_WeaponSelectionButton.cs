using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_WeaponSelectionButton: 무기 선택 버튼 UI를 관리하는 클래스
public class UI_WeaponSelectionButton : UI_Button
{
    private UI_WeaponSelection weaponSelectionUI; // 무기 선택 UI 참조

    [SerializeField] private Weapon_Data weaponData; // 버튼에 연결된 무기 데이터
    [SerializeField] private Image weaponIcon; // 버튼에 표시될 무기 아이콘

    private UI_SelectedWeaponWindow emptySlot; // 빈 슬롯 참조

    private void OnValidate()
    {
        // 에디터에서 버튼 이름 자동 설정
        gameObject.name = "Button - Select Weapon: " + weaponData.weaponType;
    }

    public override void Start()
    {
        // 기본 초기화 및 무기 선택 UI 참조 설정
        base.Start();

        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
        weaponIcon.sprite = weaponData.weaponIcon; // 무기 아이콘 설정
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 버튼 위로 올라왔을 때
        base.OnPointerEnter(eventData);
        weaponIcon.color = Color.yellow; // 아이콘 색상 변경

        emptySlot = weaponSelectionUI.FindEmptySlot(); // 빈 슬롯 검색
        emptySlot?.UpdateSlotInfo(weaponData); // 빈 슬롯이 있으면 무기 정보로 업데이트
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 벗어났을 때
        base.OnPointerExit(eventData);
        weaponIcon.color = Color.white; // 아이콘 색상 복구

        emptySlot?.UpdateSlotInfo(null); // 빈 슬롯 정보 초기화
        emptySlot = null; // 빈 슬롯 참조 초기화
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 버튼이 클릭되었을 때
        base.OnPointerDown(eventData);
        weaponIcon.color = Color.white; // 아이콘 색상 복구

        // 슬롯 상태 확인
        bool noMoreEmptySlots = weaponSelectionUI.FindEmptySlot() == null; // 빈 슬롯 없음
        bool noThisWeaponInSlots = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData) == null; // 동일한 무기가 슬롯에 없음

        if (noMoreEmptySlots && noThisWeaponInSlots)
        {
            // 빈 슬롯이 없고 동일한 무기가 선택되지 않은 경우 경고 메시지 표시
            weaponSelectionUI.ShowWarningMessage("No Empty Slots...");
            return;
        }

        UI_SelectedWeaponWindow slotBusyWithThisWeapon = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData);

        if (slotBusyWithThisWeapon != null)
        {
            // 이미 동일한 무기가 슬롯에 있는 경우 슬롯 비우기
            slotBusyWithThisWeapon.SetWeaponSlot(null);
        }
        else
        {
            // 빈 슬롯에 무기 데이터 설정
            emptySlot = weaponSelectionUI.FindEmptySlot();
            emptySlot.SetWeaponSlot(weaponData);
        }

        emptySlot = null; // 빈 슬롯 참조 초기화
    }
}
