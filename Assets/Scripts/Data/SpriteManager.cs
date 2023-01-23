using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Threading.Tasks;

public static class SpriteManager
{
    private static Dictionary<int, Sprite> sprites;

    public static void Init()
    {
        sprites = new Dictionary<int, Sprite>();
    }

    public static async Task AddSprite<T>(string path, int id, Vector2 pivot, float pixelPerUnit)
    {
        if (sprites.ContainsKey(id)) return;

        string type = typeof(T).ToString();

        Sprite sprite = await DataManager.LoadSprite(path, pivot, pixelPerUnit);
        sprites.Add(id, sprite);
    }

    public static Sprite GetSprite(int id)
    {
        if (sprites.ContainsKey(id)) return sprites[id];

        return null;
    }
}