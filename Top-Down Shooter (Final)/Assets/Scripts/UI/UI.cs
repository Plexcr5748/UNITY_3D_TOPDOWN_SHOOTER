using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI: ������ UI ���� Ŭ����
public class UI : MonoBehaviour
{
    public static UI instance; // �̱��� �ν��Ͻ�

    public UI_InGame inGameUI { get; private set; } // ���� �� UI ����

    // UI ���
    public GameObject[] NoESCUI; // ESC Ű�� ������� �ʴ� UI
    public GameObject mainUI; // ���� UI
    public GameObject[] SelectUI; // ���� UI �迭
    public GameObject weaponSelUI; // ���� ���� UI
    public GameObject missionSelUI; // �̼� ���� UI

    public UI_WeaponSelection weaponSelection { get; private set; } // ���� ���� UI ����
    public UI_GameOver gameOverUI { get; private set; } // ���� ���� UI ����
    public UI_Settings settingsUI { get; private set; } // ���� UI ����
    public GameObject victoryScreenUI; // �¸� ȭ�� UI
    public GameObject pauseUI; // �Ͻ����� UI

    [SerializeField] private GameObject[] UIElements; // ��� UI ��� �迭

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage; // ȭ�� ���̵�� �̹���

    private void Awake()
    {
        // UI ������Ʈ �ʱ�ȭ
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
    }

    private void Start()
    {
        // �Է� �̺�Ʈ ����
        AssignInputsUI();

        // ���� ���� �� ���̵� �� ȿ��
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));
    }

    private void Update()
    {
        // UI Ȱ��ȭ ���ο� ���� Ŀ�� ���� ����
        Cursor.visible = !inGameUI.gameObject.activeSelf;

        // ESC Ű �Է� ó��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainUI.gameObject.activeSelf)
                QuitTheGame();
            else if (weaponSelUI.gameObject.activeSelf)
                SwitchTo(missionSelUI);
            else
            {
                foreach (GameObject obj in SelectUI)
                {
                    if (obj != null && obj.activeInHierarchy)
                    {
                        SwitchTo(mainUI);
                    }
                }
            }
        }
    }

    // ������ UI�� ��ȯ
    public void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiToSwitchOn.SetActive(true);

        // ���� UI�� ��ȯ �� ���� �ҷ�����
        if (uiToSwitchOn == settingsUI.gameObject)
            settingsUI.LoadSettings();
    }

    // ���� ����
    public void StartTheGame() => StartCoroutine(StartGameSequence());

    // ���� ����
    public void QuitTheGame() => Application.Quit();

    // ���� ���� ����
    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

    // ���� �����
    public void RestartTheGame()
    {
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.instance.RestartScene));
    }

    // �Ͻ����� ���� ��ȯ
    public void PauseSwitch()
    {
        foreach (GameObject obj in NoESCUI)
        {
            if (obj != null && obj.activeSelf)
            {
                return; // Ȱ��ȭ�� UI�� ������ ó�� �ߴ�
            }
        }

        bool gamePaused = pauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchTo(inGameUI.gameObject);
            ControlsManager.instance.SwitchToCharacterControls();
            TimeManager.instance.ResumeTime();
            Cursor.visible = false;
        }
        else
        {
            SwitchTo(pauseUI);
            ControlsManager.instance.SwitchToUIControls();
            TimeManager.instance.PauseTime();
            Cursor.visible = true;
        }
    }

    // ���� ���� UI ǥ��
    public void ShowGameOverUI(string message = "GAME OVER!")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
        Cursor.visible = true;
    }

    // �¸� ȭ�� UI ǥ��
    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictoryScreenUI));
    }

    private void SwitchToVictoryScreenUI()
    {
        SwitchTo(victoryScreenUI);

        Color color = fadeImage.color;
        color.a = 0;

        fadeImage.color = color;
        Cursor.visible = true;
    }

    // UI �Է� �̺�Ʈ ����
    private void AssignInputsUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;

        controls.UI.UIPause.performed += ctx => PauseSwitch();
    }

    // ���� ���� ������
    private IEnumerator StartGameSequence()
    {
        StartCoroutine(ChangeImageAlpha(1, 1, null));

        yield return new WaitForSeconds(1);
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();

        StartCoroutine(ChangeImageAlpha(0, 1, null));
    }

    // ���̵� �̹��� ���İ� ����
    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        // �Ϸ� �ݹ� ����
        onComplete?.Invoke();
    }

    [ContextMenu("Assign Audio To Buttons")]
    public void AssignAudioListenesrsToButtons()
    {
        // UI ��ư�� ����� ������ ����
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }
}
