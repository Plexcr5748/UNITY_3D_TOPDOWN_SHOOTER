using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Footstep_Visual: 발걸음 시각 효과를 처리하는 클래스
public class Footstep_Visual : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround; // 발판으로 인식될 레이어

    [SerializeField] private TrailRenderer leftFoot; // 왼발의 TrailRenderer
    [SerializeField] private TrailRenderer rightFoot; // 오른발의 TrailRenderer

    [Range(0.001f, 0.3f)]
    [SerializeField] private float checkRadius = 0.05f; // 발판 체크 반경
    [Range(-.15f, .15f)]
    [SerializeField] private float rayDistance = -0.05f; // 발판 체크를 위한 레이 거리

    private void Update()
    {
        // 매 프레임 왼발과 오른발의 발판 여부를 확인
        CheckFootstep(leftFoot);
        CheckFootstep(rightFoot);
    }

    private void CheckFootstep(TrailRenderer foot)
    {
        // 발 위치를 기준으로 발판 여부 확인
        Vector3 checkposition = foot.transform.position + Vector3.down * rayDistance;

        // 발 아래 지정된 반경 내에 발판이 있는지 확인
        bool touchingGround = Physics.CheckSphere(checkposition, checkRadius, whatIsGround);

        // 발판과 접촉 중일 때 TrailRenderer 활성화
        foot.emitting = touchingGround;
    }

    private void OnDrawGizmos()
    {
        // 에디터에서 발판 확인 영역을 시각적으로 표시
        DrawFootGizmos(leftFoot.transform);
        DrawFootGizmos(rightFoot.transform);
    }

    private void DrawFootGizmos(Transform foot)
    {
        // 발판 체크 위치를 파란색 구체로 표시
        if (foot == null)
            return;

        Gizmos.color = Color.blue;
        Vector3 checkposition = foot.transform.position + Vector3.down * rayDistance;

        Gizmos.DrawWireSphere(checkposition, checkRadius);
    }
}
