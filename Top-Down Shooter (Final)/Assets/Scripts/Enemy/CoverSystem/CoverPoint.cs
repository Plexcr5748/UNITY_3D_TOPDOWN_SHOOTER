using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ������ ��Ÿ���� Ŭ����
public class CoverPoint : MonoBehaviour
{
    public bool occupied; // ���� ������ ���� ����

    // ���� ���� ���� �޼���
    public void SetOccupied(bool occupied) => this.occupied = occupied;
}
