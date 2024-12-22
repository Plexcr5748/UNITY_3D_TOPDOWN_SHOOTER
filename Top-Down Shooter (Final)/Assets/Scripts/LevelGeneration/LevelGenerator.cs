using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

// LevelGenerator: 레벨을 동적으로 생성하고 관리하는 클래스
public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance; // 싱글턴 인스턴스

    // 적 리스트
    private List<Enemy> enemyList;

    // NavMesh 생성 관련
    [SerializeField] private NavMeshSurface navMeshSurface;

    // 레벨 파트 설정
    [SerializeField] private Transform lastLevelPart; // 마지막 레벨 파트
    [SerializeField] private List<Transform> levelParts; // 사용할 레벨 파트 리스트
    private List<Transform> currentLevelParts; // 현재 생성 가능한 레벨 파트
    private List<Transform> generatedLevelParts = new List<Transform>(); // 생성된 레벨 파트

    // Snap Point (연결 지점)
    [SerializeField] private SnapPoint nextSnapPoint; // 다음 Snap Point
    private SnapPoint defaultSnapPoint; // 초기 Snap Point

    // 생성 쿨다운
    [SerializeField] private float generationCooldown; // 생성 대기 시간
    private float cooldownTimer;
    private bool generationOver = true; // 생성 완료 상태


    private MissionManager missionManager; // 미션 관리 매니저


    private void Awake()
    {
        instance = this; // 싱글턴 설정
    }

    private void Start()
    {
        enemyList = new List<Enemy>();
        defaultSnapPoint = nextSnapPoint;

        missionManager = MissionManager.instance;

    }

    private void Update()
    {
        if (generationOver) // 생성이 완료되었으면 종료
            return;

        cooldownTimer -= Time.deltaTime; // 쿨다운 감소

        if (cooldownTimer < 0)
        {
            if (currentLevelParts.Count > 0)
            {
                cooldownTimer = generationCooldown;
                GenerateNextLevelPart(); // 다음 레벨 파트 생성
            }
            else if (!generationOver)
            {
                FinishGeneration(); // 생성 완료 처리
            }
        }
    }

    // 레벨 생성 초기화
    [ContextMenu("Restart generation")]
    public void InitializeGeneration()
    {


        nextSnapPoint = defaultSnapPoint; // Snap Point 초기화
        generationOver = false;

        // Mission_LastDefence 확인
        if (MissionManager.instance.currentMission is Mission_LastDefence)
        {
            // levelParts에서 랜덤으로 하나 선택
            if (levelParts.Count > 0)
            {
                int randomIndex = Random.Range(0, levelParts.Count);
                Transform chosenPart = levelParts[randomIndex];

                levelParts.Clear(); // 모든 요소 제거
                levelParts.Add(chosenPart); // 선택된 요소만 추가
            }
        }

        // currentLevelParts 초기화
        currentLevelParts = new List<Transform>(levelParts);

        DestroyOldLevelPartsAndEnemies(); // 기존 레벨 파트 및 적 제거
    }

    // 기존 레벨 파트 및 적 제거
    private void DestroyOldLevelPartsAndEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            Destroy(enemy.gameObject); // 적 제거
        }

        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject); // 레벨 파트 제거
        }

        generatedLevelParts = new List<Transform>();
        enemyList = new List<Enemy>();
    }

    // 레벨 생성 완료 처리
    private void FinishGeneration()
    {
        generationOver = true;
        GenerateNextLevelPart(); // 마지막 레벨 파트 생성

        navMeshSurface.BuildNavMesh(); // NavMesh 생성

        foreach (Enemy enemy in enemyList)
        {
            enemy.transform.parent = null; // 부모에서 분리
            enemy.gameObject.SetActive(true); // 활성화
        }

        MissionManager.instance.StartMission(); // 미션 시작
    }

    // 다음 레벨 파트 생성
    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        // 생성 완료 여부에 따라 마지막 레벨 파트 생성 또는 랜덤 선택
        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoint); // Snap Point에 맞춰 정렬

        // 교차 감지 시 재생성
        if (levelPartScript.IntersectionDetected())
        {
            InitializeGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitPoint(); // 다음 Snap Point 설정
        enemyList.AddRange(levelPartScript.MyEnemies()); // 적 리스트에 추가
    }

    // 랜덤 레벨 파트 선택
    private Transform ChooseRandomPart()
    {
        if (currentLevelParts == null || currentLevelParts.Count == 0)
        {
            Debug.LogError("No level parts available to choose from!");
            return null;
        }

        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex); // 선택된 파트 제거
        return chosenPart;
    }

    // 랜덤 적 반환
    public Enemy GetRandomEnemy()
    {
        int randomIndex = Random.Range(0, enemyList.Count);
        return enemyList[randomIndex];
    }

    // 적 리스트 반환
    public List<Enemy> GetEnemyList() => enemyList;
}
