using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���Ÿ� ���� ���� Ÿ�� ���� (�Ϲ�, ���� ����, ���� ����)
public enum Enemy_RangeWeaponHoldType { Common, LowHold, HighHold };

// ���� ���Ÿ� ���� ���� ��Ÿ���� Ŭ����
public class Enemy_RangeWeaponModel : MonoBehaviour
{
    // �Ѿ��� �߻�Ǵ� ��ġ
    public Transform gunPoint;

    [Space]
    // ���� Ÿ�� (�ǽ���, ������ ��)
    public Enemy_RangeWeaponType weaponType;

    // ���� ���� Ÿ�� (Common, LowHold, HighHold)
    public Enemy_RangeWeaponHoldType weaponHoldType;

    // �޼��� ��ǥ ��ġ �� �Ȳ�ġ�� ��ǥ ��ġ (IK ������ ���)
    public Transform leftHandTarget;
    public Transform leftElbowTarget;
}
