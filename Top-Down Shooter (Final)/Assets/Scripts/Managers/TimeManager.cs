using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TimeManager: ������ �ð� ������ �����ϴ� Ŭ����
public class TimeManager : MonoBehaviour
{
    public static TimeManager instance; // �̱��� �ν��Ͻ�

    [SerializeField] private float resumeRate = 3; // �ð� ���� �ӵ�
    [SerializeField] private float pauseRate = 7; // �ð� �Ͻ����� �ӵ�

    private float timeAdjustRate; // �ð� ���� �ӵ�
    private float targetTimeScale = 1; // ��ǥ �ð� ����

    private void Awake()
    {
        instance = this; // �̱��� ����
    }

    private void Update()
    {
        // Q Ű�� ������ 1�ʰ� ���ο� ��� ����
        if (Input.GetKeyDown(KeyCode.Q))
            SlowMotionFor(1f);

        // ���� �ð� ����(Time.timeScale)�� ��ǥ �ð� ������ �ٸ� ��� ����
        if (Mathf.Abs(Time.timeScale - targetTimeScale) > .05f)
        {
            float adjustRate = Time.unscaledDeltaTime * timeAdjustRate; // ������ȭ�� ��Ÿ Ÿ�� ��� ���� �ӵ�
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, adjustRate); // ���� �������� �ð� ���� ����
        }
        else
        {
            // ��ǥ �ð� ������ �������� ��� ����
            Time.timeScale = targetTimeScale;
        }
    }

    // �ð��� �Ͻ�����
    public void PauseTime()
    {
        timeAdjustRate = pauseRate; // �ð� ���� �ӵ� ����
        targetTimeScale = 0; // ��ǥ �ð� ������ 0���� ����
    }

    // �ð��� �簳
    public void ResumeTime()
    {
        timeAdjustRate = resumeRate; // �ð� ���� �ӵ� ����
        targetTimeScale = 1; // ��ǥ �ð� ������ 1�� ����
    }

    // Ư�� �ð� ���� ���ο� ��� ����
    public void SlowMotionFor(float seconds) => StartCoroutine(SlowTimeCo(seconds));

    // ���ο� ��� �ڷ�ƾ
    private IEnumerator SlowTimeCo(float seconds)
    {
        targetTimeScale = .5f; // ��ǥ �ð� ������ 0.5�� ����
        Time.timeScale = targetTimeScale; // ���� �ð� ������ �ٷ� ����
        yield return new WaitForSecondsRealtime(seconds); // �ǽð����� ���
        ResumeTime(); // ���ο� ��� ���� �� �ð� ����
    }
}
