using System;
using System.Collections.Generic;

namespace EnumData
{
  public enum CharacterType { REWARD = 0, COST = 1, PROB = 2, }
  public enum DifficultyType { HP = 0, SPEED = 1, AMOUNT = 2, TIME = 3, COST = 4, PROB = 5, }
  public enum Element { FIRE = 0, WATER = 1, NATURE = 2, }
  public enum EnemyGrade { NORMAL = 0, ELITE = 1, BOSS = 2, }
  public enum Grade { NORMAL = 0, RARE = 1, HEROIC = 2, LEGENDARY = 3, }
  public enum AttackPriority { FIRST = 0, LAST = 1, STRONG = 2, WEAK = 3, ELEMENT = 4, DEBUFF = 5, }
  public enum EnemyStatType { HP = 0, SPEED = 1, EXP = 2, }
  public enum TowerStatType { DAMAGE = 0, ATTACKSPEED = 1, DISTANCE = 2, MULTISHOT = 3, SPLASH = 4, }
  public enum BuffType { GOLDMINE = 0, DMG = 1, ATKSPD = 2, }
  public enum DebuffType { SLOW = 0, BURN = 1, POISON = 2, BLEED = 3, }
  public enum AnimationType { IDLE = 0, ATTACK = 1, MOVE = 2, DEAD = 3, }
  public enum LanguageType { KOREAN = 0, ENGLISH = 1, }
  public enum AttackType { PROMPT = 0, PROJECTILE = 1, POINT = 2, }

    public static class EnumArray
    {
        public static CharacterType[] CharacterTypes { get; private set; } = (CharacterType[])Enum.GetValues(typeof(CharacterType));
public static DifficultyType[] DifficultyTypes { get; private set; } = (DifficultyType[])Enum.GetValues(typeof(DifficultyType));
public static Element[] Elements { get; private set; } = (Element[])Enum.GetValues(typeof(Element));
public static EnemyGrade[] EnemyGrades { get; private set; } = (EnemyGrade[])Enum.GetValues(typeof(EnemyGrade));
public static Grade[] Grades { get; private set; } = (Grade[])Enum.GetValues(typeof(Grade));
public static AttackPriority[] AttackPrioritys { get; private set; } = (AttackPriority[])Enum.GetValues(typeof(AttackPriority));
public static EnemyStatType[] EnemyStatTypes { get; private set; } = (EnemyStatType[])Enum.GetValues(typeof(EnemyStatType));
public static TowerStatType[] TowerStatTypes { get; private set; } = (TowerStatType[])Enum.GetValues(typeof(TowerStatType));
public static BuffType[] BuffTypes { get; private set; } = (BuffType[])Enum.GetValues(typeof(BuffType));
public static DebuffType[] DebuffTypes { get; private set; } = (DebuffType[])Enum.GetValues(typeof(DebuffType));
public static AnimationType[] AnimationTypes { get; private set; } = (AnimationType[])Enum.GetValues(typeof(AnimationType));
public static LanguageType[] LanguageTypes { get; private set; } = (LanguageType[])Enum.GetValues(typeof(LanguageType));
public static AttackType[] AttackTypes { get; private set; } = (AttackType[])Enum.GetValues(typeof(AttackType));
public static Dictionary<CharacterType,string> CharacterTypeStrings { get; private set; }
public static Dictionary<DifficultyType,string> DifficultyTypeStrings { get; private set; }
public static Dictionary<Element,string> ElementStrings { get; private set; }
public static Dictionary<EnemyGrade,string> EnemyGradeStrings { get; private set; }
public static Dictionary<Grade,string> GradeStrings { get; private set; }
public static Dictionary<AttackPriority,string> AttackPriorityStrings { get; private set; }
public static Dictionary<EnemyStatType,string> EnemyStatTypeStrings { get; private set; }
public static Dictionary<TowerStatType,string> TowerStatTypeStrings { get; private set; }
public static Dictionary<BuffType,string> BuffTypeStrings { get; private set; }
public static Dictionary<DebuffType,string> DebuffTypeStrings { get; private set; }
public static Dictionary<AnimationType,string> AnimationTypeStrings { get; private set; }
public static Dictionary<LanguageType,string> LanguageTypeStrings { get; private set; }
public static Dictionary<AttackType,string> AttackTypeStrings { get; private set; }


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

            EnemyGradeStrings = new Dictionary<EnemyGrade, string>();
            for (int i = 0; i < EnemyGrades.Length; i++)
            {
                EnemyGrade type = EnemyGrades[i];
                EnemyGradeStrings.Add(type, type.ToString()); 
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

            EnemyStatTypeStrings = new Dictionary<EnemyStatType, string>();
            for (int i = 0; i < EnemyStatTypes.Length; i++)
            {
                EnemyStatType type = EnemyStatTypes[i];
                EnemyStatTypeStrings.Add(type, type.ToString()); 
            }

            TowerStatTypeStrings = new Dictionary<TowerStatType, string>();
            for (int i = 0; i < TowerStatTypes.Length; i++)
            {
                TowerStatType type = TowerStatTypes[i];
                TowerStatTypeStrings.Add(type, type.ToString()); 
            }

            BuffTypeStrings = new Dictionary<BuffType, string>();
            for (int i = 0; i < BuffTypes.Length; i++)
            {
                BuffType type = BuffTypes[i];
                BuffTypeStrings.Add(type, type.ToString()); 
            }

            DebuffTypeStrings = new Dictionary<DebuffType, string>();
            for (int i = 0; i < DebuffTypes.Length; i++)
            {
                DebuffType type = DebuffTypes[i];
                DebuffTypeStrings.Add(type, type.ToString()); 
            }

            AnimationTypeStrings = new Dictionary<AnimationType, string>();
            for (int i = 0; i < AnimationTypes.Length; i++)
            {
                AnimationType type = AnimationTypes[i];
                AnimationTypeStrings.Add(type, type.ToString()); 
            }

            LanguageTypeStrings = new Dictionary<LanguageType, string>();
            for (int i = 0; i < LanguageTypes.Length; i++)
            {
                LanguageType type = LanguageTypes[i];
                LanguageTypeStrings.Add(type, type.ToString()); 
            }

            AttackTypeStrings = new Dictionary<AttackType, string>();
            for (int i = 0; i < AttackTypes.Length; i++)
            {
                AttackType type = AttackTypes[i];
                AttackTypeStrings.Add(type, type.ToString()); 
            }

        }
    }
}
