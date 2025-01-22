using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SettingUIController : UIController
{
    [BoxGroup("Toggle Components")] public Toggle bgmToggle;
    [BoxGroup("Toggle Components")] public Toggle sfxToggle;
    [BoxGroup("Toggle Components")] public Toggle vibrationToggle;

    [BoxGroup("White Image Objects")] public RectTransform bgmWhiteImage;
    [BoxGroup("White Image Objects")] public RectTransform sfxWhiteImage;
    [BoxGroup("White Image Objects")] public RectTransform vibrationWhiteImage;

    [BoxGroup("White Image Positions")] public Vector2 bgmOnPosition;
    [BoxGroup("White Image Positions")] public Vector2 bgmOffPosition;
    [BoxGroup("White Image Positions")] public Vector2 sfxOnPosition;
    [BoxGroup("White Image Positions")] public Vector2 sfxOffPosition;
    [BoxGroup("White Image Positions")] public Vector2 vibrationOnPosition;
    [BoxGroup("White Image Positions")] public Vector2 vibrationOffPosition;

    public Button uIDTextButton;
    
    private SoundManager soundManager;

    private SettingUIView settingUIView;
    
    private void Awake()
    {
        soundManager = SoundManager.Instance;
        settingUIView = GetComponent<SettingUIView>();
        
        // Toggle 이벤트 등록
        bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
        vibrationToggle.onValueChanged.AddListener(OnVibrationToggleChanged);
    }

    private void OnEnable()
    {
        // UI가 켜질 때 설정값 로드 및 초기화
        InitializeToggles();
    }

    private void InitializeToggles()
    {
        // SoundManager와 Vibration 클래스에서 현재 설정값 가져오기
        bool isBgmOn = soundManager.isBgmOn;
        bool isSfxOn = soundManager.isSfxOn;
        bool isVibrationOn = Vibration.IsVibrationEnabled;

        // Toggle 값 설정 (이벤트 발생 안하도록)
        bgmToggle.SetIsOnWithoutNotify(isBgmOn);
        sfxToggle.SetIsOnWithoutNotify(isSfxOn);
        vibrationToggle.SetIsOnWithoutNotify(isVibrationOn);

        // 하얀색 이미지 위치 설정 (DoTween 없이)
        settingUIView.SetWhiteImagePosition(bgmToggle, bgmWhiteImage, bgmOnPosition, bgmOffPosition, false);
        settingUIView.SetWhiteImagePosition(sfxToggle, sfxWhiteImage, sfxOnPosition, sfxOffPosition, false);
        settingUIView.SetWhiteImagePosition(vibrationToggle, vibrationWhiteImage, vibrationOnPosition, vibrationOffPosition, false);
    }

    private void OnBgmToggleChanged(bool isOn)
    {
        // SoundManager에 설정 적용
        soundManager.SetBgmOn(isOn);

        // 하얀색 이미지 위치 이동
        settingUIView.SetWhiteImagePosition(bgmToggle, bgmWhiteImage, bgmOnPosition, bgmOffPosition, true);
    }

    private void OnSfxToggleChanged(bool isOn)
    {
        // SoundManager에 설정 적용
        soundManager.SetSfxOn(isOn);

        // 하얀색 이미지 위치 이동
        settingUIView.SetWhiteImagePosition(sfxToggle, sfxWhiteImage, sfxOnPosition, sfxOffPosition, true);
    }

    private void OnVibrationToggleChanged(bool isOn)
    {
        // Vibration 클래스에 설정 적용
        Vibration.IsVibrationEnabled = isOn;

        // 하얀색 이미지 위치 이동
        settingUIView.SetWhiteImagePosition(vibrationToggle, vibrationWhiteImage, vibrationOnPosition, vibrationOffPosition, true);
    }

    public void UIDTextClicked()
    {
        GUIUtility.systemCopyBuffer = Authenticate.Instance.UID;
        UIManager.Instance.DisplayNotification("UID가 복사 되었습니다.");
    }
}
