using DG.Tweening;
using GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : MonoBehaviour
{
    public RectTransform starRectTransform;
    public TMP_Text progressText;
    public TMP_Text slotText;
    public Slider progressSlider;
    public TMP_Text slotDescriptionText;
    public TMP_Text starRewardText;
    public Button rewardButton;
    public Material glassMaterial;
    private Image slotImage;
    private Tween slotTextTween;
    
    public Achievement Achievement { get; private set; }
    private AchievementUIController achievementUIController;
    private AchievementManager achievementManager;
    private AchievementUIView achievementUIView;

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        achievementUIController = GetComponentInParent<AchievementUIController>();
        achievementUIView = GetComponentInParent<AchievementUIView>();
        achievementManager = AchievementManager.Instance;
    }

    public void Initialize(Achievement achievement)
    {
        Achievement = achievement;
        
        if (!Achievement.isCompleted)
        {
            SubscribeEvents();
        }
        
        UpdateUI();
    }
    
    private void OnAchievementUpdated(AchievementsUpdatedEvent e)
    {
        if (e.Achievement == Achievement)
        {
            UpdateUI();
        }
    }
    
    private void SubscribeEvents()
    {
        EventBus.Subscribe<AchievementsUpdatedEvent>(OnAchievementUpdated);
    }

    private void UnsubscribeEvents()
    {
        EventBus.Unsubscribe<AchievementsUpdatedEvent>(OnAchievementUpdated);
    }

    private void ChangeSlotAchievement()
    {
        Achievement = achievementManager.currentAchievementSlots[Achievement.groupID];
    }

    private void UpdateUI()
    {
        slotDescriptionText.text = Achievement.description.Replace("n", Achievement.goalValue.ToString());
        starRewardText.text = $"{Achievement.rewardPoints}";
        UpdateProgress(Achievement.currentPoints, Achievement.goalValue);

        // IsComplete 확인 여부
        Achievement.SetIsCompleted();
        
        // Achievement 완료 여부에 따라 EnableRewardButton 또는 SetCompletedUI 호출
        if (Achievement.isCompleted && !Achievement.isRewardClaimed)
        {
            EnableRewardButton();
        }
        else if (Achievement.isCompleted && !achievementManager.CheckNextAchievement(Achievement.groupID, Achievement.order + 1))
        {
            SetCompletedUI();
            UnsubscribeEvents();
        }
    }

    private void OnClaimRewardButtonClicked()
    {
        Achievement.isRewardClaimed = true;

        // 여기서 oldValue를 먼저 가져옵니다. (추가 전 상태)
        int oldValue = achievementManager.currentAchievementTitle.currentPoints;
        int increaseValue = Achievement.rewardPoints;
        int maxValue = achievementManager.currentAchievementTitle.requiredPoints;

        // 이제 포인트 추가
        achievementManager.currentAchievementTitle.AddCurrentPoints(increaseValue);

        // 남은 업적 Title 경험치량 계산
        int remainTitlePoint = 0;
        if (achievementManager.currentAchievementTitle.CheckIsCompleted())
        {
            remainTitlePoint = achievementManager.currentAchievementTitle.currentPoints - achievementManager.currentAchievementTitle.requiredPoints;
        }

        // 파티클
        CurrencyEffectParticleController ps = GameManager.Instance.Pool.SpawnFromPool<CurrencyEffectParticleController>(E_PoolObjectType.CurrencyEffect);
        Vector3 worldPosition = starRectTransform.position;
        ps.transform.position = worldPosition;

        // 첫 번째 파티클 도착 시 AddStarReward 호출
        // 여기서 oldValue, increaseValue, maxValue, remainTitlePoint를 넘겨줌
        ps.InitParticle(CurrencyType.AchievementStar, true, true, 6, () => AddStarReward(oldValue, increaseValue, maxValue, remainTitlePoint), null);

        // 업적 슬롯 상태 변화
        if (Achievement.isCompleted && Achievement.isRewardClaimed && achievementManager.CheckNextAchievement(Achievement.groupID, Achievement.order + 1))
        {
            achievementManager.SetNextAchievement(Achievement.groupID, Achievement.order + 1);
            ChangeSlotAchievement();
            SetDefaultSlot();
            UpdateUI();
            if (!achievementManager.CheckAchievementRedDot())
            {
                achievementManager.achivementRedDotComponent.OnRedDotChecked();
            }
        }

        if (SaveManager.Instance.saveData != null)
        {
            achievementManager.Save();
        }
        else
        {
            SaveManager.Instance.SaveAll();
        }

        achievementManager.SaveCompleteDate(Achievement.groupID, Achievement.order , Achievement);
    }
    
    // 칭호 Title UI 상태 업데이트
    private void AddStarReward(int oldValue, int increaseValue, int maxValue, int remainPoints)
    {
        achievementUIView.EnqueueTitleProgressBarUpdate(oldValue, increaseValue, maxValue, remainPoints);
    }
    
    private void UpdateProgress(long currentAmount, int targetAmount)
    {
        progressText.text = $"{currentAmount} / {targetAmount}";
        float progressRatio = targetAmount > 0 ? (float)currentAmount / targetAmount : 0f;
        progressSlider.value = progressRatio;
    }

    private void SetDefaultSlot()
    {
        rewardButton.interactable = false;
        rewardButton.onClick.RemoveAllListeners();
        slotImage.sprite = ResourceManager.Instance.GetResource<Sprite>("achievements_slot_default");
        slotImage.material = null;
        slotText.text = "진행중";
        progressSlider.fillRect.GetComponent<Image>().sprite = ResourceManager.Instance.GetResource<Sprite>("big_progress_full");
        achievementUIController.UpdateSlotOrder();
        StopProgressTextTween();
    }
    
    private void EnableRewardButton()
    {
        rewardButton.interactable = true;
        rewardButton.onClick.RemoveAllListeners();
        rewardButton.onClick.AddListener(OnClaimRewardButtonClicked);
        slotImage.sprite = ResourceManager.Instance.GetResource<Sprite>("achievements_slot_complete");
        slotImage.material = glassMaterial;
        slotText.text = "획득 가능!";
        achievementUIController.UpdateSlotOrder();
        StartProgressTextTween();
    }

    private void SetCompletedUI()
    {
        rewardButton.interactable = false;
        rewardButton.onClick.RemoveAllListeners();
        slotImage.sprite = ResourceManager.Instance.GetResource<Sprite>("achievements_slot_end");
        slotImage.material = null;
        progressSlider.fillRect.GetComponent<Image>().sprite = ResourceManager.Instance.GetResource<Sprite>("progress_end");
        slotText.text = "완료";
        achievementUIController.UpdateSlotOrder();
        StopProgressTextTween();
    }
    
    private void StartProgressTextTween()
    {
        StopProgressTextTween();
        slotTextTween = slotText.rectTransform.DOScale(1.1f, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopProgressTextTween()
    {
        if (slotTextTween != null && slotTextTween.IsActive())
        {
            slotTextTween.Kill();
            slotTextTween = null;
            slotText.rectTransform.localScale = Vector3.one;
        }
    }
}