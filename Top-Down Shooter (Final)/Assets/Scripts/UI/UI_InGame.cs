using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI_InGame: 게임 중 UI를 관리하는 클래스
public class UI_InGame : MonoBehaviour
{
    [SerializeField] private GameObject charcaterUI; // 캐릭터 관련 UI
    [SerializeField] private GameObject carUI; // 차량 관련 UI

    [Header("Health")]
    [SerializeField] private Image healthBar; // 캐릭터 체력 바

    [Header("Weapons")]
    [SerializeField] private UI_WeaponSlot[] weaponSlots_UI; // 무기 슬롯 UI 배열

    [Header("Missions")]
    [SerializeField] private GameObject missionTooltipParent; // 미션 툴팁 부모 객체
    [SerializeField] private GameObject missionHelpTooltip; // 미션 도움말 툴팁
    [SerializeField] private TextMeshProUGUI missionText; // 미션 제목 텍스트
    [SerializeField] private TextMeshProUGUI missionDetails; // 미션 세부사항 텍스트
    private bool tooltipActive = true; // 툴팁 활성화 여부

    [Header("Car info")]
    [SerializeField] private Image carHealthBar; // 차량 체력 바
    [SerializeField] private TextMeshProUGUI carSpeedText; // 차량 속도 텍스트

    private void Awake()
    {
        // 무기 슬롯 UI 초기화
        weaponSlots_UI = GetComponentsInChildren<UI_WeaponSlot>();
    }

    // 캐릭터 UI로 전환
    public void SwitchToCharcaterUI()
    {
        charcaterUI.SetActive(true);
        carUI.SetActive(false);
    }

    // 차량 UI로 전환
    public void SwitchToCarUI()
    {
        charcaterUI.SetActive(false);
        carUI.SetActive(true);
    }

    // 미션 툴팁 활성화/비활성화 전환
    public void SwitchMissionTooltip()
    {
        tooltipActive = !tooltipActive;
        missionTooltipParent.SetActive(tooltipActive);
        missionHelpTooltip.SetActive(!tooltipActive);
    }

    // 미션 정보 업데이트
    public void UpdateMissionInfo(string missionText, string missionDetails = "")
    {
        this.missionText.text = missionText;
        this.missionDetails.text = missionDetails;
    }

    // 무기 UI 업데이트
    public void UpdateWeaponUI(List<Weapon> weaponSlots, Weapon currentWeapon)
    {
        for (int i = 0; i < weaponSlots_UI.Length; i++)
        {
            if (i < weaponSlots.Count)
            {
                // 현재 무기인지 여부 확인
                bool isActiveWeapon = weaponSlots[i] == currentWeapon ? true : false;
                weaponSlots_UI[i].UpdateWeaponSlot(weaponSlots[i], isActiveWeapon);
            }
            else
            {
                // 비어 있는 슬롯 처리
                weaponSlots_UI[i].UpdateWeaponSlot(null, false);
            }
        }
    }

    // 캐릭터 체력 UI 업데이트
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    // 차량 체력 UI 업데이트
    public void UpdateCarHealthUI(float currentCarHealth, float maxCarHealth)
    {
        carHealthBar.fillAmount = currentCarHealth / maxCarHealth;
    }

    // 차량 속도 텍스트 업데이트
    public void UpdateSpeedText(string text) => carSpeedText.text = text;
}
