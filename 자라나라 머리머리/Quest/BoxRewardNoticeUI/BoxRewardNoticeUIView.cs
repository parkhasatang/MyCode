using UnityEngine;
using System.Collections;

public class BoxRewardNoticeUIView : UIView
{
    private float autoHideDelay = 2f; // 자동으로 사라지기까지의 시간 (2초)
    private Coroutine autoHideCoroutine;

    private void OnEnable()
    {
        // UI가 활성화될 때 자동 숨김 코루틴 시작
        autoHideCoroutine = StartCoroutine(AutoHideAfterDelay());
    }

    private void OnDisable()
    {
        // UI가 비활성화될 때 코루틴 정지
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
    }

    private IEnumerator AutoHideAfterDelay()
    {
        yield return new WaitForSeconds(autoHideDelay);
        UIManager.Instance.HideUI("BoxRewardNoticeUI");
    }
}
