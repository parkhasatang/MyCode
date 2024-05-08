using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public List<Skill> skills = new List<Skill>();
    public Weapon[] weapons;
    [SerializeField] internal List<SkillData> skillDatas = new List<SkillData>(); // �÷��� �� ��, ���޵Ǵ� ��ų�� �ְ� ���� ����Ʈ

    private void Awake()
    {
        instance = this;
    }

    public void SetSkillChoose()
    {
        if (skillDatas.Count <=2)
        {
            skills[2].transform.gameObject.SetActive(false);
            skills.Remove(skills[2]);
        }

        List<SkillData> availableSkills = new List<SkillData>(skillDatas); // ��� ������ ��ų ����� ����

        for (int i = 0; i < skills.Count; i++)
        {
            if (availableSkills.Count == 0)
                break; // ���� ��ų�� ���ٸ� �ݺ� ����

            int randomIndex = Random.Range(0, availableSkills.Count);
            skills[i].data = availableSkills[randomIndex]; // �����ϰ� ��ų ����

            // ���õ� ��ų�� ��� ������ ��Ͽ��� ����
            availableSkills.RemoveAt(randomIndex);

            // ��ų ������ ���� ������ ����
            for (int j = 0; j < weapons.Length; j++)
            {
                if (skills[i].data == weapons[j].skillData)
                {
                    skills[i].level = weapons[j].level;
                }
            }
        }
    }

    public void RemoveSkillData(int index)
    {
        skillDatas.Remove(skillDatas[index]);
    }
}
