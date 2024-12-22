using System.Collections.Generic;
using UnityEngine;

// Player_Interaction: �÷��̾�� ��ȣ�ۿ� ������ ��ü�� �����ϴ� Ŭ����
public class Player_Interaction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>(); // ��ȣ�ۿ� ������ ��ü ���
    private Interactable closestInteractable; // ���� ����� ��ȣ�ۿ� ������ ��ü

    private void Start()
    {
        // �÷��̾��� �Է� ��Ʈ�ѿ��� ��ȣ�ۿ� �̺�Ʈ ���
        Player player = GetComponent<Player>();
        player.controls.Character.Interaction.performed += context => InteractWithClosest();
    }

    private void InteractWithClosest()
    {
        // ���� ����� ��ü�� ��ȣ�ۿ�
        closestInteractable?.Interaction();

        // ��ȣ�ۿ� �� �ش� ��ü�� ����Ʈ���� ����
        interactables.Remove(closestInteractable);

        // ���� ����� ��ȣ�ۿ� ������ ��ü ������Ʈ
        UpdateClosestInteractble();
    }

    public void UpdateClosestInteractble()
    {
        // ���� ���� ����� ��ü�� ���̶���Ʈ ��Ȱ��ȭ
        closestInteractable?.HighlightActive(false);
        closestInteractable = null;

        float closestDistance = float.MaxValue; // �ʱ�ȭ�� ���� ����� �Ÿ�

        // ��ȣ�ۿ� ������ ��ü�� �� ���� ����� ��ü�� ã��
        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        // ���� ����� ��ü�� ���̶���Ʈ Ȱ��ȭ
        closestInteractable?.HighlightActive(true);
    }

    public List<Interactable> GetInteracbles() => interactables;
    // ���� ��ȣ�ۿ� ������ ��ü ��� ��ȯ
}
