using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage; // 총알의 데미지
    private float impactForce; // 충돌 시 적용되는 힘

    private BoxCollider cd; // 총알의 콜라이더
    private Rigidbody rb; // 총알의 리지드바디
    private MeshRenderer meshRenderer; // 총알의 시각적 렌더링을 담당하는 컴포넌트
    private TrailRenderer trailRenderer; // 총알의 잔상 효과를 담당하는 컴포넌트

    [SerializeField] private GameObject bulletImpactFX; // 총알 충돌 시 생성될 이펙트

    private Vector3 startPosition; // 총알의 시작 위치
    private float flyDistance; // 총알이 비행 가능한 거리
    private bool bulletDisabled; // 총알 비활성화 상태를 추적

    private LayerMask allyLayerMask; // 아군의 레이어 마스크

    protected virtual void Awake()
    {
        // 필요한 컴포넌트 초기화
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(LayerMask allyLayerMask, int bulletDamage, float flyDistance = 100, float impactForce = 100)
    {
        // 총알 초기 설정
        this.allyLayerMask = allyLayerMask;
        this.impactForce = impactForce;
        this.bulletDamage = bulletDamage;

        bulletDisabled = false; // 총알을 활성화 상태로 설정
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear(); // 잔상 초기화
        trailRenderer.time = .25f; // 잔상 지속 시간 설정
        startPosition = transform.position; // 총알 시작 위치 저장
        this.flyDistance = flyDistance + .5f; // 거리 설정 (+0.5는 레이저 길이 보정)
    }

    protected virtual void Update()
    {
        // 각종 상태를 매 프레임마다 확인
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        // 잔상이 모두 사라지면 총알을 풀로 반환
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    protected void DisableBulletIfNeeded()
    {
        // 비행 거리를 초과하면 총알을 비활성화
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        // 비행 종료가 가까워지면 잔상을 점차 제거
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; // 테스트로 설정된 값
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // 충돌 이벤트 처리
        if (FriendlyFare() == false)
        {
            // 충돌한 객체가 아군인지 확인
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10); // 아군이면 총알을 풀로 반환
                return;
            }
        }

        CreateImpactFx(); // 충돌 이펙트 생성
        ReturnBulletToPool(); // 총알 반환

        // 충돌 대상이 데미지 받을 수 있는지 확인
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(bulletDamage);

        ApplyBulletImpactToEnemy(collision); // 적에 충격 적용
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        // 적 객체에 충돌로 인한 힘 적용
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce; // 충돌 방향에 따라 힘 계산
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;
            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody); // 적에게 충돌 처리 전달
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        // 총알을 오브젝트 풀로 반환
        ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected void CreateImpactFx()
    {
        // 충돌 이펙트 생성
        GameObject newFx = Instantiate(bulletImpactFX);
        newFx.transform.position = transform.position;

        Destroy(newFx, 1); // 1초 후 이펙트 제거
    }

    private bool FriendlyFare()
    {
        // 아군 공격 여부 확인
        return GameManager.instance.friendlyFire;
    }
}
