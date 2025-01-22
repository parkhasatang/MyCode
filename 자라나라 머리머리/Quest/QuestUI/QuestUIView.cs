using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestUIView : UIView
{
    public RectTransform starTransform;
    public TMP_Text questResetTime; // 퀘스트 리셋 시간 표시용 Text
    public Slider questProgressBar; // 퀘스트 진행 바
    public TMP_Text currentStarCountText; // 현재 별 수

    public RectTransform questSlotContentTransform; // QuestSlot이 들어갈 Content

    // 진행 바 애니메이션을 위한 Tween 변수 추가
    private Tween progressBarTween;

    // Coroutine을 위한 변수
    private Coroutine updateResetTimeCoroutine;
    
    private QuestUIController questUIController;

    private void OnEnable()
    {
        // UI가 활성화될 때 코루틴 시작
        updateResetTimeCoroutine = StartCoroutine(UpdateQuestResetTimeCoroutine());

        // 두번 째 켜질 때부터 작동
        if (questUIController != null)
        {
            questUIController.SetQuestUIInteractable(true);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        questUIController = GetComponent<QuestUIController>();
        if (backButtons != null)
        {
            foreach (var button in backButtons)
            {
                button.onClick.AddListener(CloseBoxRewardPopup);
                button.onClick.AddListener(() => GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true));
            }
        }
    }

    private void OnDisable()
    {
        // UI가 비활성화될 때 코루틴 중지
        if (updateResetTimeCoroutine != null)
        {
            StopCoroutine(updateResetTimeCoroutine);
            updateResetTimeCoroutine = null;
        }
    }

    private IEnumerator UpdateQuestResetTimeCoroutine()
    {
        while (true)
        {
            questResetTime.text = TimeManager.Instance.UpdateResetTime();
            yield return new WaitForSeconds(1f); // 1초마다 업데이트
        }
    }

    public void UpdateCurrentStarCountText(int starCount)
    {
        currentStarCountText.text = starCount.ToString();
    }

    private void UpdateQuestProgressBar(int currentValue, int maxValue)
    {
        float progressRatio = maxValue > 0 ? Mathf.Clamp01((float)currentValue / maxValue) : 0f;

        // 기존 Tween이 있으면 종료
        if (progressBarTween != null && progressBarTween.IsActive())
        {
            progressBarTween.Kill();
            progressBarTween = null;
        }

        // 진행 바 애니메이션 시작
        progressBarTween = questProgressBar.DOValue(progressRatio, 0.5f).SetEase(Ease.OutCubic);
    }

    public void UpdateStarUI(int currentStarCount, int totalStarCount)
    {
        UpdateCurrentStarCountText(currentStarCount);
        UpdateQuestProgressBar(currentStarCount, totalStarCount);
    }

    private void CloseBoxRewardPopup()
    {
        BoxRewardNoticeUIView boxRewardNoticeUIView = UIManager.Instance?.GetUIView<BoxRewardNoticeUIView>("BoxRewardNoticeUI");
        if (boxRewardNoticeUIView != null && boxRewardNoticeUIView.gameObject.activeInHierarchy)
        {
            boxRewardNoticeUIView.gameObject.SetActive(false);
        }
    }
}
