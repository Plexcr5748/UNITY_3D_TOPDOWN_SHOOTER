using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ������ �����ϴ� Ŭ����
public class Cover : MonoBehaviour
{
    private Transform playerTransform; // �÷��̾��� ��ġ ����

    [Header("Cover points")]
    [SerializeField] private GameObject coverPointPrefab; // ���� ���� ������
    [SerializeField] private List<CoverPoint> coverPoints = new List<CoverPoint>(); // ������ ���� ���� ����Ʈ
    [SerializeField] private float xOffset = 1; // ���� ������ X�� ������
    [SerializeField] private float yOffset = .2f; // ���� ������ Y�� ������
    [SerializeField] private float zOffset = 1; // ���� ������ Z�� ������

    // �ʱ�ȭ: ���� ���� ���� �� �÷��̾� ���� ����
    private void Start()
    {
        GenerateCoverPoints();
        playerTransform = FindObjectOfType<Player>().transform;
    }

    // ���� ������ ����
    private void GenerateCoverPoints()
    {
        Vector3[] localCoverPoints = {
            new Vector3(0, yOffset, zOffset),  // ����
            new Vector3(0, yOffset, -zOffset), // �Ĺ�
            new Vector3(xOffset, yOffset, 0),  // ������
            new Vector3(-xOffset, yOffset, 0)  // ����
        };

        foreach (Vector3 localPoint in localCoverPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint); // ���� ��ǥ�� ���� ��ǥ�� ��ȯ
            CoverPoint coverPoint = Instantiate(coverPointPrefab, worldPoint, Quaternion.identity, transform)
                .GetComponent<CoverPoint>(); // ���� ���� ���� �� �θ� ����

            coverPoints.Add(coverPoint); // ����Ʈ�� �߰�
        }
    }

    // ������ ��ȿ�� ���� ������ ��ȯ
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

    // Ư�� ���� ������ ��ȿ���� �Ǵ�
    private bool IsValidCoverPoint(CoverPoint coverPoint, Transform enemy)
    {
        if (coverPoint.occupied) // �̹� ������ ������ ��ȿ���� ����
            return false;

        if (!IsFutherestFromPlayer(coverPoint)) // �÷��̾�κ��� ���� ���� ������ ��ȿ���� ����
            return false;

        if (IsCoverCloseToPlayer(coverPoint)) // �÷��̾�� �ʹ� ������ ��ȿ���� ����
            return false;

        if (IsCoverBehindPlayer(coverPoint, enemy)) // �÷��̾� �ڿ� ��ġ�ϸ� ��ȿ���� ����
            return false;

        if (IsCoverCloseToLastCover(coverPoint, enemy)) // ���� ���� ������ �ʹ� ������ ��ȿ���� ����
            return false;

        return true;
    }

    // �÷��̾�κ��� ���� �� ���� �������� Ȯ��
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

    // ���� ������ �÷��̾� �ڿ� ��ġ�ϴ��� Ȯ��
    private bool IsCoverBehindPlayer(CoverPoint coverPoint, Transform enemy)
    {
        float distanceToPlayer = Vector3.Distance(coverPoint.transform.position, playerTransform.position);
        float distanceToEnemy = Vector3.Distance(coverPoint.transform.position, enemy.position);

        return distanceToPlayer < distanceToEnemy;
    }

    // ���� ������ �÷��̾�� �ʹ� ������� Ȯ��
    private bool IsCoverCloseToPlayer(CoverPoint coverPoint)
    {
        return Vector3.Distance(coverPoint.transform.position, playerTransform.position) < 2;
    }

    // ���� ������ ���� ���� ������ �ʹ� ������� Ȯ��
    private bool IsCoverCloseToLastCover(CoverPoint coverPoint, Transform enemy)
    {
        CoverPoint lastCover = enemy.GetComponent<Enemy_Range>().currentCover;
        return lastCover != null &&
            Vector3.Distance(coverPoint.transform.position, lastCover.transform.position) < 3;
    }
}
