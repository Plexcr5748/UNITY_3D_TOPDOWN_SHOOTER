using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 엄폐 지점을 나타내는 클래스
public class CoverPoint : MonoBehaviour
{
    public bool occupied; // 엄폐 지점의 점유 상태

    // 점유 상태 설정 메서드
    public void SetOccupied(bool occupied) => this.occupied = occupied;
}
