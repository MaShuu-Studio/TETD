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
        private const string fileListName = "FILELIST.txt";
        private const string fileListSplit = ",\n";

        public static void MakeFileNameList()
        {
            string[] pathes =
            {
                "/Data/Map/",
                "/Sprites/Tile/",
                "/Sounds/"
            };
            for (int i = 0; i < pathes.Length; i++)
            {
                string content = FileList(pathes[i]);
                File.WriteAllText(Application.streamingAssetsPath + pathes[i] + fileListName, content);
            }
        }
        public static string FileList(string path)
        {
            string text = "";
            string[] files = Directory.GetFiles(Application.streamingAssetsPath + path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(".meta") || files[i].Contains(".txt")) continue;
                string file = Path.GetFileName(files[i]);
                text += path + file + fileListSplit;
            }
            text = text.Substring(0, text.Length - fileListSplit.Length);
            return text;
        }
        public static string FileNameTriming(string fileName)
        {
            string[] split = fileName.Split("/");
            fileName = split[split.Length - 1];
            split = fileName.Split(".");
            fileName = split[0];

            return fileName;
        }
        public static List<T> GetResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(path).ToList();
        }

        public static async Task<string[]> GetFiles(string path)
        {
            string[] files = null;
            path += fileListName;
            using (UnityWebRequest req = UnityWebRequest.Get(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();
                    files = req.downloadHandler.text.Split(fileListSplit);
                }
                catch (Exception e)
                {
                    Debug.Log($"{e}");
                }
            }

            return files;
        }

        public static async void SerializeJson<T>(string path, string fileName, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (fileName.Contains(".json") == false) fileName += ".json";
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
            if (fileName.Contains(".json") == false) fileName += ".json";
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
            path = Application.streamingAssetsPath + path;
            Sprite sprite = null;
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
            {
                req.SendWebRequest();

                try
                {
                    while (!req.isDone) await Task.Yield();

                    Texture2D texture = DownloadHandlerTexture.GetContent(req);
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
        public static async Task<AudioClip> LoadSound(string path, AudioType type)
        {
            path = Application.streamingAssetsPath + path;

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
