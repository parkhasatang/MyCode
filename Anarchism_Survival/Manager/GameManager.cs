using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]internal double amountOfExp;
    public Transform playerPosition;
    public Player player;

    private int Level;
    internal int level
    {
        get { return Level; }
        set
        {
            Level = value;
            UIManager.instance.FitExpBar(); // �ִ� ����ġ���� �°� Slider ����
        }
    }

    private double Exp;
    public double exp
    {
        get { return Exp; }
        set
        {
            Exp = value;
            CheckTotalExp(); // ����ġ �� ä�������� �˻�, �� �Ŀ� Slider UI������Ʈ
            UIManager.instance.FillExp(); // Slider value ����
        }
    }

    public SaveData saveData;
    public ColleagueStat[] colleagueDefaultStats = new ColleagueStat[3];
    public bool IsIntroPlayed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        SaveData.Save(saveData);
    }

    public void LoadGame()
    {
        saveData = SaveData.Load();
        if (saveData == null)
        {
            saveData = new();
        }
    }

    public void InitSetting()
    {
        player = FindObjectOfType<Player>();
        playerPosition = player.gameObject.GetComponent<Transform>();
        level = 1;
        amountOfExp = 600;
        UIManager.instance.FitExpBar();
    }

    public void CheckTotalExp()
    {
        if (exp >= amountOfExp)
        {
            exp -= amountOfExp;
            level++;
            UIManager.instance.RefreshLevelUI(); // ������ �Ŀ� ���� UI ������Ʈ
            SkillManager.instance.SetSkillChoose();
            PauseGameForSkillSelection();
            amountOfExp = 600 * level;
        }
    }

    public void PauseGameForSkillSelection()
    {
        Time.timeScale = 0; // ���� �ð��� ����
        UIManager.instance.skillChooseUI.SetActive(true); // ��ų ���� UI Ȱ��ȭ
    }

    public void ResumeGameAfterSkillSelection()
    {
        Time.timeScale = 1; // ���� �ð��� �������� ����
        UIManager.instance.skillChooseUI.SetActive(false); // ��ų ���� UI ��Ȱ��ȭ
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }
}
