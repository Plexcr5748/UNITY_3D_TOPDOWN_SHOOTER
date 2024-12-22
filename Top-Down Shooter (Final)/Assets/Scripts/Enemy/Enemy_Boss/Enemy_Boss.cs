using System.Collections.Generic;
using UnityEngine;

// ���� �� Ŭ����, �پ��� ����� �ɷ��� ����
public enum BossWeaponType { Flamethrower, Hummer }

public class Enemy_Boss : Enemy
{
    [Header("Boss details")]
    public BossWeaponType bossWeaponType; // ������ ���� ����
    public float actionCooldown = 10; // �ൿ ��ٿ�
    public float attackRange; // ������ ���� ����

    [Header("Ability")]
    public float minAbilityDistance; // �ɷ��� ����� �ּ� �Ÿ�
    public float abilityCooldown; // �ɷ� ��� ��ٿ�
    private float lastTimeUsedAbility; // ������ �ɷ� ��� �ð�

    [Header("Flamethrower")]
    public int flameDamage; // ȭ�� ���ط�
    public float flameDamageCooldown; // ȭ�� ���� ����
    public ParticleSystem flamethrower; // ȭ�� ����
    public float flamethrowDuration; // ȭ�� ��� �ð�
    public bool flamethrowActive { get; private set; } // ȭ�� ��� Ȱ�� ����

    [Header("Hummer")]
    public int hummerActiveDamage; // ��ġ ���ط�
    public GameObject activationPrefab; // ��ġ ȿ�� ������
    [SerializeField] private float hummerCheckRadius; // ��ġ ���� ����

    [Header("Jump attack")]
    public int jumpAttackDamage; // ���� ���� ���ط�
    public float jumpAttackCooldown = 10; // ���� ���� ��ٿ�
    private float lastTimeJumped; // ������ ���� ���� �ð�
    public float travelTimeToTarget = 1; // ���� ���� �ð�
    public float minJumpDistanceRequired; // ���� ���� �ּ� �Ÿ�
    [Space]
    public float impactRadius = 2.5f; // ���� ��� �ݰ�
    public float impactPower = 5; // ���� ��� ����
    public Transform impactPoint; // ��� �߽���
    [SerializeField] private float upforceMultiplier = 10; // ��� ���� �� ����
    [Space]
    [SerializeField] private LayerMask whatToIngore; // ������ ���̾�

    [Header("Attack")]
    [SerializeField] private int meleeAttackDamage; // ���� ���� ���ط�
    [SerializeField] private Transform[] damagePoints; // ���� ����
    [SerializeField] private float attackCheckRadius; // ���� üũ �ݰ�
    [SerializeField] private GameObject meleeAttackFx; // ���� ���� ȿ��

    // ���� ���� ����
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }
    public AttackState_Boss attackState { get; private set; }
    public JumpAttackState_Boss jumpAttackState { get; private set; }
    public AbilityState_Boss abilityState { get; private set; }
    public DeadState_Boss deadState { get; private set; }

    public Enemy_BossVisuals bossVisuals { get; private set; }

    // �ʱ�ȭ
    protected override void Awake()
    {
        base.Awake();
        bossVisuals = GetComponent<Enemy_BossVisuals>();

        // ���� �ʱ�ȭ
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
        attackState = new AttackState_Boss(this, stateMachine, "Attack");
        jumpAttackState = new JumpAttackState_Boss(this, stateMachine, "JumpAttack");
        abilityState = new AbilityState_Boss(this, stateMachine, "Ability");
        deadState = new DeadState_Boss(this, stateMachine, "Idle"); // Idle ���´� ��ü�� ragdoll ���
    }

    // ���� �ӽ� �ʱ�ȭ
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    // ���� ������Ʈ �� ���� üũ
    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        if (ShouldEnterBattleMode())
            EnterBattleMode();

        MeleeAttackCheck(damagePoints, attackCheckRadius, meleeAttackFx, meleeAttackDamage);
    }

    // ��� ó��
    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState);
    }

    // ���� ��� ����
    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(moveState);
    }

    // ȭ�� ���� Ȱ��ȭ/��Ȱ��ȭ
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

    // ��ġ Ȱ��ȭ
    public void ActivateHummer()
    {
        GameObject newActivation = ObjectPool.instance.GetObject(activationPrefab, impactPoint);
        ObjectPool.instance.ReturnObject(newActivation, 1);
        MassDamage(damagePoints[0].position, hummerCheckRadius, hummerActiveDamage);
    }

    // �ɷ� ��� ���� ���� üũ
    public bool CanDoAbility()
    {
        bool playerWithinDistance = Vector3.Distance(transform.position, player.position) < minAbilityDistance;

        if (!playerWithinDistance || Time.time <= lastTimeUsedAbility + abilityCooldown)
            return false;

        return true;
    }

    // �ɷ� ��ٿ� ����
    public void SetAbilityOnCooldown() => lastTimeUsedAbility = Time.time;

    // ���� ���� ��� ó��
    public void JumpImpact()
    {
        Transform impactPoint = this.impactPoint ?? transform;
        MassDamage(impactPoint.position, impactRadius, jumpAttackDamage);
    }

    // �ֺ��� ���ظ� ����
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

    // ���� ��� ����
    private void ApplyPhysicalForceTo(Vector3 impactPoint, float impactRadius, Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddExplosionForce(impactPower, impactPoint, impactRadius, upforceMultiplier, ForceMode.Impulse);
    }

    // ���� ���� ���� ���� üũ
    public bool CanDoJumpAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer >= minJumpDistanceRequired &&
               Time.time > lastTimeJumped + jumpAttackCooldown &&
               IsPlayerInClearSight();
    }

    // ���� ���� ��ٿ� ����
    public void SetJumpAttackOnCooldown() => lastTimeJumped = Time.time;

    // �÷��̾ �þ߿� �ִ��� Ȯ��
    public bool IsPlayerInClearSight()
    {
        Vector3 myPos = transform.position + new Vector3(0, 1.5f, 0);
        Vector3 playerPos = player.position + Vector3.up;
        Vector3 directionToPlayer = (playerPos - myPos).normalized;

        if (Physics.Raycast(myPos, directionToPlayer, out RaycastHit hit, 100, ~whatToIngore))
            return hit.transform.root == player.root;

        return false;
    }

    // �÷��̾ ���� ���� ���� �ִ��� Ȯ��
    public bool PlayerInAttackRange() => Vector3.Distance(transform.position, player.position) < attackRange;

    // ����׿� ����� ǥ��
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
