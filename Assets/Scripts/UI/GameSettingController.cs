using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using EnumData;

public class GameSettingController : MonoBehaviour
{
    [SerializeField] private List<GameSettingIcon> charIcons;
    [SerializeField] private List<GameSettingIcon> difficultIcons;
    [SerializeField] private ToggleGroup mapInfos;
    [SerializeField] private MapInfoIcon mapIconPrefab;
    private List<MapInfoIcon> mapIcons;

    public async Task Init()
    {
        // 추후 해당 부분을 데이터화 및 자동 생성시켜 작동시킬 예정
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

        if (mapIcons != null)
        {
            foreach (var icon in mapIcons)
            {
                Destroy(icon.gameObject);
            }
            mapIcons.Clear();
        }
        mapIcons = new List<MapInfoIcon>();

        while (MapManager.Maps == null || TileManager.TilePaletteNames == null) await Task.Yield();

        for (int i = 0; i < MapManager.Maps.Count; i++)
        {
            string mapName = MapManager.Maps[i];
            Map map = await MapManager.LoadMap(mapName);
            MapInfoIcon mapIcon = Instantiate(mapIconPrefab);
            mapIcon.transform.SetParent(mapInfos.transform);
            mapIcon.SetIcon(map, mapInfos);
            mapIcon.isOn = false;

            mapIcons.Add(mapIcon);
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
