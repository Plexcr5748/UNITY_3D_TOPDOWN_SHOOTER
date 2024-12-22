using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditorInternal.Profiling.Memory.Experimental;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

// ���� ������ ����� �����ϴ� Ŭ����
public class Enemy_DropController : MonoBehaviour
{
    [SerializeField] private GameObject missionObjectKey; // �̼� Ű ������
    [SerializeField] private GameObject Heal; // ü�� ȸ�� ������
    [SerializeField] private GameObject Ammo; // ź�� ������
    private MissionManager missionManager; // �̼� ���� �Ŵ���
    private Enemy Enemy; // �� ��ü ����

    // �ʱ�ȭ
    private void Start()
    {
        Enemy = GetComponent<Enemy>();
        missionManager = MissionManager.instance;
    }

    // ���ο� �̼� Ű�� ����
    public void GiveKey(GameObject newKey) => missionObjectKey = newKey;

    // ������ ��� ó��
    public void DropItems()
    {
        // Mission_KeyFind �̼� ���� ���� ��� Ű ���
        if (missionManager.currentMission is Mission_KeyFind keyFindMission && missionObjectKey != null)
        {
            CreateItem(missionObjectKey); // Ű ������ ����
            missionObjectKey = null; // ��� �� Ű ������ �ʱ�ȭ
            return; // Ű ������ ��� �� �ٸ� �������� ������� ����
        }

        // ���Ÿ� ���� ���
        if (Enemy.enemyType == EnemyType.Range)
        {
            int randomNumber = Random.Range(1, 101); // 1 ~ 100 ���� ���� �� ����
            if (randomNumber <= 30) // 30 ������ ��� ź�� ���
            {
                CreateItem(Ammo);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true; // �浹 ���� Ʈ���� Ȱ��ȭ
            }
            else if (30 < randomNumber && randomNumber <= 60) // 31 ~ 60 ������ ��� ��Ŷ ���
            {
                CreateItem(Heal);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true;
            }
            return;
        }
        // ���� ���� ���
        else if (Enemy.enemyType == EnemyType.Melee)
        {
            int randomNumber = Random.Range(1, 101); // 1 ~ 100 ���� ���� �� ����
            if (randomNumber <= 50) // 50 ������ ��� ��Ŷ ���
            {
                CreateItem(Heal);
                BoxCollider rb = Ammo.GetComponent<BoxCollider>();
                rb.isTrigger = true;
            }
            return;
        }
    }

    // ������ ����
    private void CreateItem(GameObject go)
    {
        GameObject newItem = Instantiate(go, transform.position + Vector3.up, Quaternion.identity); // ������ ����
    }

    // ź�� ������ ���� �� �θ𿡼� �и�
    private void CreateAmmo(GameObject go)
    {
        GameObject newAmmo = Instantiate(go, transform.position, Quaternion.identity);

        foreach (Transform child in newAmmo.transform)
        {
            child.SetParent(null); // �ڽ� ������Ʈ�� �θ𿡼� �и�
        }
    }
}
