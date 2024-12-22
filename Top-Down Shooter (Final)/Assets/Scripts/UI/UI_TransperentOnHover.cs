using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_TransperentOnHover: 마우스 호버 시 UI를 반투명화하는 클래스
public class UI_TransperentOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>(); // Image 컴포넌트의 원래 색상 저장
    private Dictionary<TextMeshProUGUI, Color> originalTextColors = new Dictionary<TextMeshProUGUI, Color>(); // TextMeshProUGUI 컴포넌트의 원래 색상 저장

    private bool hasUIWeaponSlots; // UI에 무기 슬롯이 있는지 여부
    private Player_WeaponController playerWeaponController; // 플레이어 무기 컨트롤러 참조

    private void Start()
    {
        // UI_WeaponSlot이 포함되어 있는지 확인
        hasUIWeaponSlots = GetComponentInChildren<UI_WeaponSlot>();
        if (hasUIWeaponSlots)
            playerWeaponController = FindObjectOfType<Player_WeaponController>(); // 무기 컨트롤러 참조

        // Image 컴포넌트와 원래 색상 캐싱
        foreach (var image in GetComponentsInChildren<Image>(true))
        {
            originalImageColors[image] = image.color;
        }

        // TextMeshProUGUI 컴포넌트와 원래 색상 캐싱
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            originalTextColors[text] = text.color;
        }
    }

    // 마우스가 UI 위에 올라왔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Image 컴포넌트를 반투명화
        foreach (var image in originalImageColors.Keys)
        {
            var color = image.color;
            color.a = .15f; // 투명도 설정
            image.color = color;
        }

        // TextMeshProUGUI 컴포넌트를 반투명화
        foreach (var text in originalTextColors.Keys)
        {
            var color = text.color;
            color.a = .15f; // 투명도 설정
            text.color = color;
        }
    }

    // 마우스가 UI에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // Image 컴포넌트의 원래 색상 복원
        foreach (var image in originalImageColors.Keys)
        {
            image.color = originalImageColors[image];
        }

        // TextMeshProUGUI 컴포넌트의 원래 색상 복원
        foreach (var text in originalTextColors.Keys)
        {
            text.color = originalTextColors[text];
        }

        // 무기 UI 업데이트 (필요한 경우)
        playerWeaponController?.UpdateWeaponUI();
    }
}
