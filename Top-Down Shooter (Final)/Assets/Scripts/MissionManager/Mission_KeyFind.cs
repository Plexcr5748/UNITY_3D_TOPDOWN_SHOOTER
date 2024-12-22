using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_KeyFind: Ű�� ã�� �̼� Ŭ����
[CreateAssetMenu(fileName = "Find Key - Mission", menuName = "Missions/Key - Mission")]
public class Mission_KeyFind : Mission
{
    [SerializeField] private GameObject key; // �̼ǿ��� ����� Ű ������Ʈ
    private bool keyFound; // Ű �߰� ����

    // �̼� ���� �޼���
    public override void StartMission()
    {
        // Ű�� �ֿ��� �� ȣ��Ǵ� �̺�Ʈ ���
        MissionObject_Key.OnKeyPickedUp += PickUpKey;

        // �̼� UI ������Ʈ
        UI.instance.inGameUI.UpdateMissionInfo("Find a key-holder. Retrieve the key.");

        // ������ ���� �����Ͽ� Ű�� �����ϰ� ����
        Enemy enemy = LevelGenerator.instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key); // ������ Ű �ο�
        enemy.MakeEnemyVIP(); // ���� VIP(Ư�� ���)�� ����
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        return keyFound; // Ű�� �߰��ߴ��� ��ȯ
    }

    // Ű�� �ֿ��� �� ȣ��Ǵ� �޼���
    private void PickUpKey()
    {
        keyFound = true; // Ű �߰� ���� ������Ʈ

        // �̺�Ʈ ����
        MissionObject_Key.OnKeyPickedUp -= PickUpKey;

        // �̼� UI ������Ʈ
        UI.instance.inGameUI.UpdateMissionInfo("You've got the key! \n Get to the evacuation point.");
    }
}
