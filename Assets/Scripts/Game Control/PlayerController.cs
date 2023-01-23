using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumData;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get { return instance; } }
    private static PlayerController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public int MaxLife { get { return maxLife; } }
    public int Life { get { return life; } }
    public int Money { get { return money; } }

    private int maxLife;
    private int life;
    private int money;

    private List<Tower> towers;
    public List<Tower> Towers { get { return towers; } }
    public CharacterType Type { get { return character.Type; } }
    private Character character;
    public float BonusProb
    {
        get
        {
            if (Type == CharacterType.PROB)
                return (100 + GetStat(CharacterStatType.ABILITY)) / 100f;
            else return 1;
        }
    }

    public void Init(CharacterType type)
    {
        towers = new List<Tower>();
        character = new Character(type);

        life = maxLife = 100;
        money = 10000;

        UpdateInfo();
        UpdateStat();
    }

    public bool Buy(int cost)
    {
        cost = Cost(cost);

        if (money >= cost)
        {
            money -= cost;
            UpdateInfo();
            return true;
        }
        return false;
    }

    public static int Cost(int cost)
    {
        if (Instance.Type == CharacterType.COST)
        {
            cost = (int)(cost / ((100 + Instance.GetStat(CharacterStatType.ABILITY)) / 100f));
        }

        return cost;
    }

    public bool AddTower(Tower tower)
    {
        if (towers.Count == 10) return false;

        towers.Add(tower);

        return true;
    }

    public void Reinforce(CharacterStatType type)
    {
        character.Reinforce(type, 5);
        UpdateStat();
    }

    public int GetStat(CharacterStatType type)
    {
        return character.Stat[type];
    }

    public int BonusElement(Element element)
    {
        switch(element)
        {
            case Element.FIRE:
                return character.Stat[CharacterStatType.FIRE];
            case Element.WATER:
                return character.Stat[CharacterStatType.WATER];
            case Element.NATURE:
                return character.Stat[CharacterStatType.NATURE];
        }

        return 0;
    }

    public void Reward(int exp, int money)
    {
        this.money += money;
        character.GetExp(exp);
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        UIController.Instance.UpdateInfo(life, maxLife, money, character);
    }

    public void UpdateStat()
    {
        UIController.Instance.UpdateStat(character);
    }
}

public class Character
{
    public int Level { get { return level; } }
    private int level;
    public int Exp { get { return exp; } }
    private int exp;
    public int BonusStat { get { return bonusStat; } }
    private int bonusStat;

    public string TypeString { get; private set; }
    public CharacterType Type { get { return type; } }
    private CharacterType type;

    public Dictionary<CharacterStatType, int> Stat { get { return stat; } }
    private Dictionary<CharacterStatType, int> stat;

    public Character(CharacterType type)
    {
        this.type = type;
        TypeString = type.ToString();

        level = 1;
        exp = 0;
        bonusStat = 0;

        stat = new Dictionary<CharacterStatType, int>();
        for (int i = 0; i < 5; i++)
        {
            stat.Add((CharacterStatType)i, 0);
        }
        stat[CharacterStatType.ABILITY] = 10;
    }

    public void GetExp(int amount)
    {
        exp += amount;

        if (exp >= 100)
        {
            exp -= 100;
            level++;
            bonusStat += 2;
        }
    }

    public void Reinforce(CharacterStatType type, int amount)
    {
        if (bonusStat <= 0) return;

        stat[type] += amount;
        bonusStat--;
    }
}