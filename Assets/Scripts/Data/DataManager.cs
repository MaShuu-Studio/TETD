using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;

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
                File.WriteAllText(UnityEngine.Application.streamingAssetsPath + pathes[i] + fileListName, content);
            }
        }
        public static string FileList(string path)
        {
            string text = "";
            string[] files = Directory.GetFiles(UnityEngine.Application.streamingAssetsPath + path);
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

        // ������ �÷��������� �۵���.
        // Streaming Asset���� Persistant Path�� ������ ��°�� �̵���ų ����� ã�� �ʿ䰡 ���� �� ��.
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
            string[] files = Directory.GetFiles(UnityEngine.Application.streamingAssetsPath + path);
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
        public static string SerializeJson<T>(T obj)
        {
            string json = JsonUtility.ToJson(obj);
            return json;
        }

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
            if (fileName.Contains(".json") == false) fileName += ".json";
            path = UnityEngine.Application.streamingAssetsPath + Path.Combine(path, fileName);

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
            string path = Path.Combine(UnityEngine.Application.persistentDataPath, settingFile);
            string text = setting.ToString();

            File.WriteAllText(path, text);
        }

        public static Setting LoadSetting()
        {
            string path = Path.Combine(UnityEngine.Application.persistentDataPath, settingFile);

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
        public static void SaveCustomData(JsonData data, string dataPath, Dictionary<string, List<Sprite>> sprites)
        {
            try
            {
                string imagePath = UnityEngine.Application.streamingAssetsPath + data.imgsrc;

                // ������ ������ ���� ���� ���� ���� �� �����
                if (Directory.Exists(imagePath))
                {
                    DirectoryInfo dir = new DirectoryInfo(imagePath);
                    foreach (var file in dir.GetFiles())
                    {
                        file.Delete();
                    }
                }
                // ������ ���� ����
                else Directory.CreateDirectory(imagePath);

                foreach (string type in sprites.Keys)
                {
                    for (int i = 0; i < sprites[type].Count; i++)
                    {
                        string fileName = type.ToUpper() + i.ToString() + ".png";
                        Sprite sprite = sprites[type][i];

                        File.WriteAllBytes(imagePath + fileName, ImageConversion.EncodeToPNG(sprite.texture));

                        // IDLE�� ù������� ���� �ϳ� �� ����
                        if (type.ToUpper() == "IDLE" && i == 0)
                            File.WriteAllBytes(imagePath + type.ToUpper() + ".png", ImageConversion.EncodeToPNG(sprite.texture));
                    }
                }

                // ������ ���� ����
                string json = File.ReadAllText(dataPath);
                string content = SerializeJson(data);

                string findString = data.id.ToString(); // �ش� ����� Ž���ϴ� ���� Ȱ��
                string idString = "\"id\""; // ����� ���� Ž���ϱ� ���� ���� ����� Ž���ϴ� ���� Ȱ��.

                int idIndex = json.IndexOf(findString);
                if (idIndex > 0)
                {

                    /* �ϳ��� ����� { }���� �̷���� ����.
                     * �ٸ�, ��� �������� { }�� �����ϱ� ������ �̸� �̿��ؼ��� Ž���� �� ����.
                     * ���, id�� �ϳ��� �����ϸ� ��� ���� �� �ϳ��� ������.
                     * ���� id�� ������ �ش�Ǵ� ����� Ž���ϰ� �� ���� {�� ã���� ����� ������ Ž����.
                     * ���� id�� �����ϴ� index�� Ž���ϸ� ���� ����� ã�� �� ����.
                     * ���� ����� �� �� �������� }�� Ž���ϸ� �ش� ����� ���� ã�� �� ����.
                     */

                    int nextIndex = json.IndexOf(idString, idIndex + 1);

                    int startIndex = json.LastIndexOf("{", idIndex);
                    int lastIndex = json.LastIndexOf("}", nextIndex);

                    json = json.Remove(startIndex, lastIndex - startIndex + 1).Insert(startIndex, content);
                }
                else
                {
                    int startIndex = json.LastIndexOf("]");
                    json = json.Insert(startIndex, "," + content);
                }

                File.WriteAllText(dataPath, json);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"{e}");
#endif
            }
        }

        public static async Task<List<Sprite>> FindSprites()
        {
            List<Sprite> sprites = new List<Sprite>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "image files(*.png, *.jpg)|*.png;*.jpg";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var path in openFileDialog.FileNames)
                    {
                        Sprite sprite = await LoadSprite(path);
                        if (sprite != null) sprites.Add(sprite);
                    }
                }
            }

            return sprites;
        }

        private static async Task<Sprite> LoadSprite(string path)
        {
            Sprite sprite = null;
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
            {
                req.SendWebRequest();
                try
                {
                    while (!req.isDone) await Task.Yield();
                    if (string.IsNullOrEmpty(req.error))
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(req);
                        texture.filterMode = FilterMode.Point;

                        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2, 24);
                    }
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.Log($"{e}");
#endif
                }
            }
            return sprite;
        }

        public static async Task<Sprite> LoadSprite(string path, Vector2 pivot, float pixelsPerUnit)
        {
            path = UnityEngine.Application.streamingAssetsPath + path;
            Sprite sprite = null;
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(path))
            {
                req.SendWebRequest();
                try
                {
                    while (!req.isDone) await Task.Yield();
                    if (string.IsNullOrEmpty(req.error) == false) return null;

                    Texture2D texture = DownloadHandlerTexture.GetContent(req);
                    texture.filterMode = FilterMode.Point;

                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.Log($"{e}");
#endif
                }
            }

            return sprite;
        }
        public static async Task<AudioClip> LoadSound(string path, AudioType type)
        {
            path = UnityEngine.Application.streamingAssetsPath + path;

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
#if UNITY_EDITOR
                    Debug.Log($"{e}");
#endif
                }
            }

            return clip;
        }

        public static async Task<CustomRuleTile> LoadTile(string path, string name, string type, bool buildable)
        {
            CustomRuleTile ruleTile = null;

            Sprite origin = await LoadSprite(path, Vector2.one / 2, 24);

            if (origin == null) return null;

            CustomTile[] tiles = new CustomTile[16];
            Texture2D texture = origin.texture;

            int width = texture.width / 4;
            int height = texture.height / 4;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 3; y >= 0; y--)
                {
                    CustomTile tile = ScriptableObject.CreateInstance<CustomTile>();
                    Rect rect = new Rect(x * width, y * height, width, height);
                    Sprite sprite = Sprite.Create(texture, rect, Vector2.one / 2, width);

                    tile.SetData(name, sprite, buildable);
                    tiles[x + 4 * (3 - y)] = tile;
                }
            }
            ruleTile = new CustomRuleTile(name, type, tiles);

            return ruleTile;
        }
        #endregion
    }
}
