using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

// LevelGenerator: ������ �������� �����ϰ� �����ϴ� Ŭ����
public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance; // �̱��� �ν��Ͻ�

    // �� ����Ʈ
    private List<Enemy> enemyList;

    // NavMesh ���� ����
    [SerializeField] private NavMeshSurface navMeshSurface;

    // ���� ��Ʈ ����
    [SerializeField] private Transform lastLevelPart; // ������ ���� ��Ʈ
    [SerializeField] private List<Transform> levelParts; // ����� ���� ��Ʈ ����Ʈ
    private List<Transform> currentLevelParts; // ���� ���� ������ ���� ��Ʈ
    private List<Transform> generatedLevelParts = new List<Transform>(); // ������ ���� ��Ʈ

    // Snap Point (���� ����)
    [SerializeField] private SnapPoint nextSnapPoint; // ���� Snap Point
    private SnapPoint defaultSnapPoint; // �ʱ� Snap Point

    // ���� ��ٿ�
    [SerializeField] private float generationCooldown; // ���� ��� �ð�
    private float cooldownTimer;
    private bool generationOver = true; // ���� �Ϸ� ����


    private MissionManager missionManager; // �̼� ���� �Ŵ���


    private void Awake()
    {
        instance = this; // �̱��� ����
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;

        missionManager = MissionManager.instance;

    }

    private void Update()
    {
        if (generationOver) // ������ �Ϸ�Ǿ����� ����
            return;

        cooldownTimer -= Time.deltaTime; // ��ٿ� ����

        if (cooldownTimer < 0)
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart(); // ���� ���� ��Ʈ ����
            }
            else if (!generationOver)
            {
                FinishGeneration(); // ���� �Ϸ� ó��
            }
        }
    }

    // ���� ���� �ʱ�ȭ
    [ContextMenu("Restart generation")]
    public void InitializeGeneration()
    {


        nextSnapPoint = defaultSnapPoint; // Snap Point �ʱ�ȭ
        generationOver = false;

        // Mission_LastDefence Ȯ��
        if (MissionManager.instance.currentMission is Mission_LastDefence)
        {
            // levelParts���� �������� �ϳ� ����
            if (levelParts.Count > 0)
            {
                int randomIndex = Random.Range(0, levelParts.Count);
                Transform chosenPart = levelParts[randomIndex];

                levelParts.Clear(); // ��� ��� ����
                levelParts.Add(chosenPart); // ���õ� ��Ҹ� �߰�
            }
        }

        // currentLevelParts �ʱ�ȭ
        currentLevelParts = new List<Transform>(levelParts);

        DestroyOldLevelPartsAndEnemies(); // ���� ���� ��Ʈ �� �� ����
    }

    // ���� ���� ��Ʈ �� �� ����
    private void DestroyOldLevelPartsAndEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject); // �� ����
        }

        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject); // ���� ��Ʈ ����
        }

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    // ���� ���� �Ϸ� ó��
    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart(); // ������ ���� ��Ʈ ����

        navMeshSurface.BuildNavMesh(); // NavMesh ����

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null; // �θ𿡼� �и�
            enemy.gameObject.SetActive(true); // Ȱ��ȭ
        }

        MissionManager.instance.StartMission(); // �̼� ����
    }

    // ���� ���� ��Ʈ ����
    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        // ���� �Ϸ� ���ο� ���� ������ ���� ��Ʈ ���� �Ǵ� ���� ����
        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoint); // Snap Point�� ���� ����

        // ���� ���� �� �����
        if (levelPartScript.IntersectionDetected())
        {
            InitializeGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitPoint(); // ���� Snap Point ����
        enemyList.AddRange(levelPartScript.MyEnemies()); // �� ����Ʈ�� �߰�
    }

    // ���� ���� ��Ʈ ����
    private Transform ChooseRandomPart()
    {
        if (currentLevelParts == null || currentLevelParts.Count == 0)
        {
            Debug.LogError("No level parts available to choose from!");
            return null;
        }

        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex); // ���õ� ��Ʈ ����
        return chosenPart;
    }

    // ���� �� ��ȯ
    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }

    // �� ����Ʈ ��ȯ
    public List<Enemy> GetEnemyList() => enemyList;
}
