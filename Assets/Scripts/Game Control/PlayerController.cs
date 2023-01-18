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
    private Character character;

    public void Init(CharacterType type)
    {
        towers = new List<Tower>();
        character = new Character(type);

        life = maxLife = 100;
        money = 0;

        UpdateInfo();
        UpdateStat();
    }

    public bool AddTower(Tower tower)
    {
        if (towers.Count == 10) return false;

        towers.Add(tower);

        return true;
    }

    public void Reinforce(StatType type)
    {
        character.Reinforce(type, 5);
        UpdateStat();
    }

    public void LevelUp()
    {
        character.LevelUp();
        UpdateStat();
    }

    public void UpdateInfo()
    {
        UIController.Instance.UpdateInfo(life, maxLife, money);
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
    public int BonusStat { get { return bonusStat; } }
    private int bonusStat;

    public string TypeString { get; private set; }
    public CharacterType Type { get { return type; } }
    private CharacterType type;

    public Dictionary<StatType, int> Stat { get { return stat; } }
    private Dictionary<StatType, int> stat;

    public Character(CharacterType type)
    {
        this.type = type;
        TypeString = type.ToString();

        level = 1;
        bonusStat = 0;

        stat = new Dictionary<StatType, int>();
        for (int i = 0; i < 5; i++)
        {
            stat.Add((StatType)i, 0);
        }
        stat[StatType.ABILITY] = 10;
    }

    public void LevelUp()
    {
        level++;
        bonusStat += 2;
    }

    public void Reinforce(StatType type, int amount)
    {
        if (bonusStat <= 0) return;

        stat[type] += amount;
        bonusStat--;
    }
}
