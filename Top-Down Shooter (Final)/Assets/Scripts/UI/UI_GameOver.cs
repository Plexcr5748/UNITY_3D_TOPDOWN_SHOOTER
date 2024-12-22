using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// UI_GameOver: 게임 오버 화면의 UI를 관리하는 클래스
public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText; // 게임 오버 메시지를 표시하는 텍스트

    // 게임 오버 메시지를 설정하는 메서드
    public void ShowGameOverMessage(string message)
    {
        gameOverText.text = message; // 전달받은 메시지를 텍스트에 설정
    }
}
