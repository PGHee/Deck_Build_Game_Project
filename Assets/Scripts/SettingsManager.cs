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
    private Resolution[] availableResolutions;
    public GameObject backgroundObject;

    // 사용자 지정 해상도 목록
    private readonly List<Vector2Int> targetResolutions = new List<Vector2Int>
    {
        new Vector2Int(1280, 720), new Vector2Int(1280, 800), new Vector2Int(1360, 768), new Vector2Int(1440, 900),
        new Vector2Int(1600, 900), new Vector2Int(1680, 1050), new Vector2Int(1920, 1080), new Vector2Int(1920, 1200),
        new Vector2Int(2560, 1440), new Vector2Int(3200, 1800), new Vector2Int(3840, 2160)
    };

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
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        List<Resolution> filteredResolutions = new List<Resolution>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        // 해상도 필터링 및 사용자 지정 해상도와 매칭되는 값들만 추가
        foreach (Resolution res in availableResolutions)
        {
            Vector2Int resolution = new Vector2Int(res.width, res.height);
            if (targetResolutions.Contains(resolution))
            {
                string option = res.width + " x " + res.height;
                if (!uniqueResolutions.Contains(option))
                {
                    uniqueResolutions.Add(option);
                    filteredResolutions.Add(res);  // 중복되지 않은 해상도만 추가
                }
            }
        }

        // 역순으로 해상도 목록 정렬
        filteredResolutions.Sort((a, b) => (b.width * b.height).CompareTo(a.width * a.height));

        // 드롭다운 옵션 추가
        foreach (Resolution res in filteredResolutions)
        {
            options.Add(res.width + " x " + res.height);
        }
        resolutionDropdown.AddOptions(options);

        // 저장된 해상도 인덱스를 불러오거나 기본값으로 설정
        int currentResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", GetDefaultResolutionIndex(filteredResolutions));

        // 유효한 인덱스인지 확인한 후 적용
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 프레임 레이트 드롭다운 초기화
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
        settingsPanel.SetActive(false);
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
        resolutionDropdown.onValueChanged.AddListener((value) => SetResolution(filteredResolutions, value));
        frameRateDropdown.onValueChanged.AddListener(SetFrameRate);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        vSyncToggle.onValueChanged.AddListener(SetVSync);
    }

    void SetResolution(List<Resolution> filteredResolutions, int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        AdjustBackgroundScale();
    }

    public void AdjustBackgroundScale()
    {
        // Background 오브젝트의 SpriteRenderer를 가져옴
        SpriteRenderer backgroundRenderer = backgroundObject.GetComponent<SpriteRenderer>();
        if (backgroundRenderer == null)
        {
            Debug.LogError("Background 오브젝트에 SpriteRenderer가 없습니다!");
            return;
        }

        // 카메라를 가져옴
        Camera mainCamera = Camera.main;

        // 배경 스프라이트의 크기를 가져옴
        Vector2 spriteSize = backgroundRenderer.sprite.bounds.size;

        // 화면의 높이와 너비 계산
        float screenHeight = mainCamera.orthographicSize * 2.0f;  // 카메라 높이
        float screenWidth = screenHeight * Screen.width / Screen.height;  // 카메라 너비 (해상도 비율에 따른)

        // 화면 크기와 스프라이트 크기 비교 후, 스케일 조정
        Vector3 scale = backgroundObject.transform.localScale;

        // 스프라이트 크기를 화면에 맞추기 위해 가로와 세로 비율을 각각 계산
        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        // 가로, 세로 비율 중 더 큰 값을 선택하여 배경이 화면을 가득 채우도록 스케일 조정
        float finalScale = Mathf.Max(scaleX, scaleY);

        scale.x = finalScale;
        scale.y = finalScale;

        backgroundObject.transform.localScale = scale;
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
