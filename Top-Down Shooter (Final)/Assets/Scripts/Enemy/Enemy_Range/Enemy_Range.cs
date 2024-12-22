using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { Unavalible, CanTakeCover, CanTakeAndChangeCover }
public enum UnstoppablePerk { Unavalible, Unstoppable }
public enum GrenadePerk { Unavalible, CanThrowGrenade }

// ���Ÿ� ���� ����, �ɷ� �� ���¸� �����ϴ� Ŭ����
public class Enemy_Range : Enemy
{
    [Header("Enemy perks")]
    public Enemy_RangeWeaponType weaponType; // ���Ÿ� ���� ����
    public CoverPerk coverPerk; // ���� ���� �ɷ�
    public UnstoppablePerk unstoppablePerk; // ���� �Ұ� �ɷ�
    public GrenadePerk grenadePerk; // ����ź ������ �ɷ�

    [Header("Grenade perk")]
    public int grenadeDamage; // ����ź ������
    public GameObject grenadePrefab; // ����ź ������
    public float impactPower; // ��ݷ�
    public float explosionTimer = .75f; // ���� Ÿ�̸�
    public float timeToTarget = 1.2f; // ��ǥ ���� �ð�
    public float grenadeCooldown; // ����ź ��ٿ� �ð�
    private float lastTimeGrenadeThrown = -10; // ������ ����ź ���� �ð�
    [SerializeField] private Transform grenadeStartPoint; // ����ź ���� ��ġ

    [Header("Advance perk")]
    public float advanceSpeed; // ���� �ӵ�
    public float advanceStoppingDistance; // ���� �ߴ� �Ÿ�
    public float advanceDuration = 2.5f; // ���� ���� �ð�

    [Header("Cover system")]
    public float minCoverTime; // �ּ� ���� �ð�
    public float safeDistance; // ���� �Ÿ�
    public CoverPoint currentCover { get; private set; } // ���� ���� ����
    public CoverPoint lastCover { get; private set; } // ������ ���� ����

    [Header("Weapon details")]
    public float attackDelay; // ���� ���� �ð�
    public Enemy_RangeWeaponData weaponData; // ���Ÿ� ���� ������

    [Space]
    public Transform gunPoint; // �Ѿ� �߻� ��ġ
    public Transform weaponHolder; // ���� ���� ��ġ
    public GameObject bulletPrefab; // �Ѿ� ������
    public Enemy_RangeWeaponModel weaponModel { get; private set; } // ���Ÿ� ���� ��

    [Header("Aim details")]
    public float slowAim = 4; // ���� ���� �ӵ�
    public float fastAim = 20; // ���� ���� �ӵ�
    public Transform aim; // ���� ��ġ
    public Transform playersBody; // �÷��̾��� ��ü
    public LayerMask whatToIgnore; // ������ ���̾�

    [SerializeField] List<Enemy_RangeWeaponData> avalibleWeaponData; // ��� ������ ���� ������

    #region States
    public IdleState_Range idleState { get; private set; } // ��� ����
    public MoveState_Range moveState { get; private set; } // �̵� ����
    public BattleState_Range battleState { get; private set; } // ���� ����
    public RunToCoverState_Range runToCoverState { get; private set; } // ���� ����
    public AdvancePlayerState_Range advancePlayerState { get; private set; } // ���� ����
    public ThrowGrenadeState_Range throwGrenadeState { get; private set; } // ����ź ������ ����
    public DeadState_Range deadState { get; private set; } // ��� ����
    #endregion

    protected override void Awake()
    {
        base.Awake();

        // ���� �ʱ�ȭ
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
        deadState = new DeadState_Range(this, stateMachine, "Idle"); // Idle �ִϸ��̼��� �÷��̽�Ȧ���� ���
    }

    protected override void Start()
    {
        base.Start();

        playersBody = player.GetComponent<Player>().playerBody; // �÷��̾��� ��ü ����
        aim.parent = null; // ���� ��ġ�� �θ� ���� ����

        InitializePerk(); // Ư�� �ʱ�ȭ

        stateMachine.Initialize(idleState); // �ʱ� ���� ����
        visuals.SetupLook(); // �ð��� ����
        SetupWeapon(); // ���� ����
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update(); // ���� ���� ������Ʈ
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState); // ��� ���·� ��ȯ
    }

    public bool CanThrowGrenade()
    {
        // ����ź�� ���� �� �ִ��� Ȯ��
        if (grenadePerk == GrenadePerk.Unavalible)
            return false;

        if (Vector3.Distance(player.transform.position, transform.position) < safeDistance)
            return false;

        if (Time.time > grenadeCooldown + lastTimeGrenadeThrown)
            return true;

        return false;
    }

    public void ThrowGrenade()
    {
        // ����ź ������
        lastTimeGrenadeThrown = Time.time;
        visuals.EnableGrenadeModel(false);

        GameObject newGrenade = ObjectPool.instance.GetObject(grenadePrefab, grenadeStartPoint);
        Enemy_Grenade newGrenadeScript = newGrenade.GetComponent<Enemy_Grenade>();

        if (stateMachine.currentState == deadState)
        {
            newGrenadeScript.SetupGrenade(whatIsAlly, transform.position, 1, explosionTimer, impactPower, grenadeDamage);
            return;
        }

        newGrenadeScript.SetupGrenade(whatIsAlly, player.transform.position, timeToTarget, explosionTimer, impactPower, grenadeDamage);
    }

    protected override void InitializePerk()
    {
        // Ư�� �ʱ�ȭ
        if (weaponType == Enemy_RangeWeaponType.Random)
        {
            ChooseRandomWeaponType();
        }

        if (IsUnstopppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1); // ���� �ȱ� �ִϸ��̼�
        }
    }

    private void ChooseRandomWeaponType()
    {
        // ������ ���� ���� ����
        List<Enemy_RangeWeaponType> validTypes = new List<Enemy_RangeWeaponType>();

        foreach (Enemy_RangeWeaponType value in Enum.GetValues(typeof(Enemy_RangeWeaponType)))
        {
            if (value != Enemy_RangeWeaponType.Random && value != Enemy_RangeWeaponType.Rifle)
                validTypes.Add(value);
        }

        int randomIndex = UnityEngine.Random.Range(0, validTypes.Count);
        weaponType = validTypes[randomIndex];
    }

    public override void EnterBattleMode()
    {
        // ���� ��� ����
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if (CanGetCover())
        {
            stateMachine.ChangeState(runToCoverState); // ���� ���·� ��ȯ
        }
        else
            stateMachine.ChangeState(battleState); // ���� ���·� ��ȯ
    }

    #region Cover System

    public bool CanGetCover()
    {
        // ���� ���� ���� Ȯ��
        if (coverPerk == CoverPerk.Unavalible)
            return false;

        currentCover = AttemptToFindCover()?.GetComponent<CoverPoint>();

        if (lastCover != currentCover && currentCover != null)
            return true;

        Debug.LogWarning("No cover found!");
        return false;
    }

    private Transform AttemptToFindCover()
    {
        // ���� ����� ���� ���� ã��
        List<CoverPoint> collectedCoverPoints = new List<CoverPoint>();

        foreach (Cover cover in CollectNearByCovers())
        {
            collectedCoverPoints.AddRange(cover.GetValidCoverPoints(transform));
        }

        CoverPoint closestCoverPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (CoverPoint coverPoint in collectedCoverPoints)
        {
            float currentDistance = Vector3.Distance(transform.position, coverPoint.transform.position);
            if (currentDistance < shortestDistance)
            {
                closestCoverPoint = coverPoint;
                shortestDistance = currentDistance;
            }
        }

        if (closestCoverPoint != null)
        {
            lastCover?.SetOccupied(false);
            lastCover = currentCover;

            currentCover = closestCoverPoint;
            currentCover.SetOccupied(true);

            return currentCover.transform;
        }

        return null;
    }

    private List<Cover> CollectNearByCovers()
    {
        // ��ó ���� ���� ����
        float coverRadiusCheck = 30;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coverRadiusCheck);
        List<Cover> collectedCovers = new List<Cover>();

        foreach (Collider collider in hitColliders)
        {
            Cover cover = collider.GetComponent<Cover>();

            if (cover != null && collectedCovers.Contains(cover) == false)
                collectedCovers.Add(cover);
        }

        return collectedCovers;
    }

    #endregion

    public void FireSingleBullet()
    {
        // �Ѿ� �߻�
        anim.SetTrigger("Shoot");

        Vector3 bulletsDirection = (aim.position - gunPoint.position).normalized;

        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab, gunPoint);
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);

        newBullet.GetComponent<Bullet>().BulletSetup(whatIsAlly, weaponData.bulletDamage);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Vector3 bulletDirectionWithSpread = weaponData.ApplyWeaponSpread(bulletsDirection);

        rbNewBullet.mass = 20 / weaponData.bulletSpeed;
        rbNewBullet.velocity = bulletDirectionWithSpread * weaponData.bulletSpeed;
    }

    private void SetupWeapon()
    {
        // ���� ������ ����
        List<Enemy_RangeWeaponData> filteredData = new List<Enemy_RangeWeaponData>();

        foreach (var weaponData in avalibleWeaponData)
        {
            if (weaponData.weaponType == weaponType)
                filteredData.Add(weaponData);
        }

        if (filteredData.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, filteredData.Count);
            weaponData = filteredData[random];
        }
        else
            Debug.LogWarning("No available weapon data was found!");

        gunPoint = visuals.currentWeaponModel.GetComponent<Enemy_RangeWeaponModel>().gunPoint;
    }

    #region Enemy's aim region

    public void UpdateAimPosition()
    {
        // ���� ��ġ ������Ʈ
        float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;
        aim.position = Vector3.MoveTowards(aim.position, playersBody.position, aimSpeed * Time.deltaTime);
    }

    public bool IsAimOnPlayer()
    {
        // �÷��̾ ���� ������ Ȯ��
        float distnaceAimToPlayer = Vector3.Distance(aim.position, player.position);

        return distnaceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        // �÷��̾ �� �� �ִ��� Ȯ��
        Vector3 myPosition = transform.position + Vector3.up;
        Vector3 directionToPlayer = playersBody.position - myPosition;

        if (Physics.Raycast(myPosition, directionToPlayer, out RaycastHit hit, Mathf.Infinity, ~whatToIgnore))
        {
            if (hit.transform.root == player.root)
            {
                UpdateAimPosition();
                return true;
            }
        }

        return false;
    }

    #endregion

    public bool IsUnstopppable() => unstoppablePerk == UnstoppablePerk.Unstoppable; // ���� �Ұ� ���� Ȯ��

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, advanceStoppingDistance); // ���� �ߴ� �Ÿ� ǥ��
    }
}
