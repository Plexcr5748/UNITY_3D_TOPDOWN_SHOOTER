using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// �� ���� ���� �����͸� �����ϴ� ����ü
public struct AttackData_EnemyMelee
{
    public int attackDamage; // ���� ������
    public string attackName; // ���� �̸�
    public float attackRange; // ���� ����
    public float moveSpeed; // �̵� �ӵ�
    public float attackIndex; // ���� �ε���
    [Range(1, 2)]
    public float animationSpeed; // �ִϸ��̼� �ӵ�
    public AttackType_Melee attackType; // ���� Ÿ��
}
public enum AttackType_Melee { Close, Charge } // ���� ���� Ÿ��
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow } // �� ����

public class Enemy_Melee : Enemy
{
    public Enemy_MeleeSFX meleeSFX { get; private set; } // ���� ���� ȿ����

    #region States
    public IdleState_Melee idleState { get; private set; } // ��� ����
    public MoveState_Melee moveState { get; private set; } // �̵� ����
    public RecoveryState_Melee recoveryState { get; private set; } // ȸ�� ����
    public ChaseState_Melee chaseState { get; private set; } // ���� ����
    public AttackState_Melee attackState { get; private set; } // ���� ����
    public DeadState_Melee deadState { get; private set; } // ��� ����
    public AbilityState_Melee abilityState { get; private set; } // �ɷ� ��� ����
    #endregion

    [Header("Enemy Settings")]
    public EnemyMelee_Type meleeType; // �� ����
    public Enemy_MeleeWeaponType weaponType; // ���� ����

    [Header("Shield")]
    public int shieldDurability; // ���� ������
    public Transform shieldTransform; // ���� ��ġ

    [Header("Dodge")]
    public float dodgeCooldown; // ȸ�� ��ٿ�
    private float lastTimeDodge = -10; // ������ ȸ�� �ð�

    [Header("Axe throw ability")]
    public int axeDamage; // ���� ������
    public GameObject axePrefab; // ���� ������
    public float axeFlySpeed; // ���� ���� �ӵ�
    public float axeAimTimer; // ���� ���� �ð�
    public float axeThrowCooldown; // ���� ������ ��ٿ�
    private float lastTimeAxeThrown; // ������ ���� ������ �ð�
    public Transform axeStartPoint; // ���� ���� ��ġ

    [Header("Attack Data")]
    public AttackData_EnemyMelee attackData; // �⺻ ���� ������
    public List<AttackData_EnemyMelee> attackList; // ���� ���� ������
    private Enemy_WeaponModel currentWeapon; // ���� ���� ��
    private bool isAttackReady; // ���� �غ� ����
    [Space]
    [SerializeField] private GameObject meleeAttackFx; // ���� ���� ȿ��

    protected override void Awake()
    {
        base.Awake();

        // ���� �ʱ�ȭ
        idleState = new IdleState_Melee(this, stateMachine, "Idle");
        moveState = new MoveState_Melee(this, stateMachine, "Move");
        recoveryState = new RecoveryState_Melee(this, stateMachine, "Recovery");
        chaseState = new ChaseState_Melee(this, stateMachine, "Chase");
        attackState = new AttackState_Melee(this, stateMachine, "Attack");
        deadState = new DeadState_Melee(this, stateMachine, "Idle");
        abilityState = new AbilityState_Melee(this, stateMachine, "AxeThrow");

        meleeSFX = GetComponent<Enemy_MeleeSFX>(); // ȿ���� �ʱ�ȭ
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // �ʱ� ���� ����
        ResetCooldown(); // ��ٿ� �ʱ�ȭ

        InitializePerk(); // Ư�� �ʱ�ȭ
        visuals.SetupLook(); // ���־� ����
        UpdateAttackData(); // ���� ������ ������Ʈ
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update(); // ���� ���� ������Ʈ

        // ���� ���� ����
        MeleeAttackCheck(currentWeapon.damagePoints, currentWeapon.attackRadius, meleeAttackFx, attackData.attackDamage);
    }

    public override void EnterBattleMode()
    {
        if (inBattleMode)
            return;

        base.EnterBattleMode();
        stateMachine.ChangeState(recoveryState); // ȸ�� ���·� ��ȯ
    }

    public override void AbilityTrigger()
    {
        base.AbilityTrigger();

        walkSpeed = walkSpeed * .6f; // �ȱ� �ӵ� ����
        visuals.EnableWeaponModel(false); // ���� �� ��Ȱ��ȭ
    }

    public void UpdateAttackData()
    {
        // ���� ���� ������ ������Ʈ
        currentWeapon = visuals.currentWeaponModel.GetComponent<Enemy_WeaponModel>();

        if (currentWeapon.weaponData != null)
        {
            attackList = new List<AttackData_EnemyMelee>(currentWeapon.weaponData.attackData);
            turnSpeed = currentWeapon.weaponData.turnSpeed; // ȸ�� �ӵ� ������Ʈ
        }
    }

    protected override void InitializePerk()
    {
        // Ư�� �� ������ ���� Ư�� �ʱ�ȭ
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
            stateMachine.ChangeState(deadState); // ��� ���·� ��ȯ
    }

    public void ActivateDodgeRoll()
    {
        // ȸ�� ���� Ȱ��ȭ
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
        // ���� ������
        GameObject newAxe = ObjectPool.instance.GetObject(axePrefab, axeStartPoint);

        newAxe.GetComponent<Enemy_Axe>().AxeSetup(axeFlySpeed, player, axeAimTimer, axeDamage);
    }

    public bool CanThrowAxe()
    {
        // ���� ���� �� �ִ��� Ȯ��
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
        // ��ٿ� �ʱ�ȭ
        lastTimeDodge -= dodgeCooldown;
        lastTimeAxeThrown -= axeThrowCooldown;
    }

    private float GetAnimationClipDuration(string clipName)
    {
        // �ִϸ��̼� Ŭ�� ���� ��ȯ
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
        Gizmos.DrawWireSphere(transform.position, attackData.attackRange); // ���� ���� ǥ��
    }
}
