using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIStrategy;
using UnityEngine.UI;

public abstract class UIView : MonoBehaviour
{
    [FoldoutGroup("UI View Settings")]
    public UIAppearance showAppearance;

    [ShowIf("@showAppearance == UIAppearance.SlideIn || showAppearance == UIAppearance.SlideFadeIn"), FoldoutGroup("UI View Settings")]
    public SlideDirection slideInDirection;
    [ShowIf("IsSlideFadeIn"), FoldoutGroup("UI View Settings")]
    public CanvasGroup canvasGroup;

    [FoldoutGroup("UI View Settings")]
    public UIHideAppearance hideAppearance;

    [ShowIf("@hideAppearance == UIHideAppearance.SlideOut || hideAppearance == UIHideAppearance.SlideFadeOut"), FoldoutGroup("UI View Settings")]
    public SlideDirection slideOutDirection;

    [FoldoutGroup("UI View Settings")]
    [FoldoutGroup("UI View Settings")]
    public float uiShowTime;
    [FoldoutGroup("UI View Settings")]
    public float uiHideTime;

    [FoldoutGroup("UI View Settings")]
    public bool backGroundCheck;
    [ShowIf("backGroundCheck"), FoldoutGroup("UI View Settings")]
    public RectTransform newRectTransform;

    protected RectTransform rectTransform;
    protected Vector2 originalPos;  // 초기 좌표를 저장할 변수

    private IShowStrategy showStrategy;
    protected IHideStrategy hideStrategy;

    [ShowInInspector] protected bool isBackBtnOn;
    protected bool isEscapeStackUsing;
    
    public List<Button> backButtons = new();
    
    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (backGroundCheck)
        {
            rectTransform = newRectTransform;
        }
        originalPos = rectTransform.anchoredPosition;  // 초기 좌표 저장
        showStrategy = ShowStrategyFactory.Create(showAppearance, this);
        hideStrategy = HideStrategyFactory.Create(hideAppearance, this);

        UIStack uiStack = GetComponent<UIStack>();
        isEscapeStackUsing = uiStack != null;

        if (backButtons != null)
        {
            foreach (var button in backButtons)
            {
                var uiName = gameObject.name;

                // "(Clone)" 접미사가 있는지 확인하고 제거
                if (uiName.EndsWith("(Clone)"))
                {
                    uiName = uiName.Replace("(Clone)", ""); // "(Clone)" 제거
                }
                button.onClick.AddListener(() => UIManager.Instance.HideUI(uiName));
            }
        }
    }

    protected virtual void Start()
    {
        Show();
    }

    private bool IsSlideIn()
    {
        return showAppearance == UIAppearance.SlideIn;
    }

    private bool IsSlideOut()
    {
        return hideAppearance == UIHideAppearance.SlideOut;
    }

    private bool IsSlideFadeIn()
    {
        return showAppearance == UIAppearance.SlideFadeIn;
    }

    private bool IsSlideFadeOut()
    {
        return hideAppearance == UIHideAppearance.SlideFadeOut;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartCoroutine(showStrategy.Execute(() =>
        {
            isBackBtnOn = true;
            if (isEscapeStackUsing)
            {
                UIManager.Instance.uiEscapeStack.isEscapeLock = false;
            }
            if (backButtons == null) return;
            foreach (var button in backButtons)
            {
                button.interactable = true;
            }
        }));
    }

    public virtual void Hide()
    {
        if (!gameObject.activeInHierarchy) return;

        if (isBackBtnOn)
        {
            isBackBtnOn = false;
            if (isEscapeStackUsing)
            {
                UIManager.Instance.uiEscapeStack.isEscapeLock = true;
            }
            if (backButtons == null) return;
            foreach (var button in backButtons)
            {
                button.interactable = false;
            }

            StartCoroutine(hideStrategy.Execute());
        }
    }

    // UIShowStrategy
    public virtual IEnumerator FadeIn(Action onComplete = null)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, uiShowTime).OnComplete(() => onComplete?.Invoke());
        yield return new WaitForSeconds(uiShowTime);
    }

    public virtual IEnumerator SlideIn(SlideDirection direction, Action onComplete = null)
    {
        Vector2 startPos = originalPos;

        switch (direction)
        {
            case SlideDirection.Left:
                startPos = new Vector2(-rectTransform.rect.width, originalPos.y);
                break;
            case SlideDirection.Right:
                startPos = new Vector2(rectTransform.rect.width, originalPos.y);
                break;
            case SlideDirection.Up:
                startPos = new Vector2(originalPos.x, rectTransform.rect.height);
                break;
            case SlideDirection.Down:
                startPos = new Vector2(originalPos.x, -rectTransform.rect.height);
                break;
        }

        rectTransform.anchoredPosition = startPos;
        rectTransform.DOAnchorPos(originalPos, uiShowTime).SetEase(Ease.OutCubic).OnComplete(() => onComplete?.Invoke());
        yield return new WaitForSeconds(uiShowTime);
    }

    public virtual IEnumerator PopUp(Action onComplete = null)
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, uiShowTime).SetEase(Ease.OutBack).OnComplete(() => onComplete?.Invoke()).SetUpdate(true);
        yield return new WaitForSecondsRealtime(uiShowTime);
    }

    // UIHideStrategy

    public IEnumerator HideNone()
    {
        gameObject.SetActive(false);
        yield break;
    }

    public virtual IEnumerator FadeOut(Action onComplete = null)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.DOFade(0, uiHideTime).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
        yield return new WaitForSeconds(uiHideTime);
    }

    public virtual IEnumerator SlideOut(SlideDirection direction, Action onComplete = null)
    {
        Vector2 endPos = originalPos;

        switch (direction)
        {
            case SlideDirection.Left:
                endPos = new Vector2(-rectTransform.rect.width, originalPos.y);
                break;
            case SlideDirection.Right:
                endPos = new Vector2(rectTransform.rect.width, originalPos.y);
                break;
            case SlideDirection.Up:
                endPos = new Vector2(originalPos.x, rectTransform.rect.height);
                break;
            case SlideDirection.Down:
                endPos = new Vector2(originalPos.x, -rectTransform.rect.height);
                break;
        }

        rectTransform.DOAnchorPos(endPos, uiHideTime).SetEase(Ease.InCubic).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
        yield return new WaitForSeconds(uiHideTime);
    }

    public virtual IEnumerator Collapse(Action onComplete = null)
    {
        rectTransform.DOScale(Vector3.zero, uiHideTime).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }).SetUpdate(true);
        yield return new WaitForSecondsRealtime(uiHideTime);
    }
}
