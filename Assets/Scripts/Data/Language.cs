using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;

public class Language
{
    public int id;
    public Dictionary<LanguageType, string> name;
    // description �� �ٸ� �������� �ʿ信 ���� �߰�

    public Language(LanguageData data)
    {
        id = data.id;
        name = new Dictionary<LanguageType, string>();
        for (int i = 0; i < data.name.Length; i++)
        {
            name.Add((LanguageType)i, data.name[i]);
        }
    }
}
[Serializable]
public class LanguageData
{
    public int id;
    public string[] name;
}