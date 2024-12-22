using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_Key: 미션에서 사용되는 키 객체 클래스
public class MissionObject_Key : MonoBehaviour
{
    private GameObject player; // 플레이어 객체
    public static event Action OnKeyPickedUp; // 키를 주웠을 때 호출되는 이벤트

    private void Awake()
    {
        // "Player"라는 이름의 게임 오브젝트를 검색
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player object not found. Ensure a GameObject named 'Player' exists in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거에 들어온 객체가 플레이어인지 확인
        if (other.gameObject != player)
            return;

        // 키를 주웠을 때 이벤트 호출
        OnKeyPickedUp?.Invoke();

        // 키 오브젝트를 삭제
        Destroy(gameObject);
    }
}
