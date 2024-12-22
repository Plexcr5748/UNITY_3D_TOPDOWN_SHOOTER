using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_ComicPanel: ��ȭ �г� UI�� �����ϴ� Ŭ����
public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    private Image myImage; // ���� ������Ʈ�� �̹��� ������Ʈ

    [SerializeField] private Image[] comicPanel; // ��ȭ �г� �迭
    [SerializeField] private GameObject buttonToEnable; // ��ȭ�� ���� �� Ȱ��ȭ�� ��ư

    private bool comicShowOver; // ��ȭ ǥ�ð� �Ϸ�Ǿ����� ����
    private int imageIndex; // ���� ǥ�� ���� �г��� �ε���

    private void Start()
    {
        // �ʱ�ȭ �� ù ��° �̹��� ǥ��
        myImage = GetComponent<Image>();
        ShowNextImage();
    }

    protected void ShowNextImage()
    {
        // ���� �̹����� ǥ��
        if (comicShowOver)
            return;

        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, Action onComplete)
    {
        // �̹����� ���� ���� �ε巴�� ����
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
            FinishComicShow(); // ��ȭ ǥ�� �Ϸ� ó��
        }

        // �Ϸ� �޼��� ȣ�� (������ ����)
        onComplete?.Invoke();
    }

    private void FinishComicShow()
    {
        // ��ȭ ǥ�� �Ϸ� ó��
        StopAllCoroutines(); // ��� �ڷ�ƾ ����
        comicShowOver = true; // ��ȭ �Ϸ� ���·� ����
        buttonToEnable.SetActive(true); // ��ư Ȱ��ȭ
        myImage.raycastTarget = false; // Ŭ�� ��� ��Ȱ��ȭ
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Ŭ�� �� ���� �̹����� ��ȯ
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        // Ŭ�� �� ���� �̹��� ��� ǥ��
        comicPanel[imageIndex].color = Color.white; // ���� �̹����� ������ ǥ��
        imageIndex++;

        if (imageIndex >= comicPanel.Length)
        {
            FinishComicShow(); // ��ȭ ǥ�� �Ϸ� ó��
        }

        if (comicShowOver)
            return;

        ShowNextImage(); // ���� �̹��� ǥ��
    }
}
