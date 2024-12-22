using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UI_GameOver: ���� ���� ȭ���� UI�� �����ϴ� Ŭ����
public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText; // ���� ���� �޽����� ǥ���ϴ� �ؽ�Ʈ

    // ���� ���� �޽����� �����ϴ� �޼���
    public void ShowGameOverMessage(string message)
    {
        gameOverText.text = message; // ���޹��� �޽����� �ؽ�Ʈ�� ����
    }
}
