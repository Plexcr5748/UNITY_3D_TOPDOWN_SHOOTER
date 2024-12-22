using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mission_LastDefence: 방어 미션 클래스
[CreateAssetMenu(fileName = "Defence - Mission", menuName = "Missions/Defence - Mission")]
public class Mission_LastDefence : Mission
{
    public bool defenceBegun = false; // 방어 이벤트 시작 여부

    [Header("Cooldown and duration")]
    public float defenceDuration = 120; // 방어 시간
    private float defenceTimer; // 방어 시간 타이머
    public float waveCooldown = 15; // 웨이브 간 대기 시간
    private float waveTimer; // 웨이브 타이머

    [Header("Respawn details")]
    public int amountOfRespawnPoints = 2; // 부활 지점 개수
    public List<Transform> respawnPoints; // 부활 지점 리스트
    private Vector3 defencePoint; // 방어 지점
    [Space]
    public int enemiesPerWave; // 웨이브당 적 수
    public GameObject[] possibleEnemies; // 생성 가능한 적 목록

    private string defenceTimerText; // 방어 시간 텍스트
    private Enemy_Spawner spawner; // 적 스포너 참조
    private GameObject[] Spawner; // 스포너 객체 배열

    private void OnEnable()
    {
        defenceBegun = false; // 방어 상태 초기화
    }

    // 미션 시작 메서드
    public override void StartMission()
    {
        defencePoint = FindObjectOfType<MissionEnd_Trigger>().transform.position; // 방어 지점 설정
        spawner = FindObjectOfType<Enemy_Spawner>(); // 적 스포너 참조
        respawnPoints = new List<Transform>(ClosestPoints(amountOfRespawnPoints)); // 가장 가까운 부활 지점 가져오기
        Spawner = GameObject.FindGameObjectsWithTag("Spawner"); // 스포너 객체 찾기
        UI.instance.inGameUI.UpdateMissionInfo("Get to the evacuation point."); // UI 업데이트
    }

    // 미션 완료 여부 확인
    public override bool MissionCompleted()
    {
        if (!defenceBegun)
        {
            StartDefenceEvent(); // 방어 이벤트 시작
            return false;
        }

        return defenceTimer < 0; // 방어 타이머가 0 이하인지 확인
    }

    // 미션 업데이트
    public override void UpdateMission()
    {
        if (!defenceBegun)
            return;

        waveTimer -= Time.deltaTime; // 웨이브 타이머 감소
        if (defenceTimer > 0)
            defenceTimer -= Time.deltaTime; // 방어 타이머 감소

        if (waveTimer < 0)
        {
            //CreateNewEnemies(enemiesPerWave); // 새 적 생성
            waveTimer = waveCooldown; // 웨이브 타이머 초기화
        }

        // 방어 시간 UI 업데이트
        defenceTimerText = System.TimeSpan.FromSeconds(defenceTimer).ToString("mm':'ss");
        string missionText = "Defend yourself till plane is ready to take off.";
        string missionDetails = "Time left: " + defenceTimerText;

        UI.instance.inGameUI.UpdateMissionInfo(missionText, missionDetails);
    }

    // 방어 이벤트 시작
    private void StartDefenceEvent()
    {
        waveTimer = 0.5f; // 첫 웨이브 준비 시간
        defenceTimer = defenceDuration; // 방어 시간 설정
        defenceBegun = true; // 방어 시작 상태 설정

        foreach (GameObject obj in Spawner)
        {
            Enemy_Spawner script = obj.GetComponent<Enemy_Spawner>();
            if (script != null)
            {
                // 스포너 설정 업데이트
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

    // 새로운 적 생성
    private void CreateNewEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomEnemyIndex = Random.Range(0, possibleEnemies.Length); // 랜덤 적 선택
            int randomRespawnIndex = Random.Range(0, respawnPoints.Count); // 랜덤 부활 지점 선택

            Transform randomRespawnPoint = respawnPoints[randomRespawnIndex];
            GameObject randomEnemy = possibleEnemies[randomEnemyIndex];

            randomEnemy.GetComponent<Enemy>().aggresionRange = 100; // 적의 공격 범위 설정
            ObjectPool.instance.GetObject(randomEnemy, randomRespawnPoint); // 적 생성
        }
    }

    // 가장 가까운 부활 지점 가져오기
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
                closestPoints.Add(closestPoint.transform); // 가장 가까운 부활 지점 추가
                allPoints.Remove(closestPoint); // 선택된 지점 제거
            }
        }

        return closestPoints; // 부활 지점 반환
    }
}
