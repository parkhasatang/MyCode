using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public SkillData skillData;
    [SerializeField]private int Level;
    public int level
    {
        get { return Level; }
        set 
        { 
            Level = value;
            CheckWeaponLevel();
        }
    }
    public int prefabId;
    public float damage;
    public int count;
    internal Bullet bullet;
    internal Fire fire;
    internal WeaponAttacking weaponAttacking;
    private Coroutine myCoroutine;

    private void Start()
    {
        for (int i = 0; i < PoolManager.instance.prefabs.Length; i++)
        {
            if (skillData.projectile)
            {
                if (PoolManager.instance.prefabs[i] == skillData.projectile)
                {
                    prefabId = i; // �̸��� ������ �ش� �ε����� prefabId�� �Ҵ�
                    bullet = PoolManager.instance.prefabs[prefabId].GetComponent<Bullet>();
                    weaponAttacking = GetComponent<WeaponAttacking>();
                    fire = PoolManager.instance.prefabs[3].transform.GetComponent<Fire>();
                    break; // ��ġ�ϴ� �׸��� ã�����Ƿ� ���� ����
                }
            }
        }
    }

    public void CheckWeaponLevel()
    {
        damage = skillData.baseDamage * skillData.damages[level - 1];
        count = skillData.counts[level - 1];

        // Bullet ������Ʈ�� �����ϸ� Init �޼ҵ带 ȣ��
        if (bullet != null)
        {
            bullet.Init(damage);
            if (myCoroutine != null)
            {
                StopCoroutine(myCoroutine);
            }
            switch (skillData.itemType)
            {
                case SkillData.ItemType.Torch:
                    weaponAttacking.Batch();
                    myCoroutine = StartCoroutine(weaponAttacking.Rotate());
                    break;
                case SkillData.ItemType.Rifle:
                    myCoroutine = StartCoroutine(weaponAttacking.Shoot(GameManager.Instance.player.attckSpeed));
                    break;
                case SkillData.ItemType.Firebomb:
                    fire.Init(damage);
                    myCoroutine = StartCoroutine(weaponAttacking.RandomThrow(GameManager.Instance.player.attckSpeed));
                    break;
                

            }
        }
        else
        {
            switch (skillData.itemType)
            {
                case SkillData.ItemType.Glove:
                    GameManager.Instance.player.attckSpeed -= skillData.damages[level - 1];
                    Debug.Log(GameManager.Instance.player.attckSpeed);
                    break;
                case SkillData.ItemType.Shoe:
                    GameManager.Instance.player.GetComponent<PlayerMove>().moveSpeed += skillData.damages[level - 1];
                    Debug.Log(GameManager.Instance.player.GetComponent<PlayerMove>().moveSpeed);
                    break;
            }
        }

        if (level >= 5)
        {
            for (int i = 0; i < SkillManager.instance.skillDatas.Count; i++)
            {
                if (SkillManager.instance.skillDatas[i] == skillData)
                {
                    SkillManager.instance.RemoveSkillData(i);
                    return;
                }
            }
        }
    }
}
