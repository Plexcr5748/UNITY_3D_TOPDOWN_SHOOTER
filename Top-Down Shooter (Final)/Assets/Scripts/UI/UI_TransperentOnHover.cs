using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_TransperentOnHover: ���콺 ȣ�� �� UI�� ������ȭ�ϴ� Ŭ����
public class UI_TransperentOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Dictionary<Image, Color> originalImageColors = new Dictionary<Image, Color>(); // Image ������Ʈ�� ���� ���� ����
    private Dictionary<TextMeshProUGUI, Color> originalTextColors = new Dictionary<TextMeshProUGUI, Color>(); // TextMeshProUGUI ������Ʈ�� ���� ���� ����

    private bool hasUIWeaponSlots; // UI�� ���� ������ �ִ��� ����
    private Player_WeaponController playerWeaponController; // �÷��̾� ���� ��Ʈ�ѷ� ����

    private void Start()
    {
        // UI_WeaponSlot�� ���ԵǾ� �ִ��� Ȯ��
        hasUIWeaponSlots = GetComponentInChildren<UI_WeaponSlot>();
        if (hasUIWeaponSlots)
            playerWeaponController = FindObjectOfType<Player_WeaponController>(); // ���� ��Ʈ�ѷ� ����

        // Image ������Ʈ�� ���� ���� ĳ��
        foreach (var image in GetComponentsInChildren<Image>(true))
        {
            originalImageColors[image] = image.color;
        }

        // TextMeshProUGUI ������Ʈ�� ���� ���� ĳ��
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            originalTextColors[text] = text.color;
        }
    }

    // ���콺�� UI ���� �ö���� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Image ������Ʈ�� ������ȭ
        foreach (var image in originalImageColors.Keys)
        {
            var color = image.color;
            color.a = .15f; // ���� ����
            image.color = color;
        }

        // TextMeshProUGUI ������Ʈ�� ������ȭ
        foreach (var text in originalTextColors.Keys)
        {
            var color = text.color;
            color.a = .15f; // ���� ����
            text.color = color;
        }
    }

    // ���콺�� UI���� ����� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        // Image ������Ʈ�� ���� ���� ����
        foreach (var image in originalImageColors.Keys)
        {
            image.color = originalImageColors[image];
        }

        // TextMeshProUGUI ������Ʈ�� ���� ���� ����
        foreach (var text in originalTextColors.Keys)
        {
            text.color = originalTextColors[text];
        }

        // ���� UI ������Ʈ (�ʿ��� ���)
        playerWeaponController?.UpdateWeaponUI();
    }
}
