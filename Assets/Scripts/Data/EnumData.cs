using System;
namespace EnumData
{
    public enum CharacterType { POWER = 0, ATTACKSPEED = 1, COST = 2, PROB = 3, }
    public enum DifficultyType { HP = 0, SPEED = 1, AMOUNT = 2, COST = 3, PROB = 4, TIME = 5, }
    public enum Element { FIRE = 0, WATER = 1, NATURE = 2, }
    public enum Grade { NORMAL = 0, RARE = 1, HEROIC = 2, LEGENDARY = 3, }
    public enum AttackPriority { FIRST = 0, LAST = 1, STRONG = 2, WEAK = 3, ELEMENT = 4, DEBUFF = 5 }
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

    }
}
