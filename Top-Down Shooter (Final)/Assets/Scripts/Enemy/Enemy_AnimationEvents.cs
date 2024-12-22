using UnityEngine;

// 적의 애니메이션 이벤트를 처리하는 클래스
public class Enemy_AnimationEvents : MonoBehaviour
{
    private Enemy enemy; // 적의 일반 기능 참조
    private Enemy_Melee enemyMelee; // 근접 적 기능 참조
    private Enemy_Boss enemyBoss; // 보스 적 기능 참조

    // 초기화: 부모 객체에서 적 클래스 참조 가져오기
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        enemyMelee = GetComponentInParent<Enemy_Melee>();
        enemyBoss = GetComponentInParent<Enemy_Boss>();
    }

    // 애니메이션 트리거 호출
    public void AnimationTrigger() => enemy.AnimationTrigger();

    // 수동 이동 시작
    public void StartManualMovement() => enemy.ActivateManualMovement(true);

    // 수동 이동 정지
    public void StopManualMovement() => enemy.ActivateManualMovement(false);

    // 수동 회전 시작
    public void StartManualRotation() => enemy.ActivateManualRotation(true);

    // 수동 회전 정지
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    // 능력 발동 이벤트
    public void AbilityEvent() => enemy.AbilityTrigger();

    // IK 활성화
    public void EnableIK() => enemy.visuals.EnableIK(true, true, 1f);

    // 무기 모델 활성화
    public void EnableWeaponModel()
    {
        enemy.visuals.EnableWeaponModel(true); // 주 무기 활성화
        enemy.visuals.EnableSeconoderyWeaponModel(false); // 보조 무기 비활성화
    }

    // 보스 점프 충격 이벤트
    public void BossJumpImpact()
    {
        enemyBoss?.JumpImpact(); // 보스 전용 점프 충격 호출
    }

    // 근접 공격 판정 시작
    public void BeginMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(true); // 근접 공격 활성화
        enemy?.audioManager.PlaySFX(enemyMelee?.meleeSFX.swoosh, true); // 근접 공격 효과음 재생
    }

    // 근접 공격 판정 종료
    public void FinishMeleeAttackCheck()
    {
        enemy?.EnableMeleeAttackCheck(false); // 근접 공격 비활성화
    }
}
