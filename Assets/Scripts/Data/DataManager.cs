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

        public static List<T> GetResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(path).ToList();
        }

        #region Get Files
        public static void MakeFileNameList()
        {
            string[] pathes =
            {
                "/Data/Map/",
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

        // 윈도우 플랫폼에서만 작동함.
        // Streaming Asset에서 Persistant Path로 파일을 통째로 이동시킬 방법을 찾을 필요가 있을 듯 함.
        public static string[] GetDics(string path)
        {
            string[] dics = Directory.GetDirectories(path);
            string[] names = new string[dics.Length];
            for (int i = 0; i < dics.Length; i++)
            {
                names[i] = Path.GetFileName(dics[i]);
            }
            return names;
        }

        public static List<string> GetFileNames(string path)
        {
            string[] files = Directory.GetFiles(Application.streamingAssetsPath + path);
            List<string> names = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(".meta") || files[i].Contains(".txt")) continue;
                names.Add(Path.GetFileName(files[i]));
            }

            return names;
        }
        #endregion

        #region Json
        public static void SerializeJson<T>(string path, string fileName, T obj)
        {
            string json = JsonUtility.ToJson(obj);
            if (fileName.Contains(".json") == false) fileName += ".json";
            path = Path.Combine(path, fileName);

            File.WriteAllText(path, json);
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
        #endregion

        #region Data
        private const string settingFile = "setting.ini";

        public static void SaveSetting(Setting setting)
        {
            string path = Path.Combine(Application.persistentDataPath, settingFile);
            string text = setting.ToString();

            File.WriteAllText(path, text);
        }

        public static Setting LoadSetting()
        {
            string path = Path.Combine(Application.persistentDataPath, settingFile);

            if (File.Exists(path) == false) return null;

            Setting setting = new Setting();

            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line != null)
                {
                    var trimStart = line.TrimStart();

                    if (trimStart.Length > 0)
                    {
                        if (trimStart[0] != '[')
                        {
                            string key;
                            int value;
                            Setting.TrimingValue(line, out key, out value);
                            setting.AddOption(key, value);
                        }
                    }
                }
            }

            return setting;
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
        #endregion
    }
}
