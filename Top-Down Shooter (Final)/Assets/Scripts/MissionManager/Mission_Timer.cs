using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_Timer: ���� �ð� ���� �Ϸ��ؾ� �ϴ� �̼� Ŭ����
[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Missions/Timer - Mission")]
public class Mission_Timer : Mission
{
    public float time = 300; // ���� �ð� (�� ����)
    private float currentTime; // ���� �ð�
    private bool missionStart = false; // �̼� ���� ����

    // �̼� ���� �޼���
    public override void StartMission()
    {
        currentTime = time; // ���� �ð��� �ʱ�ȭ
        missionStart = true; // �̼� ���� ���� ����
    }

    // �̼� ������Ʈ �޼���
    public override void UpdateMission()
    {
        if (!missionStart)
            return; // �̼��� ���۵��� �ʾ����� �ƹ� �۾��� ���� ����

        currentTime -= Time.deltaTime; // �� �����Ӹ��� ���� �ð� ����

        if (currentTime <= 0)
        {
            missionStart = false; // �̼� ����
            GameManager.instance.GameOver(); // ���� ���� ó��

            // �÷��̾� ü���� 0���� ����
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player_Health playerHealth = player.GetComponent<Player_Health>();
                if (playerHealth != null)
                {
                    playerHealth.Die();
                }
            }
            else
            {
                Debug.LogWarning("No GameObject with the 'Player' tag found.");
            }
        }

        // ���� �ð��� UI�� ǥ��
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
        string missionText = "Get to evacuation point before plane take off.";
        string missionDetails = "Time left: " + timeText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        return currentTime > 0 && missionStart; // �̼��� ���۵Ǿ��� �ð��� ���� ������ �Ϸ���� ���� ����
    }

    // �̼� �ʱ�ȭ �޼���
    public void ResetMission()
    {
        missionStart = false; // �̼� ���� ���� �ʱ�ȭ
        currentTime = time; // �ð��� �ٽ� �ʱ�ȭ
    }
}
