using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TeacherState
{
    NotWathcing,
    ExclamationMark,
    QuestionMark,
    Watching
}
public class DanceMiniGameManager : MonoBehaviour
{
    public int maxTimer = 30; // 최대 타이머 시간
    public float gameTimer;   // 게임 타이머 (float 유지)
    public Slider timerSlider; // UI 슬라이더

    public TeacherState teacherState = TeacherState.NotWathcing;

    public int maxTeacherLooks = 7;
    private int teacherLookCount = 0;
    private float nextLookTime = 0;


    private void Start()
    {
        gameTimer = maxTimer;
        timerSlider.maxValue = maxTimer;
        timerSlider.value = maxTimer;

        StartCoroutine(RunTimer());
    }

    private void Update()
    {
        
    }

    private IEnumerator RunTimer()
    {
        while (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
            timerSlider.value = gameTimer;
            yield return null; // 다음 프레임까지 기다림
        }

        // 타이머가 0에 도달했을 때 실행할 로직
        OnTimerEnd();
    }

    public void AddTime(float timeToAdd)
    {
        gameTimer += timeToAdd;

        if (gameTimer > maxTimer)
        {
            gameTimer = maxTimer;
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("Timer ended!");
        // 게임 오버 로직이나 다른 트리거를 여기에 배치
    }

    private void DetermineTeacherState()
    {
        int chance = Random.Range(0, 100);
        if (chance < 20)  // 20% 확률로 느낌표, 무조건 봄
        {
            teacherState = TeacherState.Watching;
        }
        else if (chance < 50)  // 30% 확률로 물음표, 봄 가능성 있음
        {
            teacherState = TeacherState.ExclamationMark;
        }
        else  // 50% 확률로 안 봄
        {
            teacherState = TeacherState.NotWathcing;
        }
    }
}
