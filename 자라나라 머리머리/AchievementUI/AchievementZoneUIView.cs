using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementZoneUIView : UIView
{
    public TMP_Text achievementTitleText;
    public Image medalImage;
    
    public TitleSlotMachine titleSlotMachine;
    
    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.RegisterUIView(gameObject.name, this);
    }

    protected override void Start()
    {
        SkillManager.Instance.OnStartSkillEvent += AchievementSkillOnEvent;
        SkillManager.Instance.OnSkillOffEvent += AchievementSkillOffEvent;
    }

    public void Init(string title, int level)
    {
        achievementTitleText.text = title;

        if (level >= 1 && level <= 5)
        {
            medalImage.sprite = ResourceManager.Instance.GetResource<Sprite>("구리 메달");
        }
        else if (level >= 6 && level <= 10)
        {
            medalImage.sprite = ResourceManager.Instance.GetResource<Sprite>("은 메달");
        }
        else if (level >= 11 && level <= 15)
        {
            medalImage.sprite = ResourceManager.Instance.GetResource<Sprite>("금 메달");
        }
        else if (level >= 16 && level <= 20)
        {
            medalImage.sprite = ResourceManager.Instance.GetResource<Sprite>("다이아 메달");
            if (level == 20)
            {
                achievementTitleText.text = "그래도 대머리는 돈다\n코페르니쿠스 대머리";
            }
        }
        else
        {
            medalImage.sprite = ResourceManager.Instance.GetResource<Sprite>("다이아 메달");
        }
    }

    private void AchievementSkillOnEvent()
    {
        gameObject.SetActive(false);
    }
    
    private void AchievementSkillOffEvent()
    {
        gameObject.SetActive(true);
    }
}
