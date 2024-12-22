using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 근접 무기 데이터를 정의하는 ScriptableObject
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Melee weapon Data")]
public class Enemy_MeleeWeaponData : ScriptableObject
{
    public List<AttackData_EnemyMelee> attackData; // 근접 공격 데이터 리스트
    public float turnSpeed = 10; // 공격 시 회전 속도
}
