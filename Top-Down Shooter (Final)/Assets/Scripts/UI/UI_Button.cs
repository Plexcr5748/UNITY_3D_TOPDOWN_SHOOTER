using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_Button: ��ư�� ���� �ð��� ȿ�� �� ���ͷ��� ����
public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Mouse hover settings")]
    public float scaleSpeed = 3; // ��ư ũ�� ��ȯ �ӵ�
    public float scaleRate = 1.2f; // ��ư Ȯ�� ����

    private Vector3 defaultScale; // ��ư�� �⺻ ũ��
    private Vector3 targetScale; // ��ư�� ��ǥ ũ��

    private Image buttonImage; // ��ư �̹���
    private TextMeshProUGUI buttonText; // ��ư �ؽ�Ʈ

    [Header("Audio")]
    [SerializeField] private AudioSource pointerEnterSFX; // ���콺 ���� ����
    [SerializeField] private AudioSource pointerDownSFX; // ��ư Ŭ�� ����

    public virtual void Start()
    {
        // ��ư �ʱ�ȭ
        defaultScale = transform.localScale; // �⺻ ũ�� ����
        targetScale = defaultScale; // ��ǥ ũ�⸦ �⺻ ũ��� ����

        buttonImage = GetComponent<Button>().image; // ��ư �̹��� ��������
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); // ��ư �ؽ�Ʈ ��������
    }

    public virtual void Update()
    {
        // ��ư ũ�� �ִϸ��̼� ó��
        if (Mathf.Abs(transform.lossyScale.x - targetScale.x) > .01f)
        {
            float scaleValue = Mathf.Lerp(transform.localScale.x, targetScale.x, Time.deltaTime * scaleSpeed);
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // ���콺 ���� �� ��ư Ȯ�� �� ���� ����
        targetScale = defaultScale * scaleRate;

        if (pointerEnterSFX != null)
            pointerEnterSFX.Play(); // ���콺 ���� ���� ���

        if (buttonImage != null)
            buttonImage.color = Color.yellow; // ��ư �̹��� ���� ����

        if (buttonText != null)
            buttonText.color = Color.yellow; // ��ư �ؽ�Ʈ ���� ����
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // ���콺�� ��ư���� ����� �� ���� ���·� ����
        ReturnDefaultLook();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // ��ư Ŭ�� �� ���� ���·� ���� �� Ŭ�� ���� ���
        ReturnDefaultLook();

        if (pointerDownSFX != null)
            pointerDownSFX.Play(); // Ŭ�� ���� ���
    }

    private void ReturnDefaultLook()
    {
        // ��ư�� �⺻ ���·� ����
        targetScale = defaultScale;

        if (buttonImage != null)
            buttonImage.color = Color.white; // �⺻ �̹��� ����

        if (buttonText != null)
            buttonText.color = Color.white; // �⺻ �ؽ�Ʈ ����
    }

    public void AssignAudioSource()
    {
        // ��ư�� ���� �ҽ��� �������� �Ҵ�
        pointerEnterSFX = GameObject.Find("UI_PointerEnter").GetComponent<AudioSource>();
        pointerDownSFX = GameObject.Find("UI_PointerDown").GetComponent<AudioSource>();
    }
}
