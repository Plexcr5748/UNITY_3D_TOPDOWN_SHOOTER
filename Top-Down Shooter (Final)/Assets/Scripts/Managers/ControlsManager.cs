using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ControlsManager: 게임 내 컨트롤 모드를 관리하는 클래스
public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance; // 싱글턴 인스턴스
    public PlayerControls controls { get; private set; } // 입력 시스템 클래스
    private Player player; // 플레이어 객체 참조

    private void Awake()
    {
        instance = this; // 싱글턴 설정
        controls = new PlayerControls(); // PlayerControls 초기화
    }

    private void Start()
    {
        player = GameManager.instance.player; // GameManager에서 플레이어 객체 가져오기

        // 초기 컨트롤을 캐릭터 컨트롤로 설정
        SwitchToCharacterControls();
    }

    // 캐릭터 컨트롤 활성화
    public void SwitchToCharacterControls()
    {
        controls.Character.Enable(); // 캐릭터 컨트롤 활성화

        controls.Car.Disable(); // 차량 컨트롤 비활성화
        controls.UI.Disable(); // UI 컨트롤 비활성화

        player.SetControlsEnabledTo(true); // 플레이어 입력 활성화
        UI.instance.inGameUI.SwitchToCharcaterUI(); // UI를 캐릭터 모드로 전환
    }

    // UI 컨트롤 활성화
    public void SwitchToUIControls()
    {
        controls.UI.Enable(); // UI 컨트롤 활성화

        controls.Car.Disable(); // 차량 컨트롤 비활성화
        controls.Character.Disable(); // 캐릭터 컨트롤 비활성화

        player.SetControlsEnabledTo(false); // 플레이어 입력 비활성화
    }

    // 차량 컨트롤 활성화
    public void SwitchToCarControls()
    {
        controls.Car.Enable(); // 차량 컨트롤 활성화

        controls.UI.Disable(); // UI 컨트롤 비활성화
        controls.Character.Disable(); // 캐릭터 컨트롤 비활성화

        player.SetControlsEnabledTo(false); // 플레이어 입력 비활성화
        UI.instance.inGameUI.SwitchToCarUI(); // UI를 차량 모드로 전환
    }
}
