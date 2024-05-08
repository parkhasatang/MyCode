using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Slider expBar;
    public TMP_Text levelUI;
    public GameObject skillChooseUI;

    private void Awake()
    {
        instance = this;
        GameManager.Instance.InitSetting();
        SoundManager.Play("Battle");
    }

    // �������ϸ� MaxValue�� GameManager���� ������ �Լ�, ������ �Ŀ� ȣ��
    public void FitExpBar()
    {
        expBar.maxValue = (float)GameManager.Instance.amountOfExp;
    }

    public void FillExp()
    {
        expBar.value = (float)GameManager.Instance.exp;
    }

    public void RefreshLevelUI()
    {
        levelUI.text = $"{GameManager.Instance.level}";
    }
}
