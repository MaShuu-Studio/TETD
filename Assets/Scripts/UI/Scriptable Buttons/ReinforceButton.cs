using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class ReinforceButton : ScriptableButton
{
    [SerializeField] private StatType type;
    protected override void ClickEvent()
    {
        PlayerController.Instance.Reinforce(type);
    }
}
