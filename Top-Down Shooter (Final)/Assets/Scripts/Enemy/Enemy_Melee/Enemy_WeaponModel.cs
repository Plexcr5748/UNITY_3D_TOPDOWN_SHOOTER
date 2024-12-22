using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType; // 무기 유형
    public AnimatorOverrideController overrideController; // 애니메이션 오버라이드 컨트롤러
    public Enemy_MeleeWeaponData weaponData; // 무기 데이터

    [SerializeField] private GameObject[] trailEffects; // 공격 이펙트 (예: 트레일 효과)

    [Header("Damage Attributes")]
    public Transform[] damagePoints; // 공격 지점
    public float attackRadius; // 공격 범위

    [ContextMenu("Assign damage point transforms")]
    private void GetDamagePoints()
    {
        // 트레일 이펙트의 위치를 공격 지점으로 할당
        damagePoints = new Transform[trailEffects.Length];
        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }


    //트레일 이펙트를 활성화 또는 비활성화
    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }

    private void OnDrawGizmos()
    {
        // 공격 지점 및 범위를 기즈모로 시각화
        if (damagePoints.Length > 0)
        {
            Gizmos.color = Color.red; // 기즈모 색상
            foreach (Transform point in damagePoints)
            {
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
        }
    }
}
