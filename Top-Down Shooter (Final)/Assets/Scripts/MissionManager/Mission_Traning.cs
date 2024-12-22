using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_Training: �Ʒ� �̼��� ����ϴ� Ŭ����
[CreateAssetMenu(fileName = "New Training Mission", menuName = "Missions/Training - Mission")]
public class Mission_Traning : Mission
{
    // �÷��̾ ������ ��ġ�� ����
    public Vector3 warpPosition;

    // �̼� ���� �� ȣ��
    public override void StartMission()
    {
        // �÷��̾ ������ ��ġ�� ����
        WarpPlayerToPosition(warpPosition);
    }

    // �̼� ������Ʈ �� ȣ��
    public override void UpdateMission()
    {
        // �Ʒ� �̼� �ؽ�Ʈ�� ��Ʈ�� ������ UI�� ������Ʈ
        string missionText = "This is the training room.";
        string missionDetails =
            "Control \r\n" +
            "Move : W/A/S/D \r\n" +
            "Shoot : Left Click \r\n" +
            "Precision Aim : Right Click \r\n" +
            "Running : Shift \r\n" +
            "Use Item/Interaction Car : F \r\n" +
            "Pause : ESC \r\n" +
            "If you want to create an item or enemy, shoot a model.";

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        // ESC Ű�� ������ �̼� �Ϸ�� ����
        return Input.GetKeyDown(KeyCode.Escape);
    }

    // �÷��̾ ������ ��ġ�� ������Ű�� �Լ�
    public void WarpPlayerToPosition(Vector3 targetPosition)
    {
        // "Player" �±׸� ���� ���� ������Ʈ�� �˻�
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // �÷��̾� ��ġ�� ������ ��ġ�� �̵�
            player.transform.position = targetPosition;
            Debug.Log("Player warped to: " + targetPosition);

            // �÷��̾��� ü���� �ִ�ġ�� ����
            Player_Health playerHealth = player.GetComponent<Player_Health>();
            if (playerHealth != null)
            {
                playerHealth.maxHealth = 100000;
                playerHealth.currentHealth = 100000;
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with the 'Player' tag found.");
        }
    }
}
