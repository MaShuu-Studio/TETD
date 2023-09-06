using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using EnumData;
using System.Threading.Tasks;

public static class SpriteManager
{
    public static bool isLoad { get { return sprites != null; } }
    private static Dictionary<int, Sprite> sprites;
    public static int CurProgress { get; private set; } = 0;
    public static int TotalProgress { get; private set; }
    public static async Task GetTotal()
    {
        TotalProgress = 0;
        foreach (string dataName in datas)
        {
            List<int> list = await DataManager.DeserializeListJson<int>(dataPath, dataName);
            TotalProgress += list.Count;
        }
    }

    public static string dataPath { get; private set; } = "/Data/";
    public static string path { get; private set; } = "/Sprites/";

    private static string[] datas = { "Stat" };

    // 기본 넘버만으로 데이터를 로드할 수 있도록 합해서 id를 뽑을 수 있는 고유 넘버를 보관
    public enum ETCDataNumber
    {
        TYPE = 3000000, ELEMENT = 3001000, GRADE = 3002000, BUFF = 3003000, DEBUFF = 3004000,
        CHARTYPE = 3100000, DIFF = 3101000,
        TOWERSTAT = 3200000, APRIORITY = 3201000,
        ENEMYSTAT = 3300000, ENEMYGRADE = 3301000,
    }

    public static async Task Init()
    {
        /* ETC Data id 넘버링
            - STAT: 3ABBCCC
                - A: INFO
                    - 0: Public
                    - 1: Character
                    - 2: Tower
                    - 3: Enemy
                - B: Type
                    - A: 0 (Public)
                        - 0: Type (Tower or Enemy)
                        - 1: ELEMENT
                        - 2: Grade
                        - 3: BUFF
                        - 4: DEBUFF
                    - A: 1 (Character)
                        - 0: Character Type
                        - 1: Difficulty
                        - 
                    - A: 2 (Tower)
                        - 0: TOWERSTAT
                        - 1: ATTACKPRIORITY
                    - A: 3 (Enemy)
                        - 0: ENEMYSTAT
                        - 1: ENEMYGRADE
                - C: 번호

            - UI: 4AABBBB
                - A: UI에서는 Scene으로 구분. 0: basic, 1: Title, 2: InGame
                - B: 번호
         */
        sprites = new Dictionary<int, Sprite>();

        // etc로 분류된 데이터들의 스프라이트 유무를 체크하여 가져옴.
        foreach (string dataName in datas)
        {
            List<int> list = await DataManager.DeserializeListJson<int>(dataPath, dataName);

            foreach (int id in list)
            {
                CurProgress++;
                Sprite sprite = await DataManager.LoadSprite(path + dataName + "/" + id + ".png", Vector2.one / 2, 16);
                if (sprite == null) continue;
                sprites.Add(id, sprite);
            }
        }

#if UNITY_EDITOR
        Debug.Log($"[SYSTEM] LOAD ETC SPRITES");
#endif
    }

    public static async Task AddSprite<T>(string path, int id, Vector2 pivot, float pixelPerUnit)
    {
        if (sprites.ContainsKey(id)) return;

        string type = typeof(T).ToString();

        Sprite sprite = await DataManager.LoadSprite(path + "IDLE.png", pivot, pixelPerUnit);
        if (sprite == null)
        {
            sprite = await DataManager.LoadSprite(path + "IDLE0.png", pivot, pixelPerUnit);
            if (sprite == null) return;
        }
        sprite.name = id.ToString();
        sprites.Add(id, sprite);
    }

    public static Sprite GetSprite(int id)
    {
        if (sprites.ContainsKey(id)) return sprites[id];

        return null;
    }

    public static Sprite GetSpriteWithNumber(ETCDataNumber data, int number)
    {
        int id = (int)data + number;
        return GetSprite(id);
    }

    public static void AddData(int id, Sprite sprite)
    {
        sprites.Add(id, sprite);
    }
    public static void RemoveData(int id)
    {
        sprites.Remove(id);
    }
}