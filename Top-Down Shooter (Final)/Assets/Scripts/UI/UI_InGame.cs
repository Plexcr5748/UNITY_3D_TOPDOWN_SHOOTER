using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_InGame: ���� �� UI�� �����ϴ� Ŭ����
public class UI_InGame : MonoBehaviour
{
    [SerializeField] private GameObject charcaterUI; // ĳ���� ���� UI
    [SerializeField] private GameObject carUI; // ���� ���� UI

    [Header("Health")]
    [SerializeField] private Image healthBar; // ĳ���� ü�� ��

    [Header("Weapons")]
    [SerializeField] private UI_WeaponSlot[] weaponSlots_UI; // ���� ���� UI �迭

    [Header("Missions")]
    [SerializeField] private GameObject missionTooltipParent; // �̼� ���� �θ� ��ü
    [SerializeField] private GameObject missionHelpTooltip; // �̼� ���� ����
    [SerializeField] private TextMeshProUGUI missionText; // �̼� ���� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI missionDetails; // �̼� ���λ��� �ؽ�Ʈ
    private bool tooltipActive = true; // ���� Ȱ��ȭ ����

    [Header("Car info")]
    [SerializeField] private Image carHealthBar; // ���� ü�� ��
    [SerializeField] private TextMeshProUGUI carSpeedText; // ���� �ӵ� �ؽ�Ʈ

    private void Awake()
    {
        // ���� ���� UI �ʱ�ȭ
        weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
    }

    // ĳ���� UI�� ��ȯ
    public void SwitchToCharcaterUI()
    {
        charcaterUI.SetActive(true);
        carUI.SetActive(false);
    }

    // ���� UI�� ��ȯ
    public void SwitchToCarUI()
    {
        charcaterUI.SetActive(false);
        carUI.SetActive(true);
    }

    // �̼� ���� Ȱ��ȭ/��Ȱ��ȭ ��ȯ
    public void SwitchMissionTooltip()
    {
        tooltipActive = !tooltipActive;
        missionTooltipParent.SetActive(tooltipActive);
        missionHelpTooltip.SetActive(!tooltipActive);
    }

    // �̼� ���� ������Ʈ
    public void UpdateMissionInfo(string missionText, string missionDetails = "")
    {
        this.missionText.text = missionText;
        this.missionDetails.text = missionDetails;
    }

    // ���� UI ������Ʈ
    public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon currentWeapon)
    {
        for (int i = 0; i < weaponSlots_UI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                // ���� �������� ���� Ȯ��
                bool isActiveWeapon = weaponSlots[i] == currentWeapon ? true : false;
                weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isActiveWeapon);
            }
            else
            {
                // ��� �ִ� ���� ó��
                weaponSlots_UI[i].UpdateWeaponSlot(null, false);
            }
        }
    }

    // ĳ���� ü�� UI ������Ʈ
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    // ���� ü�� UI ������Ʈ
    public void UpdateCarHealthUI(float currentCarHealth, float maxCarHealth)
    {
        carHealthBar.fillAmount = currentCarHealth / maxCarHealth;
    }

    // ���� �ӵ� �ؽ�Ʈ ������Ʈ
    public void UpdateSpeedText(string text) => carSpeedText.text = text;
}
