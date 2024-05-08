using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectMapping
{
    public EffectManager.EffectType type;
    public GameObject effectPrefab;
}
public class EffectManager : MonoBehaviour
{
    public enum EffectType
    {
        Attack,
        Mining
    }
    public static EffectManager instance;
    public EffectPool effectPool;

    public EffectMapping[] effectMappings;


    public void Awake()
    {
        instance = this;
    }

    // ����Ʈ�� ����ϰ� ������ �ش� �޼��带 ���� ����Ʈ ������Ʈ�� �ο��Ͽ� ����ϸ� ��.
    // ��� ��� GameObject effectPrefab = EffectManager.instance.GetEffectPrefab(EffectManager.EffectType.Attack);
    public GameObject GetEffectPrefab(EffectType type)
    {
        foreach (EffectMapping mapping in effectMappings)
        {
            if (mapping.type == type)
            {
                return mapping.effectPrefab;
            }
        }
        return null;
    }
}
