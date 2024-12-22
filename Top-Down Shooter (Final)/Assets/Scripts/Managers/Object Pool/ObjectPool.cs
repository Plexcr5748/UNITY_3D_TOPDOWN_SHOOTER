using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ObjectPool: 게임 오브젝트 풀링 시스템을 관리하는 클래스
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance; // 싱글턴 인스턴스

    [SerializeField] private int poolSize = 10; // 각 오브젝트 풀의 초기 크기

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary =
        new Dictionary<GameObject, Queue<GameObject>>(); // 프리팹별 오브젝트 큐 관리

    [Header("To Initialize")]
    [SerializeField] private GameObject weaponPickup; // 무기 픽업 프리팹
    [SerializeField] private GameObject ammoPickup; // 탄약 픽업 프리팹
    [SerializeField] private GameObject mediPickup; // 의료 픽업 프리팹

    private void Awake()
    {
        // 싱글턴 설정
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject); // 중복된 인스턴스 제거
    }

    private void Start()
    {
        // 초기화: 각 프리팹의 풀 생성
        InitializeNewPool(weaponPickup);
        InitializeNewPool(ammoPickup);
        InitializeNewPool(mediPickup);
    }

    // 풀에서 오브젝트를 가져오는 메서드
    public GameObject GetObject(GameObject prefab, Transform target)
    {
        // 프리팹에 대한 풀 존재 여부 확인
        if (poolDictionary.ContainsKey(prefab) == false)
        {
            InitializeNewPool(prefab); // 새 풀 생성
        }

        // 풀에 사용할 오브젝트가 없으면 새로 생성
        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        // 풀에서 오브젝트 가져오기
        GameObject objectToGet = poolDictionary[prefab].Dequeue();

        // 오브젝트 위치 및 부모 설정
        objectToGet.transform.position = target.position;
        objectToGet.transform.parent = null;

        objectToGet.SetActive(true); // 활성화
        return objectToGet;
    }

    // 오브젝트를 풀로 반환하는 메서드
    public void ReturnObject(GameObject objectToReturn, float delay = .001f)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn)); // 지연 반환 코루틴 호출
    }

    // 지연 반환 코루틴
    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objectToReturn); // 풀로 반환
    }

    // 오브젝트를 풀로 반환
    private void ReturnToPool(GameObject objectToReturn)
    {
        // 오브젝트의 원래 프리팹 참조
        GameObject originalPrefab = objectToReturn.GetComponent<PooledObject>().originalPrefab;

        objectToReturn.SetActive(false); // 비활성화
        objectToReturn.transform.parent = transform; // ObjectPool의 자식으로 설정

        // 풀에 오브젝트 추가
        poolDictionary[originalPrefab].Enqueue(objectToReturn);
    }

    // 새 프리팹에 대한 풀 초기화
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>(); // 새 큐 생성

        // 지정된 풀 크기만큼 오브젝트 생성
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    // 풀에 새 오브젝트 생성
    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform); // 프리팹 인스턴스 생성
        newObject.AddComponent<PooledObject>().originalPrefab = prefab; // PooledObject 추가
        newObject.SetActive(false); // 비활성화

        poolDictionary[prefab].Enqueue(newObject); // 풀에 추가
    }
}
