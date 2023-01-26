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

        public static async Task<List<string>> GetFiles(string path, string type)
        {
            List<string> files = new List<string>();
            
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

            string[] pathes = Directory.GetFiles(path, "*" + type);
            for (int i = 0; i < pathes.Length; i++)
            {
                string file = Path.GetFileName(pathes[i]).Replace(type, "").ToUpper();
                files.Add(file);
            }
            return files;
        }

        public static async void SerializeJson<T>(string path, string fileName, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            fileName += ".json";
            path = Path.Combine(path, fileName);

            using (UnityWebRequest req = UnityWebRequest.Put(path, json))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }
        }

        public static async Task<T> DeserializeJson<T>(string path, string fileName)
        {
            fileName += ".json";
            path = Path.Combine(path, fileName);

            T obj = default(T);

            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();
                    string json = req.downloadHandler.text;
                    obj = JsonUtility.FromJson<T>(json);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

            return obj;
        }

        public static async Task<List<T>> DeserializeListJson<T>(string path, string fileName)
        {
            fileName += ".json";
            path = Path.Combine(path, fileName);

            SerializableList<T> obj = null;

            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();
                    string json = req.downloadHandler.text;
                    obj = JsonUtility.FromJson<SerializableList<T>>(json);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

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
                    while (!req.isDone) await Task.Yield();

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
                    while (!req.isDone) await Task.Yield();

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
