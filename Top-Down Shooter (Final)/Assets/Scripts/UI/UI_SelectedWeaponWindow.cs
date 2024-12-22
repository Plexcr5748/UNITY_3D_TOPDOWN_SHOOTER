using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_SelectedWeaponWindow: ���õ� ������ ������ ǥ���ϴ� UI ���� Ŭ����
public class UI_SelectedWeaponWindow : MonoBehaviour
{
    public Weapon_Data weaponData; // ���� ���õ� ���� ������

    [SerializeField] private Image weaponIcon; // ���� ������ �̹���
    [SerializeField] private TextMeshProUGUI weaponInfo; // ���� ���� �ؽ�Ʈ

    private void Start()
    {
        // �ʱ�ȭ: ���õ� ���Ⱑ ���� ���·� ����
        weaponData = null;
        UpdateSlotInfo(null);
    }

    // ���ο� ���� �����͸� ����
    public void SetWeaponSlot(Weapon_Data newWeaponData)
    {
        weaponData = newWeaponData;
        UpdateSlotInfo(newWeaponData); // ���� UI ������Ʈ
    }

    // ���� ���� ������Ʈ
    public void UpdateSlotInfo(Weapon_Data weaponData)
    {
        if (weaponData == null)
        {
            // ���� �����Ͱ� ���� ��� �⺻ ���·� ����
            weaponIcon.color = Color.clear; // ������ ����
            weaponInfo.text = "Select a weapon..."; // �⺻ �޽���
            return;
        }

        // ���� �����Ͱ� ���� ��� UI ������Ʈ
        weaponIcon.color = Color.white; // ������ ǥ��
        weaponIcon.sprite = weaponData.weaponIcon; // ���� ������ ����
        weaponInfo.text = weaponData.weaponInfo; // ���� ���� �ؽ�Ʈ ����
    }

    // ������ ��� �ִ��� Ȯ��
    public bool IsEmpty() => weaponData == null;
}
