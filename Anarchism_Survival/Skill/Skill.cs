using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    public SkillData data;
    public int level;

    public Image icon;
    public TMP_Text description;

    private void OnEnable()
    {
        // 먼저 data에 스크립터블 오브젝트 넣어줘야함.
        if (data != null)
        {
            icon.sprite = data.itemIcon;
            
            description.text = $"<size=50>Lv.{level + 1}<size=30><br>{data.itemDescription}";
        }
    }

    public void Onclick()
    {
        for (int i = 0; i < SkillManager.instance.weapons.Length; i++)
        {
            if (SkillManager.instance.weapons[i].skillData.itemType == data.itemType)
            {
                SkillManager.instance.weapons[i].level++;
            }
            else
            {
                switch (data.itemType)
                {
                    case SkillData.ItemType.Heal:
                        // 플레이어 체력 회복
                        Debug.Log("체력회복");
                        return;
                    case SkillData.ItemType.Coin:
                        // 플레이어 주화 증가
                        Debug.Log("주화증가");
                        return;
                }
            }
            GameManager.Instance.ResumeGameAfterSkillSelection();// 게임 다시 진행
        }
    }
}
