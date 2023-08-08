using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class ReinforceButton : ScriptableButton
{
    [SerializeField] private int index;
    [SerializeField] private Character.ElementStatType type;
    protected override void ClickEvent()
    {
        PlayerController.Instance.Reinforce(index, type);
    }
}
