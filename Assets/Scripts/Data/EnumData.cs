namespace EnumData
{
    public enum CharacterType { POWER = 0, ATTACKSPEED = 1, COST = 2, PROB = 3, }
    public enum DifficultyType { HP = 0, SPEED = 1, AMOUNT = 2, COST = 3, PROB = 4, TIME = 5, }

    public enum Element { FIRE = 0, WATER = 1, NATURE = 2, }
    public enum Grade { NORMAL = 0, RARE = 1, HEROIC = 2, LEGENDARY = 3, }
    public enum AttackPriority { FIRST = 0, LAST = 1, STRONG = 2, WEAK = 3, }
    public enum TowerMainStatType { DAMAGE = 0, ATTACKSPEED }
    public enum CharacterStatType { ABILITY = 0, REWARD, FIRE, WATER, NATURE }
}