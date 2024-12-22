using UnityEngine;

// Player: 플레이어와 관련된 모든 주요 컴포넌트를 초기화하고 제어하는 클래스
public class Player : MonoBehaviour
{
    public Transform playerBody; // 플레이어의 몸체 트랜스폼

    // 플레이어와 관련된 주요 컴포넌트와 컨트롤 선언
    public PlayerControls controls { get; private set; } // 플레이어 입력 컨트롤
    public Player_AimController aim { get; private set; } // 조준 컨트롤러
    public Player_Movement movement { get; private set; } // 이동 컨트롤러
    public Player_WeaponController weapon { get; private set; } // 무기 컨트롤러
    public Player_WeaponVisuals weaponVisuals { get; private set; } // 무기 시각 효과
    public Player_Interaction interaction { get; private set; } // 상호작용 컨트롤러
    public Player_Health health { get; private set; } // 플레이어 체력 관리
    public Ragdoll ragdoll { get; private set; } // 레그돌 처리

    public Animator anim { get; private set; } // 애니메이터
    public Player_SoundFX sound { get; private set; } // 사운드 효과 관리

    public bool controlsEnabled { get; private set; } // 입력 컨트롤 활성화 여부

    private void Awake()
    {
        // 플레이어 관련 모든 컴포넌트 초기화
        anim = GetComponentInChildren<Animator>(); // 애니메이터
        ragdoll = GetComponent<Ragdoll>(); // 레그돌 컴포넌트
        health = GetComponent<Player_Health>(); // 체력 관리
        aim = GetComponent<Player_AimController>(); // 조준 관리
        movement = GetComponent<Player_Movement>(); // 이동 관리
        weapon = GetComponent<Player_WeaponController>(); // 무기 관리
        weaponVisuals = GetComponent<Player_WeaponVisuals>(); // 무기 시각 효과
        interaction = GetComponent<Player_Interaction>(); // 상호작용 관리
        sound = GetComponent<Player_SoundFX>(); // 사운드 효과
        controls = ControlsManager.instance.controls; // 입력 컨트롤 초기화
    }

    private void OnEnable()
    {
        // 입력 컨트롤 활성화 및 이벤트 등록
        controls.Enable();
        controls.Character.UIMissionToolTipSwitch.performed += ctx => UI.instance.inGameUI.SwitchMissionTooltip(); // 미션 툴팁 전환
        controls.Character.UIPause.performed += ctx => UI.instance.PauseSwitch(); // 게임 일시정지 전환
    }

    private void OnDisable()
    {
        // 입력 컨트롤 비활성화
        controls.Disable();
    }

    public void SetControlsEnabledTo(bool enabled)
    {
        // 플레이어 입력 컨트롤 활성화/비활성화
        controlsEnabled = enabled;

        // 레그돌 및 조준 레이저 활성화 여부 설정
        ragdoll.CollidersActive(enabled);
        aim.EnableAimLaer(enabled);
    }
}
