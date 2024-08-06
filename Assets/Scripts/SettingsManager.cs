using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider bgmVolumeSlider;
    public Slider effectVolumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown frameRateDropdown;
    public Toggle vSyncToggle;

    public AudioMixer audioMixer;

    private Resolution[] resolutions;

    void Start()
    {
        // 해상도 설정 초기화
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 프레임 제한 설정 초기화
        frameRateDropdown.ClearOptions();
        List<string> frameRateOptions = new List<string> { "30", "60", "120", "Unlimited" };
        frameRateDropdown.AddOptions(frameRateOptions);
        frameRateDropdown.value = PlayerPrefs.GetInt("frameRate", 1); // 기본값: 60 FPS
        frameRateDropdown.RefreshShownValue();

        // 이전 설정 불러오기
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        effectVolumeSlider.value = PlayerPrefs.GetFloat("effectVolume", 1f);
        fullscreenToggle.isOn = Screen.fullScreen;
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        // 프레임 제한 적용
        SetFrameRate(frameRateDropdown.value);
        SetBGMVolume(bgmVolumeSlider.value);
        SetEffectVolume(effectVolumeSlider.value);
        SetResolution(resolutionDropdown.value);
        SetFullscreen(fullscreenToggle.isOn);
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
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("bgmVolume", volume);
    }

    public void SetEffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
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
        int[] frameRates = { 30, 60, 120, -1 }; // -1은 프레임 제한 없음
        Application.targetFrameRate = frameRates[frameRateIndex];
        PlayerPrefs.SetInt("frameRate", frameRateIndex);
    }

    public void SetVSync(bool isVSync)
    {
        QualitySettings.vSyncCount = isVSync ? 1 : 0;
        PlayerPrefs.SetInt("vSync", isVSync ? 1 : 0);
    }
}
