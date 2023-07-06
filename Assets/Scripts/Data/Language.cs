using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;
using System;

public class Language
{
    public int id;
    public string name;
    // description �� �ٸ� �������� �ʿ信 ���� �߰�

    public Language(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public Language(LanguageData data)
    {
        id = data.id;
        name = data.name;
    }
}
[Serializable]
public class LanguageData
{
    public int id;
    public string name;
}