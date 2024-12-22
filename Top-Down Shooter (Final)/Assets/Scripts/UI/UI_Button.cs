using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_Button: 버튼에 대한 시각적 효과 및 인터랙션 관리
public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Mouse hover settings")]
    public float scaleSpeed = 3; // 버튼 크기 전환 속도
    public float scaleRate = 1.2f; // 버튼 확대 비율

    private Vector3 defaultScale; // 버튼의 기본 크기
    private Vector3 targetScale; // 버튼의 목표 크기

    private Image buttonImage; // 버튼 이미지
    private TextMeshProUGUI buttonText; // 버튼 텍스트

    [Header("Audio")]
    [SerializeField] private AudioSource pointerEnterSFX; // 마우스 오버 사운드
    [SerializeField] private AudioSource pointerDownSFX; // 버튼 클릭 사운드

    public virtual void Start()
    {
        // 버튼 초기화
        defaultScale = transform.localScale; // 기본 크기 저장
        targetScale = defaultScale; // 목표 크기를 기본 크기로 설정

        buttonImage = GetComponent<Button>().image; // 버튼 이미지 가져오기
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); // 버튼 텍스트 가져오기
    }

    public virtual void Update()
    {
        // 버튼 크기 애니메이션 처리
        if (Mathf.Abs(transform.lossyScale.x - targetScale.x) > .01f)
        {
            float scaleValue = Mathf.Lerp(transform.localScale.x, targetScale.x, Time.deltaTime * scaleSpeed);
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 오버 시 버튼 확대 및 색상 변경
        targetScale = defaultScale * scaleRate;

        if (pointerEnterSFX != null)
            pointerEnterSFX.Play(); // 마우스 오버 사운드 재생

        if (buttonImage != null)
            buttonImage.color = Color.yellow; // 버튼 이미지 색상 변경

        if (buttonText != null)
            buttonText.color = Color.yellow; // 버튼 텍스트 색상 변경
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 버튼에서 벗어났을 때 원래 상태로 복구
        ReturnDefaultLook();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 클릭 시 원래 상태로 복구 및 클릭 사운드 재생
        ReturnDefaultLook();

        if (pointerDownSFX != null)
            pointerDownSFX.Play(); // 클릭 사운드 재생
    }

    private void ReturnDefaultLook()
    {
        // 버튼을 기본 상태로 복구
        targetScale = defaultScale;

        if (buttonImage != null)
            buttonImage.color = Color.white; // 기본 이미지 색상

        if (buttonText != null)
            buttonText.color = Color.white; // 기본 텍스트 색상
    }

    public void AssignAudioSource()
    {
        // 버튼에 사운드 소스를 동적으로 할당
        pointerEnterSFX = GameObject.Find("UI_PointerEnter").GetComponent<AudioSource>();
        pointerDownSFX = GameObject.Find("UI_PointerDown").GetComponent<AudioSource>();
    }
}
