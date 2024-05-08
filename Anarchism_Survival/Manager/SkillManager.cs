using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public List<Skill> skills = new List<Skill>();
    public Weapon[] weapons;
    [SerializeField] internal List<SkillData> skillDatas = new List<SkillData>(); // 플레이 할 때, 지급되는 스킬을 넣고 빼는 리스트

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

        List<SkillData> availableSkills = new List<SkillData>(skillDatas); // 사용 가능한 스킬 목록을 복사

        for (int i = 0; i < skills.Count; i++)
        {
            if (availableSkills.Count == 0)
                break; // 남은 스킬이 없다면 반복 종료

            int randomIndex = Random.Range(0, availableSkills.Count);
            skills[i].data = availableSkills[randomIndex]; // 랜덤하게 스킬 선택

            // 선택된 스킬을 사용 가능한 목록에서 제거
            availableSkills.RemoveAt(randomIndex);

            // 스킬 레벨을 무기 레벨로 설정
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
