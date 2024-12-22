using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_WeaponSlot: ���� ���� UI�� �����ϴ� Ŭ����
public class UI_WeaponSlot : MonoBehaviour
{
    public Image weaponIcon; // ���� ������ �̹���
    public TextMeshProUGUI ammoText; // ź�� ������ ǥ���ϴ� �ؽ�Ʈ

    private void Awake()
    {
        // ���� �����ܰ� ź�� �ؽ�Ʈ �ʱ�ȭ
        weaponIcon = GetComponentInChildren<Image>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // ���� ���� UI ������Ʈ �޼���
    public void UpdateWeaponSlot(Weapon myWeapon, bool activeWeapon)
    {
        if (myWeapon == null)
        {
            // ���Ⱑ ���� ��� ������ �ʱ�ȭ
            weaponIcon.color = Color.clear; // ������ �����
            ammoText.text = ""; // �ؽ�Ʈ ����
            return;
        }

        // ���� ���Ⱑ Ȱ�� ���������� ���� ���� ����
        Color newColor = activeWeapon ? Color.white : new Color(1, 1, 1, .35f);

        weaponIcon.color = newColor; // ���� ������ ���� ����
        weaponIcon.sprite = myWeapon.weaponData.weaponIcon; // ���� ������ �̹��� ����

        // ź�� ���� �ؽ�Ʈ ������Ʈ
        ammoText.text = myWeapon.bulletsInMagazine + "/" + myWeapon.totalReserveAmmo;
        ammoText.color = Color.white; // �ؽ�Ʈ ���� ����
    }
}
