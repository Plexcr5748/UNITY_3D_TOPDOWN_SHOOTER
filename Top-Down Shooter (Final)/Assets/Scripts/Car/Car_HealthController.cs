// 자동차의 체력과 폭발 관련 동작을 관리하는 클래스

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamagable
{
    private Car_Controller carController; // 자동차 컨트롤러 참조

    public int maxHealth; // 최대 체력
    public int currentHealth; // 현재 체력

    private bool carBroken; // 자동차가 파손 상태인지 여부

    [Header("Explosion Info")]
    [SerializeField] private int explosionDamage = 350; // 폭발로 가하는 피해량
    [Space]
    [SerializeField] private float explosionRadius = 3; // 폭발 반경
    [SerializeField] private float explosionDelay = 3; // 폭발 지연 시간
    [SerializeField] private float explosionForce = 7; // 폭발 충격력
    [SerializeField] private float explosionUpwardsModifer = 2; // 폭발의 상향 힘 조정
    [SerializeField] private Transform explosionPoint; // 폭발 발생 위치
    [Space]
    [SerializeField] private ParticleSystem fireFx; // 불꽃 효과
    [SerializeField] private ParticleSystem explosionFx; // 폭발 효과

    [Header("Explosion Sound")]
    [SerializeField] private AudioSource explosionAudioSource; // 폭발 소리를 재생할 AudioSource
    [SerializeField] private AudioClip explosionSoundClip; // 폭발 사운드 클립

    private void Start()
    {
        carController = GetComponent<Car_Controller>(); // Car_Controller 컴포넌트 가져오기
        currentHealth = maxHealth; // 현재 체력을 최대 체력으로 초기화

        // 폭발 오디오 소스가 없을 경우 컴포넌트에서 가져오기
        if (explosionAudioSource == null)
        {
            explosionAudioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 불꽃 효과가 활성화된 경우 회전을 고정
        if (fireFx.gameObject.activeSelf)
        {
            fireFx.transform.rotation = Quaternion.identity;
        }
    }

    public void UpdateCarHealthUI()
    {
        // UI에 체력 정보 업데이트
        UI.instance.inGameUI.UpdateCarHealthUI(currentHealth, maxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return; // 이미 파손된 경우 체력 감소 무시

        currentHealth -= damage; // 피해량만큼 체력 감소

        if (currentHealth < 0)
            BrakeTheCar(); // 체력이 0 이하가 되면 자동차 파손 처리
    }

    private void BrakeTheCar()
    {
        carBroken = true; // 자동차 파손 상태 설정
        carController.BrakeTheCar(); // 자동차 정지 처리

        fireFx.gameObject.SetActive(true); // 불꽃 효과 활성화
        StartCoroutine(ExplosionCo(explosionDelay)); // 폭발 처리 코루틴 실행
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage); // 체력 감소 처리
        UpdateCarHealthUI(); // UI 업데이트
    }

    private IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay); // 폭발 지연

        explosionFx.gameObject.SetActive(true); // 폭발 효과 활성화
        carController.rb.AddExplosionForce(explosionForce, explosionPoint.position,
            explosionRadius, explosionUpwardsModifer, ForceMode.Impulse); // 폭발 물리 효과 적용

        Explode(); // 폭발 피해 처리

        PlayExplosionSound(); // 폭발 사운드 재생
    }

    private void Explode()
    {
        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>(); // 중복 처리 방지를 위한 집합

        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius); // 폭발 반경 내 모든 충돌체 가져오기

        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>(); // 피해를 받을 수 있는 객체 확인

            if (damagable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject; // 루트 객체 가져오기

                if (uniqueEntities.Add(rootEntity) == false)
                    continue; // 이미 처리한 객체는 무시

                damagable.TakeDamage(explosionDamage); // 폭발 피해 적용

                hit.GetComponentInChildren<Rigidbody>()?.AddExplosionForce(
                    explosionForce, explosionPoint.position, explosionRadius,
                    explosionUpwardsModifer, ForceMode.VelocityChange); // 폭발 충격력 적용
            }
        }
    }

    private void PlayExplosionSound()
    {
        // 폭발 사운드가 유효한 경우 재생
        if (explosionSoundClip != null && explosionAudioSource != null)
        {
            explosionAudioSource.PlayOneShot(explosionSoundClip);
        }
    }

    private void OnDrawGizmos()
    {
        // 폭발 반경을 기즈모로 표시
        Gizmos.DrawWireSphere(explosionPoint.position, explosionRadius);
    }
}
