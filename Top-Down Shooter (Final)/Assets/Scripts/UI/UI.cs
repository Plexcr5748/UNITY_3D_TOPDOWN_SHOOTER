using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI: 게임의 UI 관리 클래스
public class UI : MonoBehaviour
{
    public static UI instance; // 싱글톤 인스턴스

    public UI_InGame inGameUI { get; private set; } // 게임 중 UI 참조

    // UI 요소
    public GameObject[] NoESCUI; // ESC 키로 제어되지 않는 UI
    public GameObject mainUI; // 메인 UI
    public GameObject[] SelectUI; // 선택 UI 배열
    public GameObject weaponSelUI; // 무기 선택 UI
    public GameObject missionSelUI; // 미션 선택 UI

    public UI_WeaponSelection weaponSelection { get; private set; } // 무기 선택 UI 관리
    public UI_GameOver gameOverUI { get; private set; } // 게임 오버 UI 관리
    public UI_Settings settingsUI { get; private set; } // 설정 UI 관리
    public GameObject victoryScreenUI; // 승리 화면 UI
    public GameObject pauseUI; // 일시정지 UI

    [SerializeField] private GameObject[] UIElements; // 모든 UI 요소 배열

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage; // 화면 페이드용 이미지

    private void Awake()
    {
        // UI 컴포넌트 초기화
        instance = this;
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        weaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        gameOverUI = GetComponentInChildren<UI_GameOver>(true);
        settingsUI = GetComponentInChildren<UI_Settings>(true);
    }

    private void Start()
    {
        // 입력 이벤트 연결
        AssignInputsUI();

        // 게임 시작 시 페이드 인 효과
        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));
    }

    private void Update()
    {
        // UI 활성화 여부에 따라 커서 상태 변경
        Cursor.visible = !inGameUI.gameObject.activeSelf;

        // ESC 키 입력 처리
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

    // 지정된 UI로 전환
    public void SwitchTo(GameObject uiToSwitchOn)
    {
        foreach (GameObject go in UIElements)
        {
            go.SetActive(false);
        }

        uiToSwitchOn.SetActive(true);

        // 설정 UI로 전환 시 설정 불러오기
        if (uiToSwitchOn == settingsUI.gameObject)
            settingsUI.LoadSettings();
    }

    // 게임 시작
    public void StartTheGame() => StartCoroutine(StartGameSequence());

    // 게임 종료
    public void QuitTheGame() => Application.Quit();

    // 레벨 생성 시작
    public void StartLevelGeneration() => LevelGenerator.instance.InitializeGeneration();

    // 게임 재시작
    public void RestartTheGame()
    {
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.instance.RestartScene));
    }

    // 일시정지 상태 전환
    public void PauseSwitch()
    {
        foreach (GameObject obj in NoESCUI)
        {
            if (obj != null && obj.activeSelf)
            {
                return; // 활성화된 UI가 있으면 처리 중단
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

    // 게임 오버 UI 표시
    public void ShowGameOverUI(string message = "GAME OVER!")
    {
        SwitchTo(gameOverUI.gameObject);
        gameOverUI.ShowGameOverMessage(message);
        Cursor.visible = true;
    }

    // 승리 화면 UI 표시
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

    // UI 입력 이벤트 연결
    private void AssignInputsUI()
    {
        PlayerControls controls = GameManager.instance.player.controls;

        controls.UI.UIPause.performed += ctx => PauseSwitch();
    }

    // 게임 시작 시퀀스
    private IEnumerator StartGameSequence()
    {
        StartCoroutine(ChangeImageAlpha(1, 1, null));

        yield return new WaitForSeconds(1);
        SwitchTo(inGameUI.gameObject);
        GameManager.instance.GameStart();

        StartCoroutine(ChangeImageAlpha(0, 1, null));
    }

    // 페이드 이미지 알파값 변경
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

        // 완료 콜백 실행
        onComplete?.Invoke();
    }

    [ContextMenu("Assign Audio To Buttons")]
    public void AssignAudioListenesrsToButtons()
    {
        // UI 버튼에 오디오 리스너 연결
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }
}
