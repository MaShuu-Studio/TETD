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
        public static void Serialize<T>(string path, T obj)
        {
            string json = JsonUtility.ToJson(new SerializableObject<T>(obj));
            string fileName = path + typeof(T).Name + ".json";

            File.WriteAllText(fileName, json);
        }
        public static T Deserialize<T>(string path)
        {
            string fileName = path + typeof(T).Name + ".json";
            string json = File.ReadAllText(fileName);

            SerializableObject<T> obj = JsonUtility.FromJson<SerializableObject<T>>(json);

            return obj.obj;
        }
    }
}
