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

    // 이펙트를 사용하고 싶으면 해당 메서드를 통해 이펙트 오브젝트를 부여하여 사용하면 됌.
    // 사용 방법 GameObject effectPrefab = EffectManager.instance.GetEffectPrefab(EffectManager.EffectType.Attack);
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
