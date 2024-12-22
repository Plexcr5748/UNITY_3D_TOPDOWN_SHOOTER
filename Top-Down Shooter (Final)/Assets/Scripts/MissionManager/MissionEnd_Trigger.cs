using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionEnd_Trigger: �̼� �ϷḦ Ʈ�����ϴ� Ŭ����
public class MissionEnd_Trigger : MonoBehaviour
{
    private GameObject player; // �÷��̾� ������Ʈ

    private void Start()
    {
        // "Player"��� �̸��� ���� ������Ʈ�� ã��
        player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("Player object not found! Ensure the player GameObject is named 'Player'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���ſ� ���� ������Ʈ�� �÷��̾ �ƴ� ��� ����
        if (other.gameObject != player)
            return;

        // �̼��� �Ϸ�Ǿ����� Ȯ��
        if (MissionManager.instance.MissionCompleted())
        {
            // ���� �Ϸ� ó�� �� �޽��� ���
            GameManager.instance.GameCompleted();
            Debug.Log("Level completed!");
        }
        else
        {
            Debug.Log("Mission not completed yet.");
        }
    }
}
