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
    public int maxTimer = 30; // �ִ� Ÿ�̸� �ð�
    public float gameTimer;   // ���� Ÿ�̸� (float ����)
    public Slider timerSlider; // UI �����̴�

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
            yield return null; // ���� �����ӱ��� ��ٸ�
        }

        // Ÿ�̸Ӱ� 0�� �������� �� ������ ����
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
        // ���� ���� �����̳� �ٸ� Ʈ���Ÿ� ���⿡ ��ġ
    }

    private void DetermineTeacherState()
    {
        int chance = Random.Range(0, 100);
        if (chance < 20)  // 20% Ȯ���� ����ǥ, ������ ��
        {
            teacherState = TeacherState.Watching;
        }
        else if (chance < 50)  // 30% Ȯ���� ����ǥ, �� ���ɼ� ����
        {
            teacherState = TeacherState.ExclamationMark;
        }
        else  // 50% Ȯ���� �� ��
        {
            teacherState = TeacherState.NotWathcing;
        }
    }
}
