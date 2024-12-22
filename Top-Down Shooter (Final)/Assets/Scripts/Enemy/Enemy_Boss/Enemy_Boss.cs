using System.Collections.Generic;
using UnityEngine;

// 보스 적 클래스, 다양한 무기와 능력을 관리
public enum BossWeaponType { Flamethrower, Hummer }

public class Enemy_Boss : Enemy
{
    [Header("Boss details")]
    public BossWeaponType bossWeaponType; // 보스의 무기 유형
    public float actionCooldown = 10; // 행동 쿨다운
    public float attackRange; // 보스의 공격 범위

    [Header("Ability")]
    public float minAbilityDistance; // 능력을 사용할 최소 거리
    public float abilityCooldown; // 능력 사용 쿨다운
    private float lastTimeUsedAbility; // 마지막 능력 사용 시간

    [Header("Flamethrower")]
    public int flameDamage; // 화염 피해량
    public float flameDamageCooldown; // 화염 피해 간격
    public ParticleSystem flamethrower; // 화염 방사기
    public float flamethrowDuration; // 화염 방사 시간
    public bool flamethrowActive { get; private set; } // 화염 방사 활성 상태

    [Header("Hummer")]
    public int hummerActiveDamage; // 망치 피해량
    public GameObject activationPrefab; // 망치 효과 프리팹
    [SerializeField] private float hummerCheckRadius; // 망치 공격 범위

    [Header("Jump attack")]
    public int jumpAttackDamage; // 점프 공격 피해량
    public float jumpAttackCooldown = 10; // 점프 공격 쿨다운
    private float lastTimeJumped; // 마지막 점프 공격 시간
    public float travelTimeToTarget = 1; // 점프 도달 시간
    public float minJumpDistanceRequired; // 점프 공격 최소 거리
    [Space]
    public float impactRadius = 2.5f; // 점프 충격 반경
    public float impactPower = 5; // 점프 충격 강도
    public Transform impactPoint; // 충격 중심점
    [SerializeField] private float upforceMultiplier = 10; // 충격 위쪽 힘 배율
    [Space]
    [SerializeField] private LayerMask whatToIngore; // 무시할 레이어

    [Header("Attack")]
    [SerializeField] private int meleeAttackDamage; // 근접 공격 피해량
    [SerializeField] private Transform[] damagePoints; // 공격 지점
    [SerializeField] private float attackCheckRadius; // 공격 체크 반경
    [SerializeField] private GameObject meleeAttackFx; // 근접 공격 효과

    // 보스 상태 관리
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public DeadState_Boss deadState { get; private set; }

    public Enemy_BossVisuals bossVisuals { get; private set; }

    // 초기화
    protected override void Awake()
    {
        base.Awake();
        bossVisuals = GetComponent<Enemy_BossVisuals>();

        // 상태 초기화
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle"); // Idle 상태는 대체로 ragdoll 사용
    }

    // 상태 머신 초기화
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    // 상태 업데이트 및 공격 체크
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFx, meleeAttackDamage);
    }

    // 사망 처리
    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    // 전투 모드 진입
    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    // 화염 방사기 활성화/비활성화
    public void ActivateFlamethrower(bool activate)
    {
        flamethrowActive = activate;

        if (!activate)
        {
            flamethrower.Stop();
            anim.SetTrigger("StopFlamethrower");
            return;
        }

        var mainModule = flamethrower.main;
        var extraModule = flamethrower.transform.GetChild(0).GetComponent<ParticleSystem>().main;

        mainModule.duration = flamethrowDuration;
        extraModule.duration = flamethrowDuration;

        flamethrower.Clear();
        flamethrower.Play();
    }

    // 망치 활성화
    public void ActivateHummer()
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);
        ObjectPool.instance.ReturnObject(newActivation, 1);
        MassDamage(damagePoints[0].position, hummerCheckRadius, hummerActiveDamage);
    }

    // 능력 사용 가능 여부 체크
    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        if (!playerWithinDistance || Time.time <= lastTimeUsedAbility + abilityCooldown)
            return false;

        return true;
    }

    // 능력 쿨다운 설정
    public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;

    // 점프 공격 충격 처리
    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint ?? transform;
        MassDamage(impactPoint.position, impactRadius, jumpAttackDamage);
    }

    // 주변에 피해를 입힘
    private void MassDamage(Vector3 impactPoint, float impactRadius, int damage)
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(impactPoint, impactRadius, ~whatIsAlly);

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null && uniqueEntities.Add(hit.transform.root.gameObject))
                damagable.TakeDamage(damage);

            ApplyPhysicalForceTo(impactPoint, impactRadius, hit);
        }
    }

    // 물리 충격 적용
    private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upforceMultiplier, ForceMode.Impulse);
    }

    // 점프 공격 가능 여부 체크
    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer >= minJumpDistanceRequired &&
               Time.time > lastTimeJumped + jumpAttackCooldown &&
               IsPlayerInClearSight();
    }

    // 점프 공격 쿨다운 설정
    public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;

    // 플레이어가 시야에 있는지 확인
    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - myPos).normalized;

        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIngore))
            return hit.transform.root == player.root;

        return false;
    }

    // 플레이어가 공격 범위 내에 있는지 확인
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    // 디버그용 기즈모 표시
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player != null)
        {
            Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
            Vector3 playerPos = player.position + Vector3.up;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(myPos, playerPos);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, minAbilityDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minJumpDistanceRequired);

        foreach (var damagePoint in damagePoints)
            Gizmos.DrawWireSphere(damagePoint.position, attackCheckRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(damagePoints[0].position, hummerCheckRadius);
    }
}
