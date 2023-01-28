using System;
using System.Collections.Generic;

namespace EnumData
{
  public enum CharacterType { POWER = 0, ATTACKSPEED = 1, COST = 2, PROB = 3, }
  public enum DifficultyType { HP = 0, SPEED = 1, AMOUNT = 2, TIME = 3, COST = 4, PROB = 5, }
  public enum Element { FIRE = 0, WATER = 1, NATURE = 2, }
  public enum Grade { NORMAL = 0, RARE = 1, HEROIC = 2, LEGENDARY = 3, }
  public enum AttackPriority { FIRST = 0, LAST = 1, STRONG = 2, WEAK = 3, ELEMENT = 4, DEBUFF = 5, }
  public enum TowerStatType { DAMAGE = 0, ATTACKSPEED = 1, DISTANCE = 2, GOLDMINE = 3, MULTISHOT = 4, SPLASH = 5, DOTDAMAGE = 6, SLOW = 7, }
  public enum CharacterStatType { ABILITY = 0, REWARD = 1, FIRE = 2, WATER = 3, NATURE = 4, }

    public static class EnumArray
    {
        public static CharacterType[] CharacterTypes { get; private set; } = (CharacterType[])Enum.GetValues(typeof(CharacterType));
public static DifficultyType[] DifficultyTypes { get; private set; } = (DifficultyType[])Enum.GetValues(typeof(DifficultyType));
public static Element[] Elements { get; private set; } = (Element[])Enum.GetValues(typeof(Element));
public static Grade[] Grades { get; private set; } = (Grade[])Enum.GetValues(typeof(Grade));
public static AttackPriority[] AttackPrioritys { get; private set; } = (AttackPriority[])Enum.GetValues(typeof(AttackPriority));
public static TowerStatType[] TowerStatTypes { get; private set; } = (TowerStatType[])Enum.GetValues(typeof(TowerStatType));
public static CharacterStatType[] CharacterStatTypes { get; private set; } = (CharacterStatType[])Enum.GetValues(typeof(CharacterStatType));
public static Dictionary<CharacterType,string> CharacterTypeStrings { get; private set; }
public static Dictionary<DifficultyType,string> DifficultyTypeStrings { get; private set; }
public static Dictionary<Element,string> ElementStrings { get; private set; }
public static Dictionary<Grade,string> GradeStrings { get; private set; }
public static Dictionary<AttackPriority,string> AttackPriorityStrings { get; private set; }
public static Dictionary<TowerStatType,string> TowerStatTypeStrings { get; private set; }
public static Dictionary<CharacterStatType,string> CharacterStatTypeStrings { get; private set; }


        public static void Init()
        {
            
            CharacterTypeStrings = new Dictionary<CharacterType, string>();
            for (int i = 0; i < CharacterTypes.Length; i++)
            {
                CharacterType type = CharacterTypes[i];
                CharacterTypeStrings.Add(type, type.ToString()); 
            }

            DifficultyTypeStrings = new Dictionary<DifficultyType, string>();
            for (int i = 0; i < DifficultyTypes.Length; i++)
            {
                DifficultyType type = DifficultyTypes[i];
                DifficultyTypeStrings.Add(type, type.ToString()); 
            }

            ElementStrings = new Dictionary<Element, string>();
            for (int i = 0; i < Elements.Length; i++)
            {
                Element type = Elements[i];
                ElementStrings.Add(type, type.ToString()); 
            }

            GradeStrings = new Dictionary<Grade, string>();
            for (int i = 0; i < Grades.Length; i++)
            {
                Grade type = Grades[i];
                GradeStrings.Add(type, type.ToString()); 
            }

            AttackPriorityStrings = new Dictionary<AttackPriority, string>();
            for (int i = 0; i < AttackPrioritys.Length; i++)
            {
                AttackPriority type = AttackPrioritys[i];
                AttackPriorityStrings.Add(type, type.ToString()); 
            }

            TowerStatTypeStrings = new Dictionary<TowerStatType, string>();
            for (int i = 0; i < TowerStatTypes.Length; i++)
            {
                TowerStatType type = TowerStatTypes[i];
                TowerStatTypeStrings.Add(type, type.ToString()); 
            }

            CharacterStatTypeStrings = new Dictionary<CharacterStatType, string>();
            for (int i = 0; i < CharacterStatTypes.Length; i++)
            {
                CharacterStatType type = CharacterStatTypes[i];
                CharacterStatTypeStrings.Add(type, type.ToString()); 
            }

        }
    }
}
