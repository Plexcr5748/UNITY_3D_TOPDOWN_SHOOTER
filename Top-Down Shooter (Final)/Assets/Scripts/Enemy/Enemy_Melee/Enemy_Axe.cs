using UnityEngine;

// ���� ���� ��ô�� �����ϴ� Ŭ����
public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx; // �浹 ȿ��
    [SerializeField] private Rigidbody rb; // ������ Rigidbody
    [SerializeField] private Transform axeVisual; // ������ �ð��� ���

    private Vector3 direction; // ������ ���ư� ����
    private Transform player; // �÷��̾��� ��ġ
    private float flySpeed; // ������ ���� �ӵ�
    private float rotationSpeed; // ������ ȸ�� �ӵ�
    private float timer = 1; // ���� ���� �ð�

    private int damage; // ������ ���ط�

    // ���� ���� �޼���
    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        rotationSpeed = 1600; // �⺻ ȸ�� �ӵ� ����

        this.damage = damage; // ���ط� ����
        this.flySpeed = flySpeed; // ���� �ӵ� ����
        this.player = player; // �÷��̾� ���� ����
        this.timer = timer; // ���� ���� �ð� ����
    }

    // �� ������ ���� ȸ�� �� ���� ����
    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime); // ���� ȸ��
        timer -= Time.deltaTime; // Ÿ�̸� ����

        if (timer > 0)
            direction = player.position + Vector3.up - transform.position; // Ÿ�� ���� ����

        transform.forward = rb.velocity; // ������ ���� ���� ����
    }

    // ���� ������Ʈ���� ���� �̵� ó��
    private void FixedUpdate()
    {
        rb.velocity = direction.normalized * flySpeed; // ���⿡ ���� �ӵ� ����
    }

    // �浹 �̺�Ʈ ó��
    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>(); // �浹 ����� ���ظ� ���� �� �ִ��� Ȯ��
        damagable?.TakeDamage(damage); // ������ ����

        // �浹 ȿ�� ����
        GameObject newFx = ObjectPool.instance.GetObject(impactFx, transform);

        // ������ ȿ�� ��ü�� Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject);
        ObjectPool.instance.ReturnObject(newFx, 1f);
    }
}
