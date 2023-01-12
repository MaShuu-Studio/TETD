using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SerializableList<T>
    {
        public List<T> list;
    }

    public static class DataManager
    {
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
    }
}
