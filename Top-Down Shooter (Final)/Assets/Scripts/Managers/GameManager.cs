using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// GameManager: 게임 상태를 관리하는 클래스
public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 싱글턴 인스턴스
    public Player player; // 플레이어 객체 참조

    private Mission_Timer mission;

    [Header("Settings")]
    public bool friendlyFire; // 아군 피해 허용 여부

    private void Awake()
    {
        instance = this; // 싱글턴 설정

        // 씬 내의 플레이어 객체를 찾음
        player = FindObjectOfType<Player>();

        if (mission != null)
        {
            mission.ResetMission(); // 미션 초기화
        }
    }

    // 게임 시작 시 호출
    public void GameStart()
    {
        SetDefaultWeaponsForPlayer(); // 플레이어의 기본 무기 설정

        // LevelGenerator에서 레벨 생성 및 미션 시작
        // LevelGenerator.instance.InitializeGeneration();
        // 미션 시작은 LevelGenerator에서 레벨 생성 완료 후 처리
    }

    // 현재 씬을 재시작
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드

        if (mission != null)
        {
            mission.ResetMission(); // 미션 초기화
        }
    }

    // 게임 클리어 처리
    public void GameCompleted()
    {
        // 승리 화면 UI 표시
        UI.instance.ShowVictoryScreenUI();

        // 플레이어 컨트롤 비활성화
        ControlsManager.instance.controls.Character.Disable();

        // 플레이어 체력을 매우 높게 설정하여 마지막 순간 사망 방지
        player.health.currentHealth += 99999;
    }

    // 게임 오버 처리
    public void GameOver()
    {
        // 슬로우 모션 효과 적용
        TimeManager.instance.SlowMotionFor(1.5f);

        // 게임 오버 화면 UI 표시
        UI.instance.ShowGameOverUI();

        // 카메라 거리 조정
        CameraManager.instance.ChangeCameraDistance(5);
    }

    // 플레이어의 기본 무기 설정
    private void SetDefaultWeaponsForPlayer()
    {
        // UI에서 선택된 무기 데이터를 가져옴
        List<Weapon_Data> newList = UI.instance.weaponSelection.SelectedWeaponData();

        // 플레이어 무기 초기화
        player.weapon.SetDefaultWeapon(newList);
    }
}
