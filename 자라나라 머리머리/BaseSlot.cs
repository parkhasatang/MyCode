using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseSlot : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] protected RectTransform starRectTransform;
    [SerializeField] protected TMP_Text progressText;
    [SerializeField] protected Slider progressSlider;
    [SerializeField] protected TMP_Text slotDescriptionText;
    [SerializeField] protected TMP_Text starRewardText;
    [SerializeField] protected Button rewardButton;
    [SerializeField] protected Material glassMaterial;

    protected Image slotImage;
    protected Tween progressTextTween;

    protected virtual void Awake()
    {
        InitializeComponents();
    }

    protected virtual void InitializeComponents()
    {
        slotImage = GetComponent<Image>();
    }

    /// <summary>
    /// UI 갱신 로직을 구현하는 추상 메서드
    /// </summary>
    public abstract void UpdateUI();

    /// <summary>
    /// 보상 버튼 클릭 시 호출될 메서드
    /// </summary>
    protected abstract void OnClaimRewardButtonClicked();

    protected void UpdateProgress(int currentAmount, int targetAmount)
    {
        progressText.text = $"{currentAmount} / {targetAmount}";
        float progressRatio = targetAmount > 0 ? (float)currentAmount / targetAmount : 0f;
        progressSlider.value = progressRatio;
    }

    protected void StartProgressTextTween()
    {
        StopProgressTextTween();
        progressTextTween = progressText.rectTransform.DOScale(1.1f, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    protected void StopProgressTextTween()
    {
        if (progressTextTween != null && progressTextTween.IsActive())
        {
            progressTextTween.Kill();
            progressTextTween = null;
            progressText.rectTransform.localScale = Vector3.one;
        }
    }

    protected void EnableRewardButton(string completeSpriteName = "quest_slot_complete")
    {
        rewardButton.onClick.RemoveAllListeners();
        rewardButton.onClick.AddListener(OnClaimRewardButtonClicked);
        slotImage.sprite = ResourceManager.Instance.GetResource<Sprite>(completeSpriteName);
        slotImage.material = glassMaterial;
        progressText.text = "획득 가능!";
        StartProgressTextTween();
    }

    protected void SetCompletedUI(string endSpriteName = "quest_slot_end", string endProgressSpriteName = "progress_end")
    {
        slotImage.sprite = ResourceManager.Instance.GetResource<Sprite>(endSpriteName);
        slotImage.material = null;
        progressSlider.fillRect.GetComponent<Image>().sprite = ResourceManager.Instance.GetResource<Sprite>(endProgressSpriteName);
        progressText.text = "완료";
        StopProgressTextTween();
    }
}
