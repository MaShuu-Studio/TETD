using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ResourceManager
{
    public static List<T> GetResources<T> (string path) where T : Object
    {
        return Resources.LoadAll<T>(path).ToList();
    }
}
