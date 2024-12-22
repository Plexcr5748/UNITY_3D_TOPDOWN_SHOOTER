using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// UI_Settings: ���� �� ���� ȭ���� �����ϴ� Ŭ����
public class UI_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer; // ����� �ͼ� ����
    [SerializeField] private float sliderMultiplier = 25; // �����̴� �� ��ȯ�� ���� ���

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider; // SFX ���� �����̴�
    [SerializeField] private TextMeshProUGUI sfxSliderText; // SFX ���� �� ǥ�� �ؽ�Ʈ
    [SerializeField] private string sfxParametr; // ����� �ͼ��� SFX �Ķ���� �̸�

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider; // BGM ���� �����̴�
    [SerializeField] private TextMeshProUGUI bgmSliderText; // BGM ���� �� ǥ�� �ؽ�Ʈ
    [SerializeField] private string bgmParametr; // ����� �ͼ��� BGM �Ķ���� �̸�

    [Header("Toggle")]
    [SerializeField] private Toggle friendlyFireToggle; // Friendly Fire ���� ���

    // SFX �����̴� �� ���� ó��
    public void SFXSliderValue(float value)
    {
        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%"; // �����̴� �� ǥ��
        float newValue = Mathf.Log10(value) * sliderMultiplier; // �α� ������ ��ȯ
        audioMixer.SetFloat(sfxParametr, newValue); // ����� �ͼ��� �� ����
    }

    // BGM �����̴� �� ���� ó��
    public void BGMSliderValue(float value)
    {
        bgmSliderText.text = Mathf.RoundToInt(value * 100) + "%"; // �����̴� �� ǥ��
        float newValue = Mathf.Log10(value) * sliderMultiplier; // �α� ������ ��ȯ
        audioMixer.SetFloat(bgmParametr, newValue); // ����� �ͼ��� �� ����
    }

    // Friendly Fire ��� ó��
    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire; // ���� ���� �� ��������
        GameManager.instance.friendlyFire = !friendlyFire; // ���� ����
    }

    // ���� �ε�
    public void LoadSettings()
    {
        // �����̴� �� �ε� (�⺻�� 0.7f)
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, .7f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, .7f);

        // Friendly Fire ���� �ε�
        int friendlyFireInt = PlayerPrefs.GetInt("FriendlyFire", 0);
        bool newFriendlyFire = friendlyFireInt == 1;

        friendlyFireToggle.isOn = newFriendlyFire; // ��� ���� ����
    }

    // ���� ����
    private void OnDisable()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireInt = friendlyFire ? 1 : 0;

        // Friendly Fire, SFX, BGM ���� ����
        PlayerPrefs.SetInt("FriendlyFire", friendlyFireInt);
        PlayerPrefs.SetFloat(sfxParametr, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParametr, bgmSlider.value);
    }
}
