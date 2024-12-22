using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MissionObject_Key: �̼ǿ��� ���Ǵ� Ű ��ü Ŭ����
public class MissionObject_Key : MonoBehaviour
{
    private GameObject player; // �÷��̾� ��ü
    public static event Action OnKeyPickedUp; // Ű�� �ֿ��� �� ȣ��Ǵ� �̺�Ʈ

    private void Awake()
    {
        // "Player"��� �̸��� ���� ������Ʈ�� �˻�
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player object not found. Ensure a GameObject named 'Player' exists in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ʈ���ſ� ���� ��ü�� �÷��̾����� Ȯ��
        if (other.gameObject != player)
            return;

        // Ű�� �ֿ��� �� �̺�Ʈ ȣ��
        OnKeyPickedUp?.Invoke();

        // Ű ������Ʈ�� ����
        Destroy(gameObject);
    }
}
