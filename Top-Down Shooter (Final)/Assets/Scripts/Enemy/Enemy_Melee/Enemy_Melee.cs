using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// 적 근접 공격 데이터를 정의하는 구조체
public struct AttackData_EnemyMelee
{
    public int attackDamage; // 공격 데미지
    public string attackName; // 공격 이름
    public float attackRange; // 공격 범위
    public float moveSpeed; // 이동 속도
    public float attackIndex; // 공격 인덱스
    [Range(1, 2)]
    public float animationSpeed; // 애니메이션 속도
    public AttackType_Melee attackType; // 공격 타입
}
public enum AttackType_Melee { Close, Charge } // 근접 공격 타입
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow } // 적 유형

public class Enemy_Melee : Enemy
{
    public Enemy_MeleeSFX meleeSFX { get; private set; } // 근접 공격 효과음

    #region States
    public IdleState_Melee idleState { get; private set; } // 대기 상태
    public MoveState_Melee moveState { get; private set; } // 이동 상태
    public RecoveryState_Melee recoveryState { get; private set; } // 회복 상태
    public ChaseState_Melee chaseState { get; private set; } // 추적 상태
    public AttackState_Melee attackState { get; private set; } // 공격 상태
    public DeadState_Melee deadState { get; private set; } // 사망 상태
    public AbilityState_Melee abilityState { get; private set; } // 능력 사용 상태
    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType; // 적 유형
    public Enemy_MeleeWeaponType weaponType; // 무기 유형

    [Header("Shield")]
    public int shieldDurability; // 방패 내구도
    public Transform shieldTransform; // 방패 위치

    [Header("Dodge")]
    public float dodgeCooldown; // 회피 쿨다운
    private float lastTimeDodge = -10; // 마지막 회피 시간

    [Header("Axe throw ability")]
    public int axeDamage; // 도끼 데미지
    public GameObject axePrefab; // 도끼 프리팹
    public float axeFlySpeed; // 도끼 비행 속도
    public float axeAimTimer; // 도끼 조준 시간
    public float axeThrowCooldown; // 도끼 던지기 쿨다운
    private float lastTimeAxeThrown; // 마지막 도끼 던지기 시간
    public Transform axeStartPoint; // 도끼 시작 위치

    [Header("Attack Data")]
    public AttackData_EnemyMelee attackData; // 기본 공격 데이터
    public List<AttackData_EnemyMelee> attackList; // 여러 공격 데이터
    private Enemy_WeaponModel currentWeapon; // 현재 무기 모델
    private bool isAttackReady; // 공격 준비 여부
    [Space]
    [SerializeField] private GameObject meleeAttackFx; // 근접 공격 효과

    protected override void Awake()
    {
        base.Awake();

        // 상태 초기화
        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle");
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        meleeSFX = GetComponent<Enemy_MeleeSFX>(); // 효과음 초기화
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 초기 상태 설정
        ResetCooldown(); // 쿨다운 초기화

        InitializePerk(); // 특성 초기화
        visuals.SetupLook(); // 비주얼 설정
        UpdateAttackData(); // 공격 데이터 업데이트
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update(); // 현재 상태 업데이트

        // 근접 공격 판정
        MeleeAttackCheck(currentWeapon.damagePoints, currentWeapon.attackRadius, meleeAttackFx, attackData.attackDamage);
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState); // 회복 상태로 전환
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        walkSpeed = walkSpeed * .6f; // 걷기 속도 감소
        visuals.EnableWeaponModel(false); // 무기 모델 비활성화
    }

    public void UpdateAttackData()
    {
        // 현재 무기 데이터 업데이트
        currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_WeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed; // 회전 속도 업데이트
        }
    }

    protected override void InitializePerk()
    {
        // 특정 적 유형에 따른 특성 초기화
        if (meleeType == EnemyMelee_Type.AxeThrow)
        {
            weaponType = Enemy_MeleeWeaponType.Throw;
        }

        if (meleeType == EnemyMelee_Type.Shield)
        {
            anim.SetFloat("ChaseIndex", 1);
            shieldTransform.gameObject.SetActive(true);
            weaponType = Enemy_MeleeWeaponType.OneHand;
        }

        if (meleeType == EnemyMelee_Type.Dodge)
        {
            weaponType = Enemy_MeleeWeaponType.Unarmed;
        }
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState); // 사망 상태로 전환
    }

    public void ActivateDodgeRoll()
    {
        // 회피 동작 활성화
        if (meleeType != EnemyMelee_Type.Dodge)
            return;

        if (stateMachine.currentState != chaseState)
            return;

        if (Vector3.Distance(transform.position, player.position) < 2f)
            return;

        float dodgeAnimationDuration = GetAnimationClipDuration("Dodge roll");

        if (Time.time > dodgeCooldown + dodgeAnimationDuration + lastTimeDodge)
        {
            lastTimeDodge = Time.time;
            anim.SetTrigger("Dodge");
        }
    }

    public void ThrowAxe()
    {
        // 도끼 던지기
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);

        newAxe.GetComponent<Enemy_Axe>().AxeSetup(axeFlySpeed, player, axeAimTimer, axeDamage);
    }

    public bool CanThrowAxe()
    {
        // 도끼 던질 수 있는지 확인
        if (meleeType != EnemyMelee_Type.AxeThrow)
            return false;

        if (Time.time > axeThrowCooldown + lastTimeAxeThrown)
        {
            lastTimeAxeThrown = Time.time;
            return true;
        }
        return false;
    }

    private void ResetCooldown()
    {
        // 쿨다운 초기화
        lastTimeDodge -= dodgeCooldown;
        lastTimeAxeThrown -= axeThrowCooldown;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        // 애니메이션 클립 길이 반환
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        Debug.Log(clipName + "animation not found!");
        return 0;
    }

    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackData.attackRange;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange); // 공격 범위 표시
    }
}
