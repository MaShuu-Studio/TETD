using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace Data
{
    public class SerializableList<T>
    {
        public List<T> list;
    }

    public static class DataManager
    {
        public static List<T> GetResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(path).ToList();
        }

        public static List<string> GetFiles(string path, string type)
        {
            List<string> files = new List<string>();
            string[] pathes = Directory.GetFiles(path, "*" + type);

            for (int i = 0; i < pathes.Length; i++)
            {
                string file = Path.GetFileName(pathes[i]).Replace(type, "").ToUpper();
                files.Add(file);
            }
            return files;
        }

        public static void SerializeJson<T>(string path, string fileName, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            fileName += ".json";

            File.WriteAllText(path + fileName, json);
        }

        public static T DeserializeJson<T>(string path, string fileName)
        {
            fileName += ".json";
            string json = File.ReadAllText(path + fileName);

            T obj = JsonUtility.FromJson<T>(json);

            return obj;
        }

        public static List<T> DeserializeListJson<T>(string path, string fileName)
        {
            fileName += ".json";
            string json = File.ReadAllText(path + fileName);

            SerializableList<T> obj = JsonUtility.FromJson<SerializableList<T>>(json);

            return obj.list;
        }

        public static async Task<Sprite> LoadSprite(string path, Vector2 pivot, float pixelsPerUnit)
        {
            path = Application.streamingAssetsPath + "/Sprites" + path;
            Sprite sprite = null;
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Delay(5);

                    Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                    texture = DownloadHandlerTexture.GetContent(req);
                    texture.filterMode = FilterMode.Point;

                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

            return sprite;
        }
        public static async Task<AudioClip> LoadSound(string path, string name, AudioType type)
        {
            path = Application.streamingAssetsPath + "/Sounds" + path;

            AudioClip clip = null;
            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(path, type))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Delay(5);

                    clip = DownloadHandlerAudioClip.GetContent(req);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

            return clip;
        }
    }
}
