using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class TitleSlotMachine : MonoBehaviour
{
    [SerializeField] private RectTransform contentParent; // 텍스트들이 들어있는 부모
    [SerializeField] private float slotDuration = 1.5f;
    [SerializeField] private float scrollSpeed = 500f; // px per second
    [SerializeField] private TMP_Text[] candidateTitles; // 슬롯머신 리스트로 쓸 텍스트들
    [SerializeField] private TMP_Text finalTitle;        // 최종 칭호 텍스트
    [SerializeField] private GameObject blockImage;      // 슬롯 진행중일 때 터치 막기용
    [SerializeField] private GameObject infoBackBoard;
    
    //임시용
    private bool isForceQuit;

    private Coroutine slotRoutine;

    // 예시: StartSlotMachine 호출 시 strings를 받아 candidateTitles를 세팅, 마지막에 finalTitle 설정
    public void StartSlotMachine(string[] strings, string finalTitleText)
    {
        if (slotRoutine != null)
            StopCoroutine(slotRoutine);

        // 슬롯 진행 중 터치 막기
        if (blockImage != null)
            blockImage.SetActive(true);

        // strings 배열을 랜덤으로 섞는다
        ShuffleArray(strings);
        
        // candidateTitles[0]과 candidateTitles[candidateTitles.Length-1]이 동일한 텍스트를 가져서 자연스럽게 순환되도록
        if (strings.Length > 0)
        {
            for (int i = 0; i < candidateTitles.Length; i++)
            {
                // 인덱스를 strings 길이에 맞게 모듈로 처리
                candidateTitles[i].text = strings[i % strings.Length];
            }
            // 첫 텍스트 = 마지막 텍스트 같게
            candidateTitles[candidateTitles.Length - 1].text = candidateTitles[0].text;
        }

        // contentParent 초기 위치 0
        contentParent.anchoredPosition = new Vector2(25, 0);

        // 슬롯 코루틴 시작
        slotRoutine = StartCoroutine(RunSlotMachine(finalTitleText));
    }

    private IEnumerator RunSlotMachine(string finalTitleText)
    {
        if (isForceQuit)
        {
            ForceQuitSlotMachine(finalTitleText);
        }
        else
        {
            GameManager.Instance.Player.PluckController.SetPointerInputEnabled(false);
            float elapsed = 0f;
            float resetDownPosition = -200f;
            float resetUpPosition = 200f;

            // 슬롯머신 회전 단계 (빠르게 스크롤)
            while (elapsed < slotDuration)
            {
                // 아래로 스크롤
                contentParent.anchoredPosition -= new Vector2(0, scrollSpeed * Time.deltaTime);

                // y가 -200까지 내려갔으면 200으로 설정하여 다시 위로 올려 놓기
                if (contentParent.anchoredPosition.y < resetDownPosition)
                {
                    Vector2 pos = contentParent.anchoredPosition;
                    pos.y += resetUpPosition - resetDownPosition; 
                    // -200보다 아래로 갔으면 200으로 올려주는 식으로 반복
                    pos.y = 200f; 
                    contentParent.anchoredPosition = pos;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 여기서 최종 칭호를 0 위치로 맞추기
            contentParent.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutCubic);

            // 최종 칭호 설정
            finalTitle.text = finalTitleText;
            UIManager.Instance.GetUIView<AchievementUIView>("AchievementUI").UpdateTitle(AchievementManager.Instance.currentAchievementTitle.titleName);
            infoBackBoard.SetActive(true);
        
            // 정렬이 끝난 후, 두근거리는 애니메이션 예시 (scale punch)
            yield return new WaitForSeconds(0.6f);
            finalTitle.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1f).OnComplete(() =>
            {
                var zoneUI = UIManager.Instance.GetUIView<AchievementZoneUIView>("AchievementZoneUI");
                zoneUI.Init(finalTitle.text, AchievementManager.Instance.currentAchievementTitle.level);
            });
        
            // 뻠핑 기다리기
            yield return new WaitForSeconds(1f);

            // 슬롯 종료 후 블록 이미지 비활성
            if (blockImage != null)
                blockImage.SetActive(false);

            GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true);
            transform.SetAsFirstSibling();
            infoBackBoard.SetActive(false);
        
            slotRoutine = null;
        }
    }
    
    private void ShuffleArray(string[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (array[k], array[n]) = (array[n], array[k]);
        }
    }

    private void ForceQuitSlotMachine(string finalTitleText)
    {
        var zoneUI = UIManager.Instance.GetUIView<AchievementZoneUIView>("AchievementZoneUI");
        zoneUI.Init(finalTitleText, AchievementManager.Instance.currentAchievementTitle.level);
            
        contentParent.anchoredPosition = new Vector2(25, 0);
        finalTitle.transform.localScale = Vector3.one;
            
        slotRoutine = null;
        blockImage.SetActive(false);
        transform.SetAsFirstSibling();
        infoBackBoard.SetActive(false);
        // 혹시 몰라 추가
        GameManager.Instance.Player.PluckController.SetPointerInputEnabled(true);
    }

    public void SetForceQuit(bool forceQuit)
    {
        isForceQuit = forceQuit;
    }
}
