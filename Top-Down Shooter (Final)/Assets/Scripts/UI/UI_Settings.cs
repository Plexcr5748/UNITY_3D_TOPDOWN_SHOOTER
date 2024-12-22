using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// UI_Settings: 게임 내 설정 화면을 관리하는 클래스
public class UI_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // 오디오 믹서 참조
    [SerializeField] private float sliderMultiplier = 25; // 슬라이더 값 변환을 위한 배수

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider; // SFX 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI sfxSliderText; // SFX 볼륨 값 표시 텍스트
    [SerializeField] private string sfxParametr; // 오디오 믹서의 SFX 파라미터 이름

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider; // BGM 볼륨 슬라이더
    [SerializeField] private TextMeshProUGUI bgmSliderText; // BGM 볼륨 값 표시 텍스트
    [SerializeField] private string bgmParametr; // 오디오 믹서의 BGM 파라미터 이름

    [Header("Toggle")]
    [SerializeField] private Toggle friendlyFireToggle; // Friendly Fire 설정 토글

    // SFX 슬라이더 값 변경 처리
    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%"; // 슬라이더 값 표시
        float newValue = Mathf.Log10(value) * sliderMultiplier; // 로그 값으로 변환
        audioMixer.SetFloat(sfxParametr, newValue); // 오디오 믹서에 값 설정
    }

    // BGM 슬라이더 값 변경 처리
    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%"; // 슬라이더 값 표시
        float newValue = Mathf.Log10(value) * sliderMultiplier; // 로그 값으로 변환
        audioMixer.SetFloat(bgmParametr, newValue); // 오디오 믹서에 값 설정
    }

    // Friendly Fire 토글 처리
    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire; // 현재 설정 값 가져오기
        GameManager.instance.friendlyFire = !friendlyFire; // 설정 반전
    }

    // 설정 로드
    public void LoadSettings()
    {
        // 슬라이더 값 로드 (기본값 0.7f)
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, .7f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, .7f);

        // Friendly Fire 설정 로드
        int friendlyFireInt = PlayerPrefs.GetInt("FriendlyFire", 0);
        bool newFriendlyFire = friendlyFireInt == 1;

        friendlyFireToggle.isOn = newFriendlyFire; // 토글 상태 설정
    }

    // 설정 저장
    private void OnDisable()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireInt = friendlyFire ? 1 : 0;

        // Friendly Fire, SFX, BGM 설정 저장
        PlayerPrefs.SetInt("FriendlyFire", friendlyFireInt);
        PlayerPrefs.SetFloat(sfxParametr, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParametr, bgmSlider.value);
    }
}
