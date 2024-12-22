using UnityEngine;

// 적의 도끼 투척을 관리하는 클래스
public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx; // 충돌 효과
    [SerializeField] private Rigidbody rb; // 도끼의 Rigidbody
    [SerializeField] private Transform axeVisual; // 도끼의 시각적 요소

    private Vector3 direction; // 도끼가 날아갈 방향
    private Transform player; // 플레이어의 위치
    private float flySpeed; // 도끼의 비행 속도
    private float rotationSpeed; // 도끼의 회전 속도
    private float timer = 1; // 비행 지속 시간

    private int damage; // 도끼의 피해량

    // 도끼 설정 메서드
    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        rotationSpeed = 1600; // 기본 회전 속도 설정

        this.damage = damage; // 피해량 설정
        this.flySpeed = flySpeed; // 비행 속도 설정
        this.player = player; // 플레이어 참조 설정
        this.timer = timer; // 비행 지속 시간 설정
    }

    // 매 프레임 도끼 회전 및 방향 설정
    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime); // 도끼 회전
        timer -= Time.deltaTime; // 타이머 감소

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position; // 타겟 방향 설정

        transform.forward = rb.velocity; // 도끼의 진행 방향 설정
    }

    // 물리 업데이트에서 도끼 이동 처리
    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * flySpeed; // 방향에 따라 속도 적용
    }

    // 충돌 이벤트 처리
    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // 충돌 대상이 피해를 받을 수 있는지 확인
        damagable?.TakeDamage(damage); // 데미지 적용

        // 충돌 효과 생성
        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);

        // 도끼와 효과 객체를 풀로 반환
        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }
}
