using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoverPerk { Unavalible, CanTakeCover, CanTakeAndChangeCover }
public enum UnstoppablePerk { Unavalible, Unstoppable }
public enum GrenadePerk { Unavalible, CanThrowGrenade }

// 원거리 적의 동작, 능력 및 상태를 관리하는 클래스
public class Enemy_Range : Enemy
{
    [Header("Enemy perks")]
    public Enemy_RangeWeaponType weaponType; // 원거리 무기 유형
    public CoverPerk coverPerk; // 엄폐물 관련 능력
    public UnstoppablePerk unstoppablePerk; // 방해 불가 능력
    public GrenadePerk grenadePerk; // 수류탄 던지기 능력

    [Header("Grenade perk")]
    public int grenadeDamage; // 수류탄 데미지
    public GameObject grenadePrefab; // 수류탄 프리팹
    public float impactPower; // 충격력
    public float explosionTimer = .75f; // 폭발 타이머
    public float timeToTarget = 1.2f; // 목표 도달 시간
    public float grenadeCooldown; // 수류탄 쿨다운 시간
    private float lastTimeGrenadeThrown = -10; // 마지막 수류탄 던진 시간
    [SerializeField] private Transform grenadeStartPoint; // 수류탄 시작 위치

    [Header("Advance perk")]
    public float advanceSpeed; // 전진 속도
    public float advanceStoppingDistance; // 전진 중단 거리
    public float advanceDuration = 2.5f; // 전진 지속 시간

    [Header("Cover system")]
    public float minCoverTime; // 최소 엄폐 시간
    public float safeDistance; // 안전 거리
    public CoverPoint currentCover { get; private set; } // 현재 엄폐 지점
    public CoverPoint lastCover { get; private set; } // 마지막 엄폐 지점

    [Header("Weapon details")]
    public float attackDelay; // 공격 지연 시간
    public Enemy_RangeWeaponData weaponData; // 원거리 무기 데이터

    [Space]
    public Transform gunPoint; // 총알 발사 위치
    public Transform weaponHolder; // 무기 보유 위치
    public GameObject bulletPrefab; // 총알 프리팹
    public Enemy_RangeWeaponModel weaponModel { get; private set; } // 원거리 무기 모델

    [Header("Aim details")]
    public float slowAim = 4; // 느린 조준 속도
    public float fastAim = 20; // 빠른 조준 속도
    public Transform aim; // 조준 위치
    public Transform playersBody; // 플레이어의 몸체
    public LayerMask whatToIgnore; // 무시할 레이어

    [SerializeField] List<Enemy_RangeWeaponData> avalibleWeaponData; // 사용 가능한 무기 데이터

    #region States
    public IdleState_Range idleState { get; private set; } // 대기 상태
    public MoveState_Range moveState { get; private set; } // 이동 상태
    public BattleState_Range battleState { get; private set; } // 전투 상태
    public RunToCoverState_Range runToCoverState { get; private set; } // 엄폐 상태
    public AdvancePlayerState_Range advancePlayerState { get; private set; } // 전진 상태
    public ThrowGrenadeState_Range throwGrenadeState { get; private set; } // 수류탄 던지기 상태
    public DeadState_Range deadState { get; private set; } // 사망 상태
    #endregion

    protected override void Awake()
    {
        base.Awake();

        // 상태 초기화
        idleState = new IdleState_Range(this, stateMachine, "Idle");
        moveState = new MoveState_Range(this, stateMachine, "Move");
        battleState = new BattleState_Range(this, stateMachine, "Battle");
        runToCoverState = new RunToCoverState_Range(this, stateMachine, "Run");
        advancePlayerState = new AdvancePlayerState_Range(this, stateMachine, "Advance");
        throwGrenadeState = new ThrowGrenadeState_Range(this, stateMachine, "ThrowGrenade");
        deadState = new DeadState_Range(this, stateMachine, "Idle"); // Idle 애니메이션을 플레이스홀더로 사용
    }

    protected override void Start()
    {
        base.Start();

        playersBody = player.GetComponent<Player>().playerBody; // 플레이어의 몸체 참조
        aim.parent = null; // 조준 위치의 부모 설정 해제

        InitializePerk(); // 특성 초기화

        stateMachine.Initialize(idleState); // 초기 상태 설정
        visuals.SetupLook(); // 시각적 설정
        SetupWeapon(); // 무기 설정
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update(); // 현재 상태 업데이트
    }

    public override void Die()
    {
        base.Die();

        if (stateMachine.currentState != deadState)
            stateMachine.ChangeState(deadState); // 사망 상태로 전환
    }

    public bool CanThrowGrenade()
    {
        // 수류탄을 던질 수 있는지 확인
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
        // 수류탄 던지기
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
        // 특성 초기화
        if (weaponType == Enemy_RangeWeaponType.Random)
        {
            ChooseRandomWeaponType();
        }

        if (IsUnstopppable())
        {
            advanceSpeed = 1;
            anim.SetFloat("AdvanceAnimIndex", 1); // 느린 걷기 애니메이션
        }
    }

    private void ChooseRandomWeaponType()
    {
        // 무작위 무기 유형 선택
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
        // 전투 모드 진입
        if (inBattleMode)
            return;

        base.EnterBattleMode();

        if (CanGetCover())
        {
            stateMachine.ChangeState(runToCoverState); // 엄폐 상태로 전환
        }
        else
            stateMachine.ChangeState(battleState); // 전투 상태로 전환
    }

    #region Cover System

    public bool CanGetCover()
    {
        // 엄폐 가능 여부 확인
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
        // 가장 가까운 엄폐 지점 찾기
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
        // 근처 엄폐 지점 수집
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
        // 총알 발사
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
        // 무기 데이터 설정
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
        // 조준 위치 업데이트
        float aimSpeed = IsAimOnPlayer() ? fastAim : slowAim;
        aim.position = Vector3.MoveTowards(aim.position, playersBody.position, aimSpeed * Time.deltaTime);
    }

    public bool IsAimOnPlayer()
    {
        // 플레이어를 조준 중인지 확인
        float distnaceAimToPlayer = Vector3.Distance(aim.position, player.position);

        return distnaceAimToPlayer < 2;
    }

    public bool IsSeeingPlayer()
    {
        // 플레이어를 볼 수 있는지 확인
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

    public bool IsUnstopppable() => unstoppablePerk == UnstoppablePerk.Unstoppable; // 방해 불가 여부 확인

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, advanceStoppingDistance); // 전진 중단 거리 표시
    }
}
