using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SerializableObject<T>
    {
        public T obj;
        public SerializableObject(T o)
        {
            obj = o;
        }
    }

    public static class DataManager
    {
        public static void SerializeJson<T>(string path, string fileName, T obj)
        {
            string json = JsonUtility.ToJson(new SerializableObject<T>(obj));
            fileName += ".json";

            File.WriteAllText(path + fileName, json);
        }

        public static T DeserializeJson<T>(string path, string fileName)
        {
            fileName += ".json";
            string json = File.ReadAllText(path + fileName);

            SerializableObject<T> obj = JsonUtility.FromJson<SerializableObject<T>>(json);

            return obj.obj;
        }
    }
}
