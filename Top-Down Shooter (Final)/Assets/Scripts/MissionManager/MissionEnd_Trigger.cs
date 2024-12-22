using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionEnd_Trigger: 미션 완료를 트리거하는 클래스
public class MissionEnd_Trigger : MonoBehaviour
{
    private GameObject player; // 플레이어 오브젝트

    private void Start()
    {
        // "Player"라는 이름의 게임 오브젝트를 찾음
        player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found! Ensure the player GameObject is named 'Player'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거에 들어온 오브젝트가 플레이어가 아닌 경우 무시
        if (other.gameObject != player)
            return;

        // 미션이 완료되었는지 확인
        if (MissionManager.instance.MissionCompleted())
        {
            // 게임 완료 처리 및 메시지 출력
            GameManager.instance.GameCompleted();
            Debug.Log("Level completed!");
        }
        else
        {
            Debug.Log("Mission not completed yet.");
        }
    }
}
