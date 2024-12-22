using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectPool: ���� ������Ʈ Ǯ�� �ý����� �����ϴ� Ŭ����
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance; // �̱��� �ν��Ͻ�

    [SerializeField] private int poolSize = 10; // �� ������Ʈ Ǯ�� �ʱ� ũ��

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary =
        new Dictionary<GameObject, Queue<GameObject>>(); // �����պ� ������Ʈ ť ����

    [Header("To Initialize")]
    [SerializeField] private GameObject weaponPickup; // ���� �Ⱦ� ������
    [SerializeField] private GameObject ammoPickup; // ź�� �Ⱦ� ������
    [SerializeField] private GameObject mediPickup; // �Ƿ� �Ⱦ� ������

    private void Awake()
    {
        // �̱��� ����
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // �ߺ��� �ν��Ͻ� ����
    }

    private void Start()
    {
        // �ʱ�ȭ: �� �������� Ǯ ����
        InitializeNewPool(weaponPickup);
        InitializeNewPool(ammoPickup);
        InitializeNewPool(mediPickup);
    }

    // Ǯ���� ������Ʈ�� �������� �޼���
    public GameObject GetObject(GameObject prefab, Transform target)
    {
        // �����տ� ���� Ǯ ���� ���� Ȯ��
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab); // �� Ǯ ����
        }

        // Ǯ�� ����� ������Ʈ�� ������ ���� ����
        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        // Ǯ���� ������Ʈ ��������
        GameObject objectToGet = poolDictionary[prefab].Dequeue();

        // ������Ʈ ��ġ �� �θ� ����
        objectToGet.transform.position = target.position;
        objectToGet.transform.parent = null;

        objectToGet.SetActive(true); // Ȱ��ȭ
        return objectToGet;
    }

    // ������Ʈ�� Ǯ�� ��ȯ�ϴ� �޼���
    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn)); // ���� ��ȯ �ڷ�ƾ ȣ��
    }

    // ���� ��ȯ �ڷ�ƾ
    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn); // Ǯ�� ��ȯ
    }

    // ������Ʈ�� Ǯ�� ��ȯ
    private void ReturnToPool(GameObject objectToReturn)
    {
        // ������Ʈ�� ���� ������ ����
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false); // ��Ȱ��ȭ
        objectToReturn.transform.parent = transform; // ObjectPool�� �ڽ����� ����

        // Ǯ�� ������Ʈ �߰�
        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    // �� �����տ� ���� Ǯ �ʱ�ȭ
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>(); // �� ť ����

        // ������ Ǯ ũ�⸸ŭ ������Ʈ ����
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    // Ǯ�� �� ������Ʈ ����
    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform); // ������ �ν��Ͻ� ����
        newObject.AddComponent<PooledObject>().originalPrefab = prefab; // PooledObject �߰�
        newObject.SetActive(false); // ��Ȱ��ȭ

        poolDictionary[prefab].Enqueue(newObject); // Ǯ�� �߰�
    }
}
