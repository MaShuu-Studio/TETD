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

    public List<int>[,] UsableTowers { get { return usableTowers; } }
    private List<int>[,] usableTowers;
    public List<Tower> Towers { get { return towers; } }
    private List<Tower> towers;
    public CharacterType Type { get { return character.Type; } }
    private Character character;

    private float probDiff;
    private float costDiff;

    public float BonusProb
    {
        get
        {
            float prob = probDiff;
            if (Type == CharacterType.PROB)
                return prob * ((100 + character.Ability) / 100f);
            else return prob;
        }
    }

    public void Init(CharacterType type, List<DifficultyType> difficulties)
    {
        probDiff = 1;
        costDiff = 1;

        if (difficulties.Contains(DifficultyType.PROB)) probDiff = 0.5f;
        if (difficulties.Contains(DifficultyType.COST)) costDiff = 1.5f;

        towers = new List<Tower>();
        usableTowers = new List<int>[EnumArray.Elements.Length, EnumArray.Grades.Length];

        for (int i = 0; i < usableTowers.GetLength(0); i++)
        {
            for (int j = 0; j < usableTowers.GetLength(1); j++)
            {
                usableTowers[i, j] = new List<int>();
                usableTowers[i, j].AddRange(TowerManager.EgTowerIds[i, j]);
            }
        }

        character = new Character(type, new Element[3] { Element.FIRE, Element.WATER, Element.NATURE });

        life = maxLife = 10;
        money = 10000;

        UpdateInfo();
        UpdateStat();
    }

    public void Damaged(int damage)
    {
        life -= damage;
        CameraController.Instance.ShakeCamera(0.1f);
        UIController.Instance.Flash(new Color(0.5f, 0, 0), 0.2f);
        if (life < 0) life = 0;
        UpdateInfo();
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
        float c = cost;
        if (Instance.Type == CharacterType.COST)
        {
            c /= ((100 + Instance.character.Ability) / 100f);
        }

        c *= Instance.costDiff;

        return (int)c;
    }

    public bool AddTower(Tower tower)
    {
        if (towers.Count == 10) return false;

        towers.Add(tower);
        usableTowers[(int)tower.element, (int)tower.grade].Remove(tower.id);

        return true;
    }

    public void Reinforce(int index, Character.ElementStatType type)
    {
        character.Reinforce(index, type, 5);
        UpdateStat();
    }

    public int GetAbility()
    {
        return character.Ability;
    }
    public int BonusElement(Element element, Character.ElementStatType type)
    {
        return character.GetStat(element, type);
    }

    public void Reward(int exp, int money)
    {
        if (character.Type == CharacterType.REWARD)
        {
            exp = (int)(exp * (100 + character.Ability) / 100f);
            money = (int)(money * (100 + character.Ability) / 100f * (100 + character.Ability) / 100f);
        }

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

    public int Ability { get { return ability; } }
    private int ability;

    public enum ElementStatType { DMG = 0, ATTACKSPEED };
    private ElementStat[] stat;
    private Element[] statElements;

    public int GetStat(Element element, ElementStatType type)
    {
        for (int i = 0; i < stat.Length; i++)
        {
            if (statElements[i] == element)
            {
                if (type == ElementStatType.DMG) return stat[i].dmg;
                else return stat[i].attackspeed;
            }
        }
        return 0;
    }

    public Character(CharacterType type, Element[] elements)
    {
        this.type = type;
        TypeString = type.ToString();

        level = 1;
        exp = 0;
        bonusStat = 20;
        ability = 50;

        statElements = elements;
        stat = new ElementStat[elements.Length];
        for (int i = 0; i < stat.Length; i++)
            stat[i] = new ElementStat()
            {
                dmg = 0,
                attackspeed = 0
            };
    }

    public void GetExp(int amount)
    {
        exp += amount;

        if (exp >= 10)
        {
            UIController.Instance.ShowTutorial(5);
            exp -= 100;
            level++;
            bonusStat += 2;
            ability += 2;
        }
    }

    public void Reinforce(int index, ElementStatType type, int amount)
    {
        if (bonusStat <= 0) return;

        stat[index].AddStat(type, amount);
        bonusStat--;
    }

    public struct ElementStat
    {
        public int dmg;
        public int attackspeed;

        public int GetStat(ElementStatType type)
        {
            if (type == ElementStatType.DMG) return dmg;
            else return attackspeed;
        }

        public void AddStat(ElementStatType type, int amount)
        {
            if (type == ElementStatType.DMG) dmg += amount;
            else attackspeed += amount;
        }
    }
}