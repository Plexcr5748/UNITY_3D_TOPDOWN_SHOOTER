using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Mission_EnemyHunt: ������ ���� �����ϴ� �̼� Ŭ����
[CreateAssetMenu(fileName = "Hunt - Mission", menuName = "Missions/Hunt - Mission")]
public class Mission_EnemyHunt : Mission
{
    public int amountToKill = 12; // �����ؾ� �� ���� ��
    public EnemyType enemyType; // Ÿ�� ���� ����

    private int killsToGo; // ���� ���� ��

    // �̼� ���� �޼���
    public override void StartMission()
    {
        killsToGo = amountToKill; // �ʱ�ȭ
        UpdateMissionUI(); // UI ������Ʈ

        MissionObject_HuntTarget.OnTargetKilled += EliminateTarget; // �� ���� �̺�Ʈ ���

        List<Enemy> validEnemies = new List<Enemy>();

        // �� ������ ���� Ÿ�� ����
        if (enemyType == EnemyType.Random)
        {
            validEnemies = LevelGenerator.instance.GetEnemyList();
        }
        else
        {
            foreach (Enemy enemy in LevelGenerator.instance.GetEnemyList())
            {
                if (enemy.enemyType == enemyType)
                {
                    validEnemies.Add(enemy);
                }
            }
        }

        // ������ ����ŭ ������ Ÿ�� ������Ʈ �߰�
        for (int i = 0; i < amountToKill; i++)
        {
            if (validEnemies.Count <= 0)
                return;

            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    // Ÿ�� ���� �� ȣ��Ǵ� �޼���
    private void EliminateTarget()
    {
        killsToGo--; // ���� �� �� ����
        UpdateMissionUI(); // UI ������Ʈ

        if (killsToGo <= 0)
        {
            // �̼� �Ϸ� �� �̼� ���� ������Ʈ �� �̺�Ʈ ����
            UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point.");
            MissionObject_HuntTarget.OnTargetKilled -= EliminateTarget;
        }
    }

    // �̼� UI ������Ʈ
    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + amountToKill + " enemies with signal disruptor.";
        string missionDetaiils = "Targets left: " + killsToGo;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetaiils);
    }
}
