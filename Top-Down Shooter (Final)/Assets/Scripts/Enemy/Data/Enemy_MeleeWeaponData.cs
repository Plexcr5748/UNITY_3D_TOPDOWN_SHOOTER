using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �����͸� �����ϴ� ScriptableObject
[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Enemy data/Melee weapon Data")]
public class Enemy_MeleeWeaponData : ScriptableObject
{
    public List<AttackData_EnemyMelee> attackData; // ���� ���� ������ ����Ʈ
    public float turnSpeed = 10; // ���� �� ȸ�� �ӵ�
}
