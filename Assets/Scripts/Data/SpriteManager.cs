using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class SpriteManager
{
    private static string path = Application.streamingAssetsPath + "/Sprites/";

    private static Dictionary<int, Sprite> sprites;

    public static void Init()
    {
        sprites = new Dictionary<int, Sprite>();
    }

    public static void AddSprite<T>(int id, Vector2 pivot, int pixelPerUnit)
    {
        if (sprites.ContainsKey(id)) return;

        string type = typeof(T).ToString();

        Sprite sprite = DataManager.LoadSprite(path + $"{type}/{id}.png", pivot, pixelPerUnit);
        sprites.Add(id, sprite);
    }

    public static Sprite GetSprite(int id)
    {
        if (sprites.ContainsKey(id)) return sprites[id];

        return null;
    }
}