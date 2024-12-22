using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_ComicPanel: 만화 패널 UI를 관리하는 클래스
public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    private Image myImage; // 현재 오브젝트의 이미지 컴포넌트

    [SerializeField] private Image[] comicPanel; // 만화 패널 배열
    [SerializeField] private GameObject buttonToEnable; // 만화가 끝난 후 활성화될 버튼

    private bool comicShowOver; // 만화 표시가 완료되었는지 여부
    private int imageIndex; // 현재 표시 중인 패널의 인덱스

    private void Start()
    {
        // 초기화 및 첫 번째 이미지 표시
        myImage = GetComponent<Image>();
        ShowNextImage();
    }

    protected void ShowNextImage()
    {
        // 다음 이미지를 표시
        if (comicShowOver)
            return;

        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, Action onComplete)
    {
        // 이미지의 알파 값을 부드럽게 변경
        float time = 0;
        Color currentColor = comicPanel[imageIndex].color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        imageIndex++;

        if (imageIndex >= comicPanel.Length)
        {
            FinishComicShow(); // 만화 표시 완료 처리
        }

        // 완료 메서드 호출 (있으면 실행)
        onComplete?.Invoke();
    }

    private void FinishComicShow()
    {
        // 만화 표시 완료 처리
        StopAllCoroutines(); // 모든 코루틴 정지
        comicShowOver = true; // 만화 완료 상태로 설정
        buttonToEnable.SetActive(true); // 버튼 활성화
        myImage.raycastTarget = false; // 클릭 대상 비활성화
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭 시 다음 이미지로 전환
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        // 클릭 시 다음 이미지 즉시 표시
        comicPanel[imageIndex].color = Color.white; // 현재 이미지를 완전히 표시
        imageIndex++;

        if (imageIndex >= comicPanel.Length)
        {
            FinishComicShow(); // 만화 표시 완료 처리
        }

        if (comicShowOver)
            return;

        ShowNextImage(); // 다음 이미지 표시
    }
}
