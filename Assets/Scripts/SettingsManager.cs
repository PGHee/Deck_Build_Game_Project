using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public GameObject settingsPanel;
    public Slider bgmVolumeSlider;
    public Slider effectVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown frameRateDropdown;
    public Toggle vSyncToggle;
    public AudioMixer audioMixer;
    private Resolution[] resolutions;

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeSettings();  // 게임 시작 시 설정값을 UI에 반영 및 적용
    }

    void InitializeSettings()
    {
        // 해상도 설정 초기화
        audioMixer.SetFloat("MasterVolume", 20f);
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        List<Resolution> filteredResolutions = new List<Resolution>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        // 중복 제거 및 고유 해상도 옵션 추가
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!uniqueResolutions.Contains(option))
            {
                uniqueResolutions.Add(option);
                options.Add(option);
                filteredResolutions.Add(resolutions[i]); // 중복되지 않은 해상도만 추가
            }
        }
        resolutionDropdown.AddOptions(options);

        // 저장된 해상도 인덱스를 불러오거나 기본값으로 설정
        int currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", GetDefaultResolutionIndex(filteredResolutions));

        // 유효한 인덱스인지 확인한 후 적용
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        frameRateDropdown.ClearOptions();
        List<string> frameRateOptions = new List<string> { "30", "60", "120", "Unlimited" };
        frameRateDropdown.AddOptions(frameRateOptions);
        frameRateDropdown.value = PlayerPrefs.GetInt("frameRate", 1); // 기본값: 60 FPS
        frameRateDropdown.RefreshShownValue();

        // 이전 설정 불러오기
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("bgmVolume", 0.8f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("effectVolume", 0.8f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        vSyncToggle.isOn = PlayerPrefs.GetInt("vSync", QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;

        ApplySettings(filteredResolutions);  // 설정을 실제 게임에 적용
        AttachEvents(filteredResolutions);   // 이벤트 연결
    }


    void ApplySettings(List<Resolution> filteredResolutions)
    {
        SetBGMVolume(bgmVolumeSlider.value);
        SetEffectVolume(effectVolumeSlider.value);
        SetResolution(filteredResolutions, resolutionDropdown.value);
        SetFullscreen(fullscreenToggle.isOn);
        SetFrameRate(frameRateDropdown.value);
        SetVSync(vSyncToggle.isOn);
    }

    void AttachEvents(List<Resolution> filteredResolutions)
    {
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        effectVolumeSlider.onValueChanged.AddListener(SetEffectVolume);
        resolutionDropdown.onValueChanged.AddListener((value) => SetResolution(filteredResolutions, value)); // 변경된 해상도 리스트와 함께
        frameRateDropdown.onValueChanged.AddListener(SetFrameRate);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        vSyncToggle.onValueChanged.AddListener(SetVSync);
    }

    void SetResolution(List<Resolution> filteredResolutions, int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex]; // 중복 제거된 해상도 적용
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    int GetDefaultResolutionIndex(List<Resolution> filteredResolutions)
    {
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                return i;
            }
        }
        return 0; // 기본값: 일치하는 해상도가 없으면 첫 번째 해상도를 반환
    }

    void ApplySettings()
    {
        // 슬라이더와 드롭다운 값을 즉시 적용
        SetBGMVolume(bgmVolumeSlider.value);
        SetEffectVolume(effectVolumeSlider.value);
        SetResolution(resolutionDropdown.value);
        SetFullscreen(fullscreenToggle.isOn);
        SetFrameRate(frameRateDropdown.value);
        SetVSync(vSyncToggle.isOn);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Lerp(-80f, 20f, Mathf.Clamp01(volume)));
        PlayerPrefs.SetFloat("bgmVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Lerp(-80f, 20f, Mathf.Clamp01(volume)));
        PlayerPrefs.SetFloat("effectVolume", volume);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetFrameRate(int frameRateIndex)
    {
        int[] frameRates = { 30, 60, 120, -1 };  // 프레임 제한 설정
        Application.targetFrameRate = frameRates[frameRateIndex];
        PlayerPrefs.SetInt("frameRate", frameRateIndex);
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("vSync", isVSync ? 1 : 0);
    }
}
