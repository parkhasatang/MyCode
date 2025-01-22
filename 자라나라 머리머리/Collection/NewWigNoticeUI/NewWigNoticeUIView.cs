using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class NewWigNoticeUIView : UIView
{
    public RectTransform newImage;
    private Tween newImageTween;

    public RectTransform auraTransform;
    private readonly float auraRotationSpeed = 30f; // �ʴ� ȸ�� �ӵ� (30��)

    private void OnDisable()
    {
        newImageTween.Kill();
        newImage.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        if (isBackBtnOn)
        {
            isBackBtnOn = false;
            if (isEscapeStackUsing)
            {
                UIManager.Instance.uiEscapeStack.isEscapeLock = true;
            }

            if (backButtons != null)
            {
                foreach (var button in backButtons)
                {
                    button.interactable = false;
                }
            }

            // Hide 애니메이션 후 OnNewWigUIClose 호출
            StartCoroutine(hideStrategy.Execute());
        }
    }

    private void Update()
    {
        auraTransform.Rotate(Vector3.forward, auraRotationSpeed * Time.unscaledDeltaTime);
    }

    public override IEnumerator PopUp(Action onComplete = null)
    {
        onComplete = NewImageTween;
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, uiShowTime).SetEase(Ease.OutBack).OnComplete(() => onComplete?.Invoke()).SetUpdate(true);
        yield return new WaitForSecondsRealtime(uiShowTime + 0.5f);
    }
    
    public override IEnumerator Collapse(Action onComplete = null)
    {
        onComplete = CollectionManager.Instance.OnNewWigUIClose;
        rectTransform.DOScale(Vector3.zero, uiHideTime).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }).SetUpdate(true);
        yield return new WaitForSecondsRealtime(uiHideTime);
    }

    private void NewImageTween()
    {
        if (newImageTween != null && newImageTween.IsActive())
        {
            newImageTween.Kill();
        }
        
        newImage.gameObject.SetActive(true);
        
        newImage.localScale = Vector3.one;

        newImageTween = newImage.transform.DOScale(1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine).SetUpdate(true);
    }
}
