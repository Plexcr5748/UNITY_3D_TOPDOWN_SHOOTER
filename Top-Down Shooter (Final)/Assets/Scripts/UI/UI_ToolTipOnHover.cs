using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI_ToolTipOnHover: 마우스 호버 시 툴팁을 표시하는 UI 컴포넌트
public class UI_ToolTipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject tooltip; // 툴팁 오브젝트

    [Header("Audio")]
    [SerializeField] private AudioSource pointerEnterSFX; // 마우스 오버 사운드
    [SerializeField] private AudioSource pointerDownSFX; // 클릭 사운드

    // 마우스 클릭 시 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerDownSFX != null)
            pointerDownSFX.Play(); // 클릭 사운드 재생
    }

    // 마우스가 UI 위로 올라왔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(true); // 툴팁 활성화

        if (pointerEnterSFX != null)
            pointerEnterSFX.Play(); // 마우스 오버 사운드 재생
    }

    // 마우스가 UI를 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(false); // 툴팁 비활성화
    }
}
