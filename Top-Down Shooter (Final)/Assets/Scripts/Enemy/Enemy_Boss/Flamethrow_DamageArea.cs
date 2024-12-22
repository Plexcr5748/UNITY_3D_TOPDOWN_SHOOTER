using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 화염방사기의 피해 영역을 관리하는 클래스
public class Flamethrow_DamageArea : MonoBehaviour
{
    private Enemy_Boss enemy; // 보스 객체 참조

    private float damageCooldown; // 피해 적용 간격
    private float lastTimeDamaged; // 마지막으로 피해를 적용한 시간
    private int flameDamage; // 화염 피해량

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>(); // 부모 객체에서 보스 컴포넌트 가져오기
        damageCooldown = enemy.flameDamageCooldown; // 피해 간격 초기화
        flameDamage = enemy.flameDamage; // 화염 피해량 초기화
    }

    private void OnTriggerStay(Collider other)
    {
        // 화염 방사기가 활성화되지 않은 경우 리턴
        if (!enemy.flamethrowActive)
            return;

        // 피해 간격 내에서는 피해를 적용하지 않음
        if (Time.time - lastTimeDamaged < damageCooldown)
            return;

        // 피해를 입힐 수 있는 대상인지 확인
        IDamagable damagable = other.GetComponent<IDamagable>();

        if (damagable != null)
        {
            damagable.TakeDamage(flameDamage); // 대상에게 피해 적용
            lastTimeDamaged = Time.time; // 마지막 피해 시간 갱신
            damageCooldown = enemy.flameDamageCooldown; // 피해 간격 갱신 (테스트 용이)
        }
    }
}
