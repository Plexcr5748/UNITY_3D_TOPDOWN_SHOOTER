using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player_AnimationEvents: 플레이어의 애니메이션 이벤트를 처리하는 클래스
public class Player_AnimationEvents : MonoBehaviour
{
    private Player_WeaponVisuals visualController; // 무기 시각 효과 관리
    private Player_WeaponController weaponController; // 무기 컨트롤러

    private void Start()
    {
        // 부모 객체에서 관련 컨트롤러 컴포넌트 가져오기
        visualController = GetComponentInParent<Player_WeaponVisuals>();
        weaponController = GetComponentInParent<Player_WeaponController>();
    }

    public void ReloadIsOver()
    {
        // 재장전 애니메이션이 끝났을 때 호출
        visualController.MaximizeRigWeight(); // 캐릭터 리그(Rig)의 무게 초기화
        visualController.CurrentWeaponModel().realodSfx.Stop(); // 재장전 사운드 효과 정지

        weaponController.CurrentWeapon().RefillBullets(); // 현재 무기의 탄약을 최대치로 보충
        weaponController.SetWeaponReady(true); // 무기 준비 상태로 설정
        weaponController.UpdateWeaponUI(); // UI 업데이트
    }

    public void ReturnRig()
    {
        // 리그(Rig) 상태를 기본값으로 복원
        visualController.MaximizeRigWeight(); // 캐릭터 리그 무게 복원
        visualController.MaximizeLeftHandWeight(); // 왼손 리그 무게 복원
    }

    public void WeaponEquipingIsOver()
    {
        // 무기 장착 애니메이션이 끝났을 때 호출
        weaponController.SetWeaponReady(true); // 무기 준비 상태로 설정
    }

    public void SwitchOnWeaponModel()
    {
        // 무기 모델을 활성화
        visualController.SwitchOnCurrentWeaponModel();
    }
}
