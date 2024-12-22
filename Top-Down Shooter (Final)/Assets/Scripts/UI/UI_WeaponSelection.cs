using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

// UI_WeaponSelection: ���� ���� UI�� �����ϴ� Ŭ����
public class UI_WeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject nextUIToSwitchOn; // �������� ��ȯ�� UI
    public UI_SelectedWeaponWindow[] selectedWeapon; // ���õ� ���⸦ ǥ���ϴ� ���� �迭

    [Header("Warning Info")]
    [SerializeField] private TextMeshProUGUI warningText; // ��� �޽��� �ؽ�Ʈ
    [SerializeField] private float disaperaingSpeed = .25f; // ��� �޽��� ������� �ӵ�
    private float currentWarningAlpha; // ���� ��� �޽��� ����
    private float targetWarningAlpha; // ��ǥ ��� �޽��� ����

    private void Start()
    {
        // ���� �ʱ�ȭ
        selectedWeapon = GetComponentsInChildren<UI_SelectedWeaponWindow>();
    }

    private void Update()
    {
        // ��� �޽��� ���� ������Ʈ
        if (currentWarningAlpha > targetWarningAlpha)
        {
            currentWarningAlpha -= Time.deltaTime * disaperaingSpeed;
            warningText.color = new Color(1, 1, 1, currentWarningAlpha);
        }
    }

    // ���� ���� Ȯ��
    public void ConfirmWeaponSelection()
    {
        if (AtLeastOneWeaponSelected())
        {
            UI.instance.SwitchTo(nextUIToSwitchOn); // ���� UI�� ��ȯ
            UI.instance.StartLevelGeneration(); // ���� ���� ����
        }
        else
        {
            ShowWarningMessage("Select at least one weapon."); // ��� �޽��� ǥ��
        }
    }

    // �ּ� �ϳ� �̻��� ���Ⱑ ���õǾ����� Ȯ��
    private bool AtLeastOneWeaponSelected() => SelectedWeaponData().Count > 0;

    // ���õ� ������ �����͸� ��ȯ
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

    // �� ���� ã��
    public UI_SelectedWeaponWindow FindEmptySlot()
    {
        foreach (UI_SelectedWeaponWindow slot in selectedWeapon)
        {
            if (slot.IsEmpty())
                return slot;
        }

        return null; // �� ������ ���� ��� null ��ȯ
    }

    // Ư�� Ÿ���� ���⸦ ���� ���� ã��
    public UI_SelectedWeaponWindow FindSlowWithWeaponOfType(Weapon_Data weaponData)
    {
        foreach (UI_SelectedWeaponWindow slot in selectedWeapon)
        {
            if (slot.weaponData == weaponData)
                return slot;
        }

        return null; // �ش� ���⸦ ���� ������ ���� ��� null ��ȯ
    }

    // ��� �޽��� ǥ��
    public void ShowWarningMessage(string message)
    {
        warningText.color = Color.white; // �ؽ�Ʈ�� ���̰� ����
        warningText.text = message; // �޽��� ������Ʈ

        currentWarningAlpha = warningText.color.a;
        targetWarningAlpha = 0; // �޽����� ������ ������� ����
    }
}
