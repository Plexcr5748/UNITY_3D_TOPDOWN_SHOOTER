using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적의 엄폐 지점을 관리하는 클래스
public class Cover : MonoBehaviour
{
    private Transform playerTransform; // 플레이어의 위치 참조

    [Header("Cover points")]
    [SerializeField] private GameObject coverPointPrefab; // 엄폐 지점 프리팹
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>(); // 생성된 엄폐 지점 리스트
    [SerializeField] private float xOffset = 1; // 엄폐 지점의 X축 오프셋
    [SerializeField] private float yOffset = .2f; // 엄폐 지점의 Y축 오프셋
    [SerializeField] private float zOffset = 1; // 엄폐 지점의 Z축 오프셋

    // 초기화: 엄폐 지점 생성 및 플레이어 참조 설정
    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform;
    }

    // 엄폐 지점을 생성
    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints = {
            new Vector3(0, yOffset, zOffset),  // 전방
            new Vector3(0, yOffset, -zOffset), // 후방
            new Vector3(xOffset, yOffset, 0),  // 오른쪽
            new Vector3(-xOffset, yOffset, 0)  // 왼쪽
        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint); // 로컬 좌표를 월드 좌표로 변환
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform)
                .GetComponent<CoverPoint>(); // 엄폐 지점 생성 및 부모 설정

            coverPoints.Add(coverPoint); // 리스트에 추가
        }
    }

    // 적에게 유효한 엄폐 지점을 반환
    public List<CoverPoint> GetValidCoverPoints(Transform enemy)
    {
        List<CoverPoint> validCoverPoints = new List<CoverPoint>();

        foreach (CoverPoint coverPoint in coverPoints)
        {
            if (IsValidCoverPoint(coverPoint, enemy))
                validCoverPoints.Add(coverPoint);
        }

        return validCoverPoints;
    }

    // 특정 엄폐 지점이 유효한지 판단
    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemy)
    {
        if (coverPoint.occupied) // 이미 점유된 지점은 유효하지 않음
            return false;

        if (!IsFutherestFromPlayer(coverPoint)) // 플레이어로부터 가장 멀지 않으면 유효하지 않음
            return false;

        if (IsCoverCloseToPlayer(coverPoint)) // 플레이어와 너무 가까우면 유효하지 않음
            return false;

        if (IsCoverBehindPlayer(coverPoint, enemy)) // 플레이어 뒤에 위치하면 유효하지 않음
            return false;

        if (IsCoverCloseToLastCover(coverPoint, enemy)) // 이전 엄폐 지점과 너무 가까우면 유효하지 않음
            return false;

        return true;
    }

    // 플레이어로부터 가장 먼 엄폐 지점인지 확인
    private bool IsFutherestFromPlayer(CoverPoint coverPoint)
    {
        CoverPoint furthestPoint = null;
        float furthestDistance = 0;

        foreach (CoverPoint point in coverPoints)
        {
            float distance = Vector3.Distance(point.transform.position, playerTransform.position);
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestPoint = point;
            }
        }

        return furthestPoint == coverPoint;
    }

    // 엄폐 지점이 플레이어 뒤에 위치하는지 확인
    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemy)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemy.position);

        return distanceToPlayer < distanceToEnemy;
    }

    // 엄폐 지점이 플레이어와 너무 가까운지 확인
    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.position) < 2;
    }

    // 엄폐 지점이 이전 엄폐 지점과 너무 가까운지 확인
    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().currentCover;
        return lastCover != null &&
            Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
