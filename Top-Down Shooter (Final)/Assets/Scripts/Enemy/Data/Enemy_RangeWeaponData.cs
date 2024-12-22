using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원거리 무기 데이터를 정의하는 ScriptableObject
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Range weapon Data")]
public class Enemy_RangeWeaponData : ScriptableObject
{
    [Header("Weapon details")]
    public Enemy_RangeWeaponType weaponType; // 무기 유형 (피스톨, 리볼버 등)
    public float fireRate = 1f; // 발사 속도 (초당 총알 수)

    public int minBulletsPerAttack = 1; // 공격당 최소 발사 총알 수
    public int maxBulletsPerAttack = 1; // 공격당 최대 발사 총알 수

    public float minWeaponCooldown = 2; // 최소 무기 재사용 대기 시간
    public float maxWeaponCooldown = 3; // 최대 무기 재사용 대기 시간

    [Header("Bullet details")]
    public int bulletDamage; // 총알 데미지
    [Space]
    public float bulletSpeed = 20; // 총알 속도
    public float weaponSpread = .1f; // 무기의 산탄 범위

    // 공격당 발사할 총알 수를 랜덤으로 결정
    public int GetBulletsPerAttack() => Random.Range(minBulletsPerAttack, maxBulletsPerAttack + 1);

    // 무기의 재사용 대기 시간을 랜덤으로 결정
    public float GetWeaponCooldown() => Random.Range(minWeaponCooldown, maxWeaponCooldown);

    // 무기의 산탄 효과를 적용하여 방향 벡터를 수정
    public Vector3 ApplyWeaponSpread(Vector3 originalDirection)
    {
        float randomizedValue = Random.Range(-weaponSpread, weaponSpread); // 산탄 범위 내에서 랜덤 값 생성
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue / 2, randomizedValue); // 산탄 회전값 생성

        return spreadRotation * originalDirection; // 원래 방향에 산탄 효과 적용
    }
}
