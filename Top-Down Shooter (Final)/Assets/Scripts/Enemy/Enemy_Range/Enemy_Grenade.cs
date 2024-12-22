using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 수류탄을 관리하는 클래스
public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx; // 폭발 효과
    [SerializeField] private float impactRadius; // 폭발 반경
    [SerializeField] private float upwardsMultiplier = 1; // 폭발 시 위로 가하는 힘의 배율
    private Rigidbody rb; // 수류탄의 Rigidbody
    private float timer; // 수류탄 폭발 타이머
    private float impactPower; // 폭발의 물리적 힘

    private LayerMask allyLayerMask; // 아군 레이어 마스크
    private bool canExplode = true; // 폭발 가능 여부

    private int grenadeDamage; // 수류탄의 데미지

    // 초기화
    private void Awake() => rb = GetComponent<Rigidbody>();

    // 매 프레임 타이머를 업데이트하고 조건에 따라 폭발
    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && canExplode)
            Explode();
    }

    // 폭발 처리
    private void Explode()
    {
        canExplode = false;
        PlayExplosionFx(); // 폭발 효과 재생

        // 중복 타겟 방지를 위한 해시셋
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();

            if (damagable != null)
            {
                // 타겟 검증 실패 시 무시
                if (IsTargetValid(hit) == false)
                    continue;

                // 중복 처리 방지
                GameObject rootEntity = hit.transform.root.gameObject;
                if (uniqueEntities.Add(rootEntity) == false)
                    continue;

                // 데미지 처리
                damagable.TakeDamage(grenadeDamage);
            }

            // 물리적 폭발 힘 적용
            ApplyPhysicalForceTo(hit);
        }

        // 객체 풀에 반환
        ObjectPool.instance.ReturnObject(gameObject);
    }

    // 충돌체에 물리적 폭발 힘을 적용
    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody rb = hit.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false; // 강체를 물리적으로 활성화
            rb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
        }
    }

    // 폭발 효과 재생
    private void PlayExplosionFx()
    {
        GameObject newFx = ObjectPool.instance.GetObject(explosionFx, transform); // 폭발 효과 생성
        ObjectPool.instance.ReturnObject(newFx, 1); // 일정 시간 후 효과 반환
        ObjectPool.instance.ReturnObject(gameObject); // 수류탄 반환
    }

    // 수류탄 설정
    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower, int grenadeDamage)
    {
        canExplode = true;

        this.grenadeDamage = grenadeDamage;
        this.allyLayerMask = allyLayerMask;
        rb.velocity = CalculateLaunchVelocity(target, timeToTarget); // 발사 속도 계산
        timer = countdown + timeToTarget; // 폭발 타이머 설정
        this.impactPower = impactPower;
    }

    // 타겟이 유효한지 확인
    private bool IsTargetValid(Collider collider)
    {
        // 아군 피해(friendly fire)가 활성화된 경우 모든 타겟이 유효
        if (GameManager.instance.friendlyFire)
            return true;

        // 아군 레이어에 포함된 경우 유효하지 않음
        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
            return false;

        return true;
    }

    // 수류탄 발사 속도 계산
    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position; // 타겟 방향
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z); // 수평 방향 분리

        Vector3 velocityXZ = directionXZ / timeToTarget; // 수평 속도 계산

        // 수직 속도 계산
        float velocityY =
            (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY; // 발사 속도 결합

        return launchVelocity;
    }

    // 폭발 반경을 시각적으로 표시 (디버깅 용도)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
