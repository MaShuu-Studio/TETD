using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using EnumData;
using System.Threading.Tasks;

public static class SpriteManager
{
    private static Dictionary<int, Sprite> sprites;
    private static Dictionary<string, Sprite> uiSprites;

    public static async Task Init()
    {
        sprites = new Dictionary<int, Sprite>();
        uiSprites = new Dictionary<string, Sprite>();

        // UI를 ID로 넘기게 되면 조정
        for (int i = 0; i < EnumArray.Elements.Length; i++)
        {
            string name = $"Element{i}";
            Sprite sprite = await DataManager.LoadSprite("/Sprites/UI/" + name + ".png", Vector2.one / 2, 16);
            sprite.name = name;
            uiSprites.Add(name, sprite);
        }

        for (int i = 0; i < EnumArray.TowerStatTypes.Length; i++)
        {
            string name = $"TowerStatType{i}";
            Sprite sprite = await DataManager.LoadSprite("/Sprites/UI/" + name + ".png", Vector2.one / 2, 16);
            sprite.name = name;
            uiSprites.Add(name, sprite);
        }

        for (int i = 0; i < EnumArray.BuffTypes.Length; i++)
        {
            string name = $"BuffType{i}";
            Sprite sprite = await DataManager.LoadSprite("/Sprites/UI/" + name + ".png", Vector2.one / 2, 16);
            if (sprite == null) continue;
            sprite.name = name;
            uiSprites.Add(name, sprite);
        }

        for (int i = 0; i < EnumArray.DebuffTypes.Length; i++)
        {
            string name = $"DebuffType{i}";
            Sprite sprite = await DataManager.LoadSprite("/Sprites/UI/" + name + ".png", Vector2.one / 2, 16);
            if (sprite == null) continue;
            sprite.name = name;
            uiSprites.Add(name, sprite);
        }
    }

    public static async Task AddSprite<T>(string path, int id, Vector2 pivot, float pixelPerUnit)
    {
        if (sprites.ContainsKey(id)) return;

        string type = typeof(T).ToString();

        Sprite sprite = await DataManager.LoadSprite(path + "IDLE.png", pivot, pixelPerUnit);
        if (sprite == null) return;
        sprite.name = id.ToString();
        sprites.Add(id, sprite);
    }

    public static Sprite GetSprite(int id)
    {
        if (sprites.ContainsKey(id)) return sprites[id];

        return null;
    }

    public static Sprite GetSprite(string name)
    {
        if (uiSprites.ContainsKey(name)) return uiSprites[name];

        return null;
    }
}