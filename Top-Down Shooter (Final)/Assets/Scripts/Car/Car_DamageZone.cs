using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자동차의 충돌 영역에서 피해를 처리하는 클래스

public class Car_DamageZone : MonoBehaviour
{
    private Car_Controller carController; // 자동차 컨트롤러 참조

    [SerializeField] private float minSpeedToDamage = 1.5f; // 피해를 입히기 위한 최소 속도
    [SerializeField] private int carDamage; // 피해량
    [SerializeField] private float impactForce = 150; // 충격력
    [SerializeField] private float upwardsMultiplier = 3; // 충격의 상향 보정

    private void Awake()
    {
        carController = GetComponentInParent<Car_Controller>(); // 부모 오브젝트에서 Car_Controller 가져오기
    }

    private void OnTriggerEnter(Collider other)
    {
        if (carController.rb.velocity.magnitude < minSpeedToDamage)
            return; // 자동차 속도가 최소 피해 속도보다 낮으면 종료

        IDamagable damagable = other.GetComponent<IDamagable>(); // 충돌 객체가 피해를 받을 수 있는지 확인
        if (damagable == null)
            return; // 피해를 받을 수 없는 객체면 종료

        damagable.TakeDamage(carDamage); // 피해를 입힘

        Rigidbody rb = other.GetComponent<Rigidbody>(); // 충돌 객체의 Rigidbody 가져오기
        if (rb != null)
            ApplyForce(rb); // Rigidbody가 있다면 힘을 가함
    }

    private void ApplyForce(Rigidbody rigidbody)
    {
        rigidbody.isKinematic = false; // 물리적으로 동작하도록 설정
        rigidbody.AddExplosionForce(impactForce, transform.position, 3, upwardsMultiplier, ForceMode.Impulse);
        // 폭발력 추가 (위치, 반경, 상향 힘 보정, 힘 적용 모드)
    }
}
