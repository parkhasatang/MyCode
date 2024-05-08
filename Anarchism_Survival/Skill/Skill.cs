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
        // ���� data�� ��ũ���ͺ� ������Ʈ �־������.
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
                        // �÷��̾� ü�� ȸ��
                        Debug.Log("ü��ȸ��");
                        return;
                    case SkillData.ItemType.Coin:
                        // �÷��̾� ��ȭ ����
                        Debug.Log("��ȭ����");
                        return;
                }
            }
            GameManager.Instance.ResumeGameAfterSkillSelection();// ���� �ٽ� ����
        }
    }
}
