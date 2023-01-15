using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumData;

public class GameSettingController : MonoBehaviour
{
    [SerializeField] private List<GameSettingIcon> charIcons;
    [SerializeField] private List<GameSettingIcon> difficultIcons;
    [SerializeField] private List<GameSettingIcon> mapIcons;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        // ���� �ش� �κ��� ������ȭ �� �ڵ� �������� �۵���ų ����
        for (int i = 0; i < charIcons.Count; i++)
        {
            charIcons[i].SetIcon(((CharacterType)i).ToString());
            charIcons[i].isOn = false;
        }
        for (int i = 0; i < difficultIcons.Count; i++)
        {
            difficultIcons[i].SetIcon(((DifficultyType)i).ToString());
            difficultIcons[i].isOn = false;
        }
        for (int i = 0; i < mapIcons.Count; i++)
        {
            string name = MapManager.Maps[i];
            mapIcons[i].SetIcon(name);
            mapIcons[i].isOn = false;
        }

        charIcons[0].isOn = true;
        mapIcons[0].isOn = true;
    }

    public CharacterType SelectedCharacter()
    {
        for (int i = 0; i < charIcons.Count; i++)
        {
            if (charIcons[i].isOn) return (CharacterType)i;
        }
        return (CharacterType)0;
    }

    public List<DifficultyType> Difficulty()
    {
        List<DifficultyType> list = new List<DifficultyType>();

        for (int i = 0; i < difficultIcons.Count; i++)
        {
            if (difficultIcons[i].isOn) list.Add((DifficultyType)i);
        }

        return list;
    }

    public string MapName()
    {
        for (int i = 0; i < mapIcons.Count; i++)
        {
            if (mapIcons[i].isOn) return MapManager.Maps[i];
        }
        return MapManager.Maps[0];
    }
}