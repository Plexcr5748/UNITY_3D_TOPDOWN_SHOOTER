using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage; // �Ѿ��� ������
    private float impactForce; // �浹 �� ����Ǵ� ��

    private BoxCollider cd; // �Ѿ��� �ݶ��̴�
    private Rigidbody rb; // �Ѿ��� ������ٵ�
    private MeshRenderer meshRenderer; // �Ѿ��� �ð��� �������� ����ϴ� ������Ʈ
    private TrailRenderer trailRenderer; // �Ѿ��� �ܻ� ȿ���� ����ϴ� ������Ʈ

    [SerializeField] private GameObject bulletImpactFX; // �Ѿ� �浹 �� ������ ����Ʈ

    private Vector3 startPosition; // �Ѿ��� ���� ��ġ
    private float flyDistance; // �Ѿ��� ���� ������ �Ÿ�
    private bool bulletDisabled; // �Ѿ� ��Ȱ��ȭ ���¸� ����

    private LayerMask allyLayerMask; // �Ʊ��� ���̾� ����ũ

    protected virtual void Awake()
    {
        // �ʿ��� ������Ʈ �ʱ�ȭ
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void BulletSetup(LayerMask allyLayerMask, int bulletDamage, float flyDistance = 100, float impactForce = 100)
    {
        // �Ѿ� �ʱ� ����
        this.allyLayerMask = allyLayerMask;
        this.impactForce = impactForce;
        this.bulletDamage = bulletDamage;

        bulletDisabled = false; // �Ѿ��� Ȱ��ȭ ���·� ����
        cd.enabled = true;
        meshRenderer.enabled = true;

        trailRenderer.Clear(); // �ܻ� �ʱ�ȭ
        trailRenderer.time = .25f; // �ܻ� ���� �ð� ����
        startPosition = transform.position; // �Ѿ� ���� ��ġ ����
        this.flyDistance = flyDistance + .5f; // �Ÿ� ���� (+0.5�� ������ ���� ����)
    }

    protected virtual void Update()
    {
        // ���� ���¸� �� �����Ӹ��� Ȯ��
        FadeTrailIfNeeded();
        DisableBulletIfNeeded();
        ReturnToPoolIfNeeded();
    }

    protected void ReturnToPoolIfNeeded()
    {
        // �ܻ��� ��� ������� �Ѿ��� Ǯ�� ��ȯ
        if (trailRenderer.time < 0)
            ReturnBulletToPool();
    }

    protected void DisableBulletIfNeeded()
    {
        // ���� �Ÿ��� �ʰ��ϸ� �Ѿ��� ��Ȱ��ȭ
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisabled)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisabled = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        // ���� ���ᰡ ��������� �ܻ��� ���� ����
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime; // �׽�Ʈ�� ������ ��
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        // �浹 �̺�Ʈ ó��
        if (FriendlyFare() == false)
        {
            // �浹�� ��ü�� �Ʊ����� Ȯ��
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(10); // �Ʊ��̸� �Ѿ��� Ǯ�� ��ȯ
                return;
            }
        }

        CreateImpactFx(); // �浹 ����Ʈ ����
        ReturnBulletToPool(); // �Ѿ� ��ȯ

        // �浹 ����� ������ ���� �� �ִ��� Ȯ��
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(bulletDamage);

        ApplyBulletImpactToEnemy(collision); // ���� ��� ����
    }

    private void ApplyBulletImpactToEnemy(Collision collision)
    {
        // �� ��ü�� �浹�� ���� �� ����
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.velocity.normalized * impactForce; // �浹 ���⿡ ���� �� ���
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;
            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody); // ������ �浹 ó�� ����
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        // �Ѿ��� ������Ʈ Ǯ�� ��ȯ
        ObjectPool.instance.ReturnObject(gameObject, delay);
    }

    protected void CreateImpactFx()
    {
        // �浹 ����Ʈ ����
        GameObject newFx = Instantiate(bulletImpactFX);
        newFx.transform.position = transform.position;

        Destroy(newFx, 1); // 1�� �� ����Ʈ ����
    }

    private bool FriendlyFare()
    {
        // �Ʊ� ���� ���� Ȯ��
        return GameManager.instance.friendlyFire;
    }
}
