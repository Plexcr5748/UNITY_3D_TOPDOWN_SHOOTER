using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� �� ���� �ִϸ��̼� Ÿ��
public enum EquipType { SideEquipAnimation, BackEquipAnimation };

// ���� Ȧ��(��� ���) Ÿ��
public enum HoldType { CommonHold = 1, LowHold, HighHold };

public class WeaponModel : MonoBehaviour
{
    public WeaponType weaponType; // ������ Ÿ��
    public EquipType equipAnimationType; // ���� �ִϸ��̼� Ÿ��
    public HoldType holdType; // ������ ��� ���

    public Transform gunPoint; // �Ѿ��� �߻�Ǵ� ��ġ
    public Transform holdPoint; // ĳ���Ͱ� ���⸦ ��� ��ġ

    [Header("Audio")]
    public AudioSource fireSFX; // �߻� ����
    public AudioSource realodSfx; // ������ ����
}
