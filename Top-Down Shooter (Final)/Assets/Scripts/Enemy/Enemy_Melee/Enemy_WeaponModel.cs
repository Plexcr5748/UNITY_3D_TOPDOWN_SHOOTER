using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_WeaponModel : MonoBehaviour
{
    public Enemy_MeleeWeaponType weaponType; // ���� ����
    public AnimatorOverrideController overrideController; // �ִϸ��̼� �������̵� ��Ʈ�ѷ�
    public Enemy_MeleeWeaponData weaponData; // ���� ������

    [SerializeField] private GameObject[] trailEffects; // ���� ����Ʈ (��: Ʈ���� ȿ��)

    [Header("Damage Attributes")]
    public Transform[] damagePoints; // ���� ����
    public float attackRadius; // ���� ����

    [ContextMenu("Assign damage point transforms")]
    private void GetDamagePoints()
    {
        // Ʈ���� ����Ʈ�� ��ġ�� ���� �������� �Ҵ�
        damagePoints = new Transform[trailEffects.Length];
        for (int i = 0; i < trailEffects.Length; i++)
        {
            damagePoints[i] = trailEffects[i].transform;
        }
    }


    //Ʈ���� ����Ʈ�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
    public void EnableTrailEffect(bool enable)
    {
        foreach (var effect in trailEffects)
        {
            effect.SetActive(enable);
        }
    }

    private void OnDrawGizmos()
    {
        // ���� ���� �� ������ ������ �ð�ȭ
        if (damagePoints.Length > 0)
        {
            Gizmos.color = Color.red; // ����� ����
            foreach (Transform point in damagePoints)
            {
                Gizmos.DrawWireSphere(point.position, attackRadius);
            }
        }
    }
}
