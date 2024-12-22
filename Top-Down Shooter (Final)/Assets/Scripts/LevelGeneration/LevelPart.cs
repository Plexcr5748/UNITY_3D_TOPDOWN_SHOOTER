using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection check")]
    [SerializeField] private LayerMask intersectionLayer; // ���� ���� �� ����� ���̾�
    [SerializeField] private Collider[] intersectionCheckColliders; // ���� ������ ����� �ݶ��̴�
    [SerializeField] private Transform intersectionCheckParent; // ���� ���� �ݶ��̴��� �θ�

    // ����ƽ ��ü�� ���̾ 'Environment'�� ����
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
        // ���� ���� �ݶ��̴��� ��� ������ �θ�κ��� ������
        if (intersectionCheckColliders.Length <= 0)
        {
            intersectionCheckColliders = intersectionCheckParent.GetComponentsInChildren<Collider>();
        }
    }

    // ������ �����Ǿ����� Ȯ��
    public bool IntersectionDetected()
    {
        Physics.SyncTransforms(); // Transform ������Ʈ�� ����ȭ

        foreach (var collider in intersectionCheckColliders)
        {
            // ���� ������ ���� OverlapBox ���
            Collider[] hitColliders = Physics.OverlapBox(
                collider.bounds.center,
                collider.bounds.extents,
                Quaternion.identity,
                intersectionLayer
            );

            foreach (var hit in hitColliders)
            {
                // ������ ��ü�� InteresectionCheck�� ������ �ִ��� Ȯ��
                InteresectionCheck intersectionCheck = hit.GetComponentInParent<InteresectionCheck>();

                // ������ ��ü�� �ڽ��� �ƴ� ��� ���� ����
                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                    return true;
            }
        }
        return false;
    }

    // SnapPoint�� ����Ͽ� ���� ��Ʈ�� �����ϰ� ����
    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo(entrancePoint, targetSnapPoint); // ���� ����
        SnapTo(entrancePoint, targetSnapPoint); // ��ġ ����
    }

    // SnapPoint�� �������� ȸ�� ����
    private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // �ڽ��� SnapPoint�� ���� ���� ��Ʈ�� ȸ�� ���̸� ���
        var rotationOffset =
            ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        // Ÿ�� SnapPoint�� ȸ���� ����
        transform.rotation = targetSnapPoint.transform.rotation;

        // SnapPoint�� ���� ���߱� ���� 180�� ȸ��
        transform.Rotate(0, 180, 0);

        // �ʱ� ȸ�� ���̸� �ݿ��Ͽ� �̼� ����
        transform.Rotate(0, -rotationOffset, 0);
    }

    // SnapPoint�� �������� ��ġ ����
    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // �ڽ��� SnapPoint ��ġ�� ���� ��ġ ���� ������ ���
        var offset = transform.position - ownSnapPoint.transform.position;

        // Ÿ�� SnapPoint ��ġ�� �������� �߰��Ͽ� �� ��ġ ���
        var newPosition = targetSnapPoint.transform.position + offset;

        // �� ��ġ�� �̵�
        transform.position = newPosition;
    }

    // SnapPoint Ÿ�Կ� ���� SnapPoint ��ȯ
    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);

    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        // ������ Ÿ���� SnapPoint ����
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == pointType)
                filteredSnapPoints.Add(snapPoint);
        }

        // SnapPoint�� �����ϸ� �������� ��ȯ
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        // ��Ī�Ǵ� SnapPoint�� ������ null ��ȯ
        return null;
    }

    // �ڽ��� ������ �� ����Ʈ ��ȯ
    public Enemy[] MyEnemies() => GetComponentsInChildren<Enemy>(true);
}
