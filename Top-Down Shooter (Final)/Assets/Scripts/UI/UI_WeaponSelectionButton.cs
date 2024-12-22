using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_WeaponSelectionButton: ���� ���� ��ư UI�� �����ϴ� Ŭ����
public class UI_WeaponSelectionButton : UI_Button
{
    private UI_WeaponSelection weaponSelectionUI; // ���� ���� UI ����

    [SerializeField] private Weapon_Data weaponData; // ��ư�� ����� ���� ������
    [SerializeField] private Image weaponIcon; // ��ư�� ǥ�õ� ���� ������

    private UI_SelectedWeaponWindow emptySlot; // �� ���� ����

    private void OnValidate()
    {
        // �����Ϳ��� ��ư �̸� �ڵ� ����
        gameObject.name = "Button - Select Weapon: " + weaponData.weaponType;
    }

    public override void Start()
    {
        // �⺻ �ʱ�ȭ �� ���� ���� UI ���� ����
        base.Start();

        weaponSelectionUI = GetComponentInParent<UI_WeaponSelection>();
        weaponIcon.sprite = weaponData.weaponIcon; // ���� ������ ����
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺�� ��ư ���� �ö���� ��
        base.OnPointerEnter(eventData);
        weaponIcon.color = Color.yellow; // ������ ���� ����

        emptySlot = weaponSelectionUI.FindEmptySlot(); // �� ���� �˻�
        emptySlot?.UpdateSlotInfo(weaponData); // �� ������ ������ ���� ������ ������Ʈ
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� ��ư���� ����� ��
        base.OnPointerExit(eventData);
        weaponIcon.color = Color.white; // ������ ���� ����

        emptySlot?.UpdateSlotInfo(null); // �� ���� ���� �ʱ�ȭ
        emptySlot = null; // �� ���� ���� �ʱ�ȭ
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // ��ư�� Ŭ���Ǿ��� ��
        base.OnPointerDown(eventData);
        weaponIcon.color = Color.white; // ������ ���� ����

        // ���� ���� Ȯ��
        bool noMoreEmptySlots = weaponSelectionUI.FindEmptySlot() == null; // �� ���� ����
        bool noThisWeaponInSlots = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData) == null; // ������ ���Ⱑ ���Կ� ����

        if (noMoreEmptySlots && noThisWeaponInSlots)
        {
            // �� ������ ���� ������ ���Ⱑ ���õ��� ���� ��� ��� �޽��� ǥ��
            weaponSelectionUI.ShowWarningMessage("No Empty Slots...");
            return;
        }

        UI_SelectedWeaponWindow slotBusyWithThisWeapon = weaponSelectionUI.FindSlowWithWeaponOfType(weaponData);

        if (slotBusyWithThisWeapon != null)
        {
            // �̹� ������ ���Ⱑ ���Կ� �ִ� ��� ���� ����
            slotBusyWithThisWeapon.SetWeaponSlot(null);
        }
        else
        {
            // �� ���Կ� ���� ������ ����
            emptySlot = weaponSelectionUI.FindEmptySlot();
            emptySlot.SetWeaponSlot(weaponData);
        }

        emptySlot = null; // �� ���� ���� �ʱ�ȭ
    }
}
