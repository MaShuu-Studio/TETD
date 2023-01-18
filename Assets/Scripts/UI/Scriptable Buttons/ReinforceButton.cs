using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class ReinforceButton : ScriptableButton
{
    [SerializeField] private CharacterStatType type;
    protected override void ClickEvent()
    {
        PlayerController.Instance.Reinforce(type);
    }
}
