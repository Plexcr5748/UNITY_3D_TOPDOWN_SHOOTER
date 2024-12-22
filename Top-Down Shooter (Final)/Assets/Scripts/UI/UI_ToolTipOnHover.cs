using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI_ToolTipOnHover: ���콺 ȣ�� �� ������ ǥ���ϴ� UI ������Ʈ
public class UI_ToolTipOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private GameObject tooltip; // ���� ������Ʈ

    [Header("Audio")]
    [SerializeField] private AudioSource pointerEnterSFX; // ���콺 ���� ����
    [SerializeField] private AudioSource pointerDownSFX; // Ŭ�� ����

    // ���콺 Ŭ�� �� ȣ��
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pointerDownSFX != null)
            pointerDownSFX.Play(); // Ŭ�� ���� ���
    }

    // ���콺�� UI ���� �ö���� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(true); // ���� Ȱ��ȭ

        if (pointerEnterSFX != null)
            pointerEnterSFX.Play(); // ���콺 ���� ���� ���
    }

    // ���콺�� UI�� ����� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
            tooltip.SetActive(false); // ���� ��Ȱ��ȭ
    }
}
