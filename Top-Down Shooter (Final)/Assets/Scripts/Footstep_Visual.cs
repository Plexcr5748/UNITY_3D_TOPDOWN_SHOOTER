using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Footstep_Visual: �߰��� �ð� ȿ���� ó���ϴ� Ŭ����
public class Footstep_Visual : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround; // �������� �νĵ� ���̾�

    [SerializeField] private TrailRenderer leftFoot; // �޹��� TrailRenderer
    [SerializeField] private TrailRenderer rightFoot; // �������� TrailRenderer

    [Range(0.001f, 0.3f)]
    [SerializeField] private float checkRadius = 0.05f; // ���� üũ �ݰ�
    [Range(-.15f, .15f)]
    [SerializeField] private float rayDistance = -0.05f; // ���� üũ�� ���� ���� �Ÿ�

    private void Update()
    {
        // �� ������ �޹߰� �������� ���� ���θ� Ȯ��
        CheckFootstep(leftFoot);
        CheckFootstep(rightFoot);
    }

    private void CheckFootstep(TrailRenderer foot)
    {
        // �� ��ġ�� �������� ���� ���� Ȯ��
        Vector3 checkposition = foot.transform.position + Vector3.down * rayDistance;

        // �� �Ʒ� ������ �ݰ� ���� ������ �ִ��� Ȯ��
        bool touchingGround = Physics.CheckSphere(checkposition, checkRadius, whatIsGround);

        // ���ǰ� ���� ���� �� TrailRenderer Ȱ��ȭ
        foot.emitting = touchingGround;
    }

    private void OnDrawGizmos()
    {
        // �����Ϳ��� ���� Ȯ�� ������ �ð������� ǥ��
        DrawFootGizmos(leftFoot.transform);
        DrawFootGizmos(rightFoot.transform);
    }

    private void DrawFootGizmos(Transform foot)
    {
        // ���� üũ ��ġ�� �Ķ��� ��ü�� ǥ��
        if (foot == null)
            return;

        Gizmos.color = Color.blue;
        Vector3 checkposition = foot.transform.position + Vector3.down * rayDistance;

        Gizmos.DrawWireSphere(checkposition, checkRadius);
    }
}
