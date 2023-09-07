using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;

public class Language
{
    public int id;
    public string name;
    public string desc;

    public Language(int id, string name, string desc = null)
    {
        this.id = id;
        this.name = name;
        this.desc = desc;
    }

    public Language(LanguageData data)
    {
        id = data.id;
        name = data.name;
        desc = data.desc;
    }
}

[Serializable]
public class LanguageData
{
    public int id;
    public string name;
    public string desc;
}