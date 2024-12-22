using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 적의 전투 상태를 관리하는 클래스
public class BattleState_Range : EnemyState
{
    private Enemy_Range enemy; // 현재 상태가 참조하는 Enemy_Range 객체

    private float lastTimeShot = -10; // 마지막 총알 발사 시간
    private int bulletsShot = 0; // 발사된 총알 수

    private int bulletsPerAttack; // 한 번 공격에 사용할 총알 수
    private float weaponCooldown; // 무기 쿨다운 시간

    private float coverCheckTimer; // 엄폐지 변경 확인 타이머
    private bool firstTimeAttack = true; // 첫 번째 공격 여부

    public BattleState_Range(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        enemy = enemyBase as Enemy_Range; // Enemy를 Enemy_Range로 캐스팅
    }

    public override void Enter()
    {
        base.Enter();
        SetupValuesForFirstAttack(); // 첫 번째 공격을 위한 값 설정

        enemy.agent.isStopped = true; // 이동 중지
        enemy.agent.velocity = Vector3.zero; // 이동 속도 초기화

        enemy.visuals.EnableIK(true, true); // IK 활성화

        stateTimer = enemy.attackDelay; // 공격 지연 시간 설정
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsSeeingPlayer()) // 플레이어를 볼 수 있으면
            enemy.FaceTarget(enemy.aim.position); // 플레이어를 향하도록 설정

        if (enemy.CanThrowGrenade()) // 수류탄 던지기 가능하면
            stateMachine.ChangeState(enemy.throwGrenadeState); // 수류탄 던지기 상태로 전환

        if (MustAdvancePlayer()) // 플레이어에게 전진해야 할 경우
            stateMachine.ChangeState(enemy.advancePlayerState); // 전진 상태로 전환

        ChangeCoverIfShould(); // 엄폐지 변경 필요 여부 확인

        if (stateTimer > 0)
            return;

        if (WeaponOutOfBullets()) // 총알이 부족하면
        {
            if (enemy.IsUnstopppable() && UnstoppableWalkReady()) // 방해 불가 상태이고 전진할 준비가 되면
            {
                enemy.advanceDuration = weaponCooldown;
                stateMachine.ChangeState(enemy.advancePlayerState); // 전진 상태로 전환
            }

            if (WeaponOnCooldown()) // 무기가 쿨다운 중이면
                AttemptToResetWeapon(); // 무기 초기화

            return;
        }

        if (CanShoot() && enemy.IsAimOnPlayer()) // 플레이어를 조준할 수 있으면
        {
            Shoot(); // 발사
        }
    }

    private bool MustAdvancePlayer()
    {
        if (enemy.IsUnstopppable()) // 방해 불가 상태인 경우 전진하지 않음
            return false;

        return enemy.IsPlayerInAgrresionRange() == false && ReadyToLeaveCover(); // 공격 범위 밖에 있고 엄폐를 떠날 준비가 되면 전진
    }

    private bool UnstoppableWalkReady()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);
        bool outOfStoppingDistance = distanceToPlayer > enemy.advanceStoppingDistance;
        bool unstoppableWalkOnCooldown = Time.time < enemy.weaponData.maxWeaponCooldown + enemy.advancePlayerState.lastTimeAdvanced;

        return outOfStoppingDistance && unstoppableWalkOnCooldown == false; // 전진 준비 완료
    }

    #region Cover system region

    private bool ReadyToLeaveCover()
    {
        return Time.time > enemy.minCoverTime + enemy.runToCoverState.lastTimeTookCover; // 최소 엄폐 시간 이후
    }

    private void ChangeCoverIfShould()
    {
        if (enemy.coverPerk != CoverPerk.CanTakeAndChangeCover) // 엄폐 변경 능력이 없으면
            return;

        coverCheckTimer -= Time.deltaTime; // 타이머 감소

        if (coverCheckTimer < 0)
        {
            coverCheckTimer = .5f; // 0.5초마다 엄폐 체크

            if (ReadyToChangeCover() && ReadyToLeaveCover()) // 엄폐지 변경 준비가 되면
            {
                if (enemy.CanGetCover()) // 새로운 엄폐지로 이동할 수 있으면
                    stateMachine.ChangeState(enemy.runToCoverState); // 엄폐 상태로 전환
            }
        }
    }

    private bool ReadyToChangeCover()
    {
        bool inDanger = IsPlayerInClearSight() || IsPlayerClose(); // 플레이어가 명확히 보이거나 가까운 경우
        bool advanceTimeIsOver = Time.time > enemy.advancePlayerState.lastTimeAdvanced + enemy.advanceDuration; // 전진 시간이 끝나면

        return inDanger && advanceTimeIsOver; // 위험하고 전진 시간이 끝났다면 엄폐 변경 가능
    }

    private bool IsPlayerClose()
    {
        return Vector3.Distance(enemy.transform.position, enemy.player.transform.position) < enemy.safeDistance; // 플레이어가 너무 가까운지 확인
    }

    private bool IsPlayerInClearSight()
    {
        Vector3 directionToPlayer = enemy.player.transform.position - enemy.transform.position;

        if (Physics.Raycast(enemy.transform.position, directionToPlayer, out RaycastHit hit)) // 플레이어를 볼 수 있으면
        {
            if (hit.transform.root == enemy.player.root)
                return true;
        }

        return false; // 플레이어가 보이지 않으면
    }

    #endregion

    #region Weapon region

    private void AttemptToResetWeapon()
    {
        bulletsShot = 0; // 발사된 총알 수 초기화
        bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack(); // 공격당 발사되는 총알 수
        weaponCooldown = enemy.weaponData.GetWeaponCooldown(); // 무기 쿨다운 시간
    }

    private bool WeaponOnCooldown() => Time.time > lastTimeShot + weaponCooldown; // 무기 쿨다운 확인
    private bool WeaponOutOfBullets() => bulletsShot >= bulletsPerAttack; // 총알 부족 여부 확인
    private bool CanShoot() => Time.time > lastTimeShot + 1 / enemy.weaponData.fireRate; // 발사 가능 여부 확인

    private void Shoot()
    {
        enemy.FireSingleBullet(); // 총알 발사
        lastTimeShot = Time.time; // 마지막 발사 시간 갱신
        bulletsShot++; // 발사된 총알 수 증가
    }

    private void SetupValuesForFirstAttack()
    {
        if (firstTimeAttack)
        {
            // 적이 전진할 때 공격 범위와 전진 중단 거리 설정
            enemy.aggresionRange = enemy.advanceStoppingDistance + 2;

            firstTimeAttack = false; // 첫 번째 공격 플래그 설정
            bulletsPerAttack = enemy.weaponData.GetBulletsPerAttack(); // 공격당 발사되는 총알 수 설정
            weaponCooldown = enemy.weaponData.GetWeaponCooldown(); // 무기 쿨다운 설정
        }
    }

    #endregion
}
