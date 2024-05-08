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
            UIManager.instance.FitExpBar(); // 최대 경험치량에 맞게 Slider 조절
        }
    }

    private double Exp;
    public double exp
    {
        get { return Exp; }
        set
        {
            Exp = value;
            CheckTotalExp(); // 경험치 다 채워졌는지 검사, 그 후에 Slider UI업데이트
            UIManager.instance.FillExp(); // Slider value 조절
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
            UIManager.instance.RefreshLevelUI(); // 레벨업 후에 레벨 UI 업데이트
            SkillManager.instance.SetSkillChoose();
            PauseGameForSkillSelection();
            amountOfExp = 600 * level;
        }
    }

    public void PauseGameForSkillSelection()
    {
        Time.timeScale = 0; // 게임 시간을 정지
        UIManager.instance.skillChooseUI.SetActive(true); // 스킬 선택 UI 활성화
    }

    public void ResumeGameAfterSkillSelection()
    {
        Time.timeScale = 1; // 게임 시간을 정상으로 복구
        UIManager.instance.skillChooseUI.SetActive(false); // 스킬 선택 UI 비활성화
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }
}
