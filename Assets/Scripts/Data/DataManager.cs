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

        public static async Task<List<CustomData>> LoadCustomDataList(string path)
        {
            string[] subDataDics = { "/Tower/", "/Enemy/", "/Map/" };
            List<CustomData> datas = new List<CustomData>();
            string[] dataNames = GetDics(UnityEngine.Application.streamingAssetsPath + path);
            
            foreach(string dataName in dataNames)
            {
                CustomData data = new CustomData();
                data.name = dataName;
                // Tower와 Enemy의 경우는 ObjectData로 통일되어 있음.
                // 가볍게 로드하기 위해 데이터가 좀 더 적은 ObjectData로 로드할 수 있도록 진행.
                for (int i = 0; i < subDataDics.Length; i++)
                {
                    string dpath = path + dataName + subDataDics[i];
                    if (Directory.Exists(UnityEngine.Application.streamingAssetsPath + dpath) == false) continue;

                    List<string> fileNames = GetFileNames(dpath);

                    // Tower와 Enemy의 경우 폴더 내에서 세부 분리가 되어있을 수 있음.
                    if (i < 2)
                    {
                        foreach (string fileName in fileNames)
                        {
                            List<JsonData> ods = await DeserializeListJson<JsonData>(dpath, fileName);
                            data.dataAmount[i] += ods.Count;
                        }
                    }
                    else data.dataAmount[i] = fileNames.Count;
                }

                if (data.dataAmount[0] != 0 || data.dataAmount[1] != 0 || data.dataAmount[2] != 0)
                {
                    datas.Add(data);
                }
            }

            return datas;
        }

        public static void SaveCustomData(JsonData data, string dataPath, Dictionary<string, List<Sprite>> sprites, Language[] langs)
        {
            try
            {
                // Data
                {
                    string imagePath = UnityEngine.Application.streamingAssetsPath + data.imgsrc;

                    // 폴더가 있으면 안의 파일 전부 삭제 후 재생성
                    if (Directory.Exists(imagePath))
                    {
                        DirectoryInfo dir = new DirectoryInfo(imagePath);
                        foreach (var file in dir.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    // 없으면 폴더 생성
                    else Directory.CreateDirectory(imagePath);

                    foreach (string type in sprites.Keys)
                    {
                        for (int i = 0; i < sprites[type].Count; i++)
                        {
                            string fileName = type.ToUpper() + i.ToString() + ".png";
                            Sprite sprite = sprites[type][i];
                            if (sprite == null) continue;
                            File.WriteAllBytes(imagePath + fileName, ImageConversion.EncodeToPNG(sprite.texture));

                            // IDLE의 첫번쨰라면 따로 하나 더 저장
                            if (type.ToUpper() == "IDLE" && i == 0)
                                File.WriteAllBytes(imagePath + type.ToUpper() + ".png", ImageConversion.EncodeToPNG(sprite.texture));
                        }
                    }

                    // 데이터 파일 수정
                    string json = File.ReadAllText(dataPath);
                    string content = SerializeJson(data);

                    string findString = data.id.ToString(); // 해당 블록을 탐색하는 데에 활용
                    string idString = "\"id\""; // 블록의 끝을 탐색하기 위해 다음 블록을 탐색하는 데에 활용.

                    int idIndex = json.IndexOf(findString);
                    if (idIndex > 0)
                    {
                        /* 하나의 블록은 { }으로 이루어져 있음.
                         * 다만, 블록 내에서도 { }가 존재하기 떄문에 이를 이용해서는 탐색할 수 없음.
                         * 대신, id는 하나만 존재하며 블록 내에 단 하나만 존재함.
                         * 따라서 id를 기입해 해당되는 블록을 탐색하고 앞 쪽의 {를 찾으면 블록의 시작이 탐색됨.
                         * 다음 id가 존재하는 index를 탐색하면 다음 블록을 찾을 수 있음.
                         * 다음 블록의 앞 쪽 방향으로 }를 탐색하면 해당 블록의 끝을 찾을 수 있음.
                         */

                        int nextIndex = json.IndexOf(idString, idIndex + 1);
                        if (nextIndex < 0) nextIndex = json.LastIndexOf("]");

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

                // Language
                {
                    List<string> langPath = new List<string>();

                    langPath = GetFileNames(Translator.path);

                    for (int i = 0; i < langPath.Count; i++)
                    {
                        langPath[i] = UnityEngine.Application.streamingAssetsPath + Translator.path + langPath[i];

                        // 데이터 파일 수정
                        string json = File.ReadAllText(langPath[i]);
                        string content = SerializeJson(langs[i]);


                        string findString = data.id.ToString(); // 해당 블록을 탐색하는 데에 활용
                        string idString = "\"id\""; // 블록의 끝을 탐색하기 위해 다음 블록을 탐색하는 데에 활용.

                        int idIndex = json.IndexOf(findString);
                        if (idIndex > 0)
                        {
                            /* 하나의 블록은 { }으로 이루어져 있음.
                             * 다만, 블록 내에서도 { }가 존재하기 떄문에 이를 이용해서는 탐색할 수 없음.
                             * 대신, id는 하나만 존재하며 블록 내에 단 하나만 존재함.
                             * 따라서 id를 기입해 해당되는 블록을 탐색하고 앞 쪽의 {를 찾으면 블록의 시작이 탐색됨.
                             * 다음 id가 존재하는 index를 탐색하면 다음 블록을 찾을 수 있음.
                             * 다음 블록의 앞 쪽 방향으로 }를 탐색하면 해당 블록의 끝을 찾을 수 있음.
                             */

                            int nextIndex = json.IndexOf(idString, idIndex + 1);
                            if (nextIndex < 0) nextIndex = json.LastIndexOf("]");

                            int startIndex = json.LastIndexOf("{", idIndex);
                            int lastIndex = json.LastIndexOf("}", nextIndex);

                            json = json.Remove(startIndex, lastIndex - startIndex + 1).Insert(startIndex, content);
                        }
                        else
                        {
                            int startIndex = json.LastIndexOf("]");
                            json = json.Insert(startIndex, "," + content);
                        }

                        File.WriteAllText(langPath[i], json);
                    }
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"{e}");
#endif
            }
        }

        public static void RemoveCustomData(int id, string dataPath, string imgPath)
        {
            try
            {
                // Data
                {
                    // 폴더가 있으면 안의 파일 전부 삭제 후 재생성
                    if (Directory.Exists(imgPath))
                    {
                        DirectoryInfo dir = new DirectoryInfo(imgPath);
                        foreach (var file in dir.GetFiles())
                        {
                            file.Delete();
                        }
                    }

                    // 데이터 파일 수정
                    string json = File.ReadAllText(dataPath);

                    string findString = id.ToString(); // 해당 블록을 탐색하는 데에 활용
                    string idString = "\"id\""; // 블록의 끝을 탐색하기 위해 다음 블록을 탐색하는 데에 활용.

                    int idIndex = json.IndexOf(findString);
                    if (idIndex > 0)
                    {
                        /* 하나의 블록은 { }으로 이루어져 있음.
                         * 다만, 블록 내에서도 { }가 존재하기 떄문에 이를 이용해서는 탐색할 수 없음.
                         * 대신, id는 하나만 존재하며 블록 내에 단 하나만 존재함.
                         * 따라서 id를 기입해 해당되는 블록을 탐색하고 앞 쪽의 {를 찾으면 블록의 시작이 탐색됨.
                         * 다음 id가 존재하는 index를 탐색하면 다음 블록을 찾을 수 있음.
                         * 다음 블록의 앞 쪽 방향으로 }를 탐색하면 해당 블록의 끝을 찾을 수 있음.
                         */


                        int nextIndex = json.IndexOf(idString, idIndex + 1);
                        if (nextIndex < 0)
                            nextIndex = json.LastIndexOf("]");

                        int startIndex = json.LastIndexOf(",", idIndex);
                        int lastIndex = json.LastIndexOf("}", nextIndex);

                        json = json.Remove(startIndex, lastIndex - startIndex + 1);
                        File.WriteAllText(dataPath, json);
                    }
                }

                // Language
                {
                    List<string> langPath = new List<string>();

                    langPath = GetFileNames(Translator.path);

                    for (int i = 0; i < langPath.Count; i++)
                    {
                        langPath[i] = UnityEngine.Application.streamingAssetsPath + Translator.path + langPath[i];

                        // 데이터 파일 수정
                        string json = File.ReadAllText(langPath[i]);

                        string findString = id.ToString(); // 해당 블록을 탐색하는 데에 활용
                        string idString = "\"id\""; // 블록의 끝을 탐색하기 위해 다음 블록을 탐색하는 데에 활용.

                        int idIndex = json.IndexOf(findString);
                        if (idIndex > 0)
                        {
                            /* 하나의 블록은 { }으로 이루어져 있음.
                             * 다만, 블록 내에서도 { }가 존재하기 떄문에 이를 이용해서는 탐색할 수 없음.
                             * 대신, id는 하나만 존재하며 블록 내에 단 하나만 존재함.
                             * 따라서 id를 기입해 해당되는 블록을 탐색하고 앞 쪽의 {를 찾으면 블록의 시작이 탐색됨.
                             * 다음 id가 존재하는 index를 탐색하면 다음 블록을 찾을 수 있음.
                             * 다음 블록의 앞 쪽 방향으로 }를 탐색하면 해당 블록의 끝을 찾을 수 있음.
                             */


                            int nextIndex = json.IndexOf(idString, idIndex + 1);
                            if (nextIndex < 0)
                                nextIndex = json.LastIndexOf("]");

                            int startIndex = json.LastIndexOf(",", idIndex);
                            int lastIndex = json.LastIndexOf("}", nextIndex);

                            json = json.Remove(startIndex, lastIndex - startIndex + 1);
                            File.WriteAllText(langPath[i], json);
                        }
                    }
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.Log($"{e}");
#endif
            }
        }

        private static OpenFileDialog openFileDialog =
            new OpenFileDialog()
            {
                InitialDirectory = "c:\\",
                Filter = "image files(*.png, *.jpg)|*.png;*.jpg",
                Title = "Find Image",
                FilterIndex = 1,
                Multiselect = true,
                RestoreDirectory = true
            };

        private static string lastDir = "c:\\";
        public static async Task<List<Sprite>> FindSprites()
        {
            List<Sprite> sprites = new List<Sprite>();

            openFileDialog.InitialDirectory = lastDir;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileNames.Length > 0)
                    lastDir = Path.GetDirectoryName(openFileDialog.FileNames[0]);

                foreach (var path in openFileDialog.FileNames)
                {
                    Sprite sprite = await LoadSprite(path);
                    if (sprite != null) sprites.Add(sprite);
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
