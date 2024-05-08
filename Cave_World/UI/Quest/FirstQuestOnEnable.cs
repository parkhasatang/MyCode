using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuestOnEnable : MonoBehaviour
{
    private Quest quest;
    private void Awake()
    {
        quest = GetComponentInParent<Quest>();
    }

    private void Start()
    {
        quest.SetMainQuest(0);
        quest.SetQuestImgAndTxt();
    }
}
