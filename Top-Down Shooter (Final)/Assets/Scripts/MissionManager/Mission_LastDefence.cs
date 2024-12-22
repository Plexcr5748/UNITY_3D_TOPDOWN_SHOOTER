using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_LastDefence: ��� �̼� Ŭ����
[CreateAssetMenu(fileName = "Defence - Mission", menuName = "Missions/Defence - Mission")]
public class Mission_LastDefence : Mission
{
    public bool defenceBegun = false; // ��� �̺�Ʈ ���� ����

    [Header("Cooldown and duration")]
    public float defenceDuration = 120; // ��� �ð�
    private float defenceTimer; // ��� �ð� Ÿ�̸�
    public float waveCooldown = 15; // ���̺� �� ��� �ð�
    private float waveTimer; // ���̺� Ÿ�̸�

    [Header("Respawn details")]
    public int amountOfRespawnPoints = 2; // ��Ȱ ���� ����
    public List<Transform> respawnPoints; // ��Ȱ ���� ����Ʈ
    private Vector3 defencePoint; // ��� ����
    [Space]
    public int enemiesPerWave; // ���̺�� �� ��
    public GameObject[] possibleEnemies; // ���� ������ �� ���

    private string defenceTimerText; // ��� �ð� �ؽ�Ʈ
    private Enemy_Spawner spawner; // �� ������ ����
    private GameObject[] Spawner; // ������ ��ü �迭

    private void OnEnable()
    {
        defenceBegun = false; // ��� ���� �ʱ�ȭ
    }

    // �̼� ���� �޼���
    public override void StartMission()
    {
        defencePoint = FindObjectOfType<MissionEnd_Trigger>().transform.position; // ��� ���� ����
        spawner = FindObjectOfType<Enemy_Spawner>(); // �� ������ ����
        respawnPoints = new List<Transform>(ClosestPoints(amountOfRespawnPoints)); // ���� ����� ��Ȱ ���� ��������
        Spawner = GameObject.FindGameObjectsWithTag("Spawner"); // ������ ��ü ã��
        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point."); // UI ������Ʈ
    }

    // �̼� �Ϸ� ���� Ȯ��
    public override bool MissionCompleted()
    {
        if (!defenceBegun)
        {
            StartDefenceEvent(); // ��� �̺�Ʈ ����
            return false;
        }

        return defenceTimer < 0; // ��� Ÿ�̸Ӱ� 0 �������� Ȯ��
    }

    // �̼� ������Ʈ
    public override void UpdateMission()
    {
        if (!defenceBegun)
            return;

        waveTimer -= Time.deltaTime; // ���̺� Ÿ�̸� ����
        if (defenceTimer > 0)
            defenceTimer -= Time.deltaTime; // ��� Ÿ�̸� ����

        if (waveTimer < 0)
        {
            //CreateNewEnemies(enemiesPerWave); // �� �� ����
            waveTimer = waveCooldown; // ���̺� Ÿ�̸� �ʱ�ȭ
        }

        // ��� �ð� UI ������Ʈ
        defenceTimerText = System.TimeSpan.FromSeconds(defenceTimer).ToString("mm':'ss");
        string missionText = "Defend yourself till plane is ready to take off.";
        string missionDetails = "Time left: " + defenceTimerText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // ��� �̺�Ʈ ����
    private void StartDefenceEvent()
    {
        waveTimer = 0.5f; // ù ���̺� �غ� �ð�
        defenceTimer = defenceDuration; // ��� �ð� ����
        defenceBegun = true; // ��� ���� ���� ����

        foreach (GameObject obj in Spawner)
        {
            Enemy_Spawner script = obj.GetComponent<Enemy_Spawner>();
            if (script != null)
            {
                // ������ ���� ������Ʈ
                script.EnemyRespawn = true;
                script.maxEnemies = 2;
                script.currentEnemyCount = 0;
            }
            else
            {
                Debug.LogWarning($"{obj.name} does not have Enemy_Spawner attached.");
            }
        }
    }

    // ���ο� �� ����
    private void CreateNewEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomEnemyIndex = Random.Range(0, possibleEnemies.Length); // ���� �� ����
            int randomRespawnIndex = Random.Range(0, respawnPoints.Count); // ���� ��Ȱ ���� ����

            Transform randomRespawnPoint = respawnPoints[randomRespawnIndex];
            GameObject randomEnemy = possibleEnemies[randomEnemyIndex];

            randomEnemy.GetComponent<Enemy>().aggresionRange = 100; // ���� ���� ���� ����
            ObjectPool.instance.GetObject(randomEnemy, randomRespawnPoint); // �� ����
        }
    }

    // ���� ����� ��Ȱ ���� ��������
    private List<Transform> ClosestPoints(int amount)
    {
        List<Transform> closestPoints = new List<Transform>();
        List<MissionObject_EnemyRespawnPoint> allPoints =
            new List<MissionObject_EnemyRespawnPoint>(FindObjectsOfType<MissionObject_EnemyRespawnPoint>());

        while (closestPoints.Count < amount && allPoints.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            MissionObject_EnemyRespawnPoint closestPoint = null;

            foreach (var point in allPoints)
            {
                float distance = Vector3.Distance(point.transform.position, defencePoint);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestPoint = point;
                }
            }

            if (closestPoint != null)
            {
                closestPoints.Add(closestPoint.transform); // ���� ����� ��Ȱ ���� �߰�
                allPoints.Remove(closestPoint); // ���õ� ���� ����
            }
        }

        return closestPoints; // ��Ȱ ���� ��ȯ
    }
}
