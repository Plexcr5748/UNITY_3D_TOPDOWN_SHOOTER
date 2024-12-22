using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection check")]
    [SerializeField] private LayerMask intersectionLayer; // 교차 감지 시 사용할 레이어
    [SerializeField] private Collider[] intersectionCheckColliders; // 교차 감지에 사용할 콜라이더
    [SerializeField] private Transform intersectionCheckParent; // 교차 감지 콜라이더의 부모

    // 스태틱 객체의 레이어를 'Environment'로 설정
    [ContextMenu("Set static to envoirment layer")]
    private void AdjustLayerForStaticObjcets()
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            if (childTransform.gameObject.isStatic)
            {
                childTransform.gameObject.layer = LayerMask.NameToLayer("Environment");
            }
        }
    }

    private void Start()
    {
        // 교차 감지 콜라이더가 비어 있으면 부모로부터 가져옴
        if (intersectionCheckColliders.Length <= 0)
        {
            intersectionCheckColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }
    }

    // 교차가 감지되었는지 확인
    public bool IntersectionDetected()
    {
        Physics.SyncTransforms(); // Transform 업데이트를 동기화

        foreach (var collider in intersectionCheckColliders)
        {
            // 교차 감지를 위해 OverlapBox 사용
            Collider[] hitColliders = Physics.OverlapBox(
                collider.bounds.center,
                collider.bounds.extents,
                Quaternion.identity,
                intersectionLayer
            );

            foreach (var hit in hitColliders)
            {
                // 교차된 객체가 InteresectionCheck를 가지고 있는지 확인
                InteresectionCheck intersectionCheck = hit.GetComponentInParent<InteresectionCheck>();

                // 교차된 객체가 자신이 아닌 경우 교차 감지
                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                    return true;
            }
        }
        return false;
    }

    // SnapPoint를 사용하여 레벨 파트를 정렬하고 맞춤
    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo(entrancePoint, targetSnapPoint); // 먼저 정렬
        SnapTo(entrancePoint, targetSnapPoint); // 위치 맞춤
    }

    // SnapPoint를 기준으로 회전 정렬
    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // 자신의 SnapPoint와 현재 레벨 파트의 회전 차이를 계산
        var rotationOffset =
            ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        // 타겟 SnapPoint의 회전에 맞춤
        transform.rotation = targetSnapPoint.transform.rotation;

        // SnapPoint를 서로 맞추기 위해 180도 회전
        transform.Rotate(0, 180, 0);

        // 초기 회전 차이를 반영하여 미세 조정
        transform.Rotate(0, -rotationOffset, 0);
    }

    // SnapPoint를 기준으로 위치 맞춤
    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // 자신의 SnapPoint 위치와 현재 위치 간의 오프셋 계산
        var offset = transform.position - ownSnapPoint.transform.position;

        // 타겟 SnapPoint 위치에 오프셋을 추가하여 새 위치 계산
        var newPosition = targetSnapPoint.transform.position + offset;

        // 새 위치로 이동
        transform.position = newPosition;
    }

    // SnapPoint 타입에 따라 SnapPoint 반환
    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);

    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        // 지정된 타입의 SnapPoint 수집
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == pointType)
                filteredSnapPoints.Add(snapPoint);
        }

        // SnapPoint가 존재하면 랜덤으로 반환
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        // 매칭되는 SnapPoint가 없으면 null 반환
        return null;
    }

    // 자신이 소유한 적 리스트 반환
    public Enemy[] MyEnemies() => GetComponentsInChildren<Enemy>(true);
}
