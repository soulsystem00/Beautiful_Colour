using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;

    public Unit(UnitBase ubase, int uLevel)
    {
        _base = ubase;
        level = uLevel;

        init();
    }
    public Unit(UnitSaveData saveData)
    {
        _base = UnitDB.GetUnitByName(saveData.name);
        HP = saveData.hp;
        energy = saveData.energy;
        level = saveData.level;
        Exp = saveData.exp;

        Skills = saveData.skills.Select(s => new Skill(s)).ToList();

        CalculateStats();
        ResetStatBoost();
        StateChanges = new Queue<string>();
    }
    public int Exp { get; set; }
    public UnitBase Base { get => _base; }
    public int Level { get => level; }
    public int HP { get; set; }
    public int energy { get; set; }
    public List<Skill> Skills { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Dictionary<Stat, Queue<BuffInfo>> BuffDic { get; private set; }
    public Queue<string> StateChanges { get; private set; }
    public void init()
    {
        Skills = new List<Skill>();
        
        foreach (var skill in Base.LearnableSkills)
        {
            if (skill.Level <= this.Level)
            {
                Skills.Add(new Skill(skill.Base));
            }
            if(Skills.Count >= UnitBase.MaxNumOfSKills)
            {
                break;
            }
        }
        Exp = Base.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHp;
        StateChanges = new Queue<string>();
        energy = MaxEnergy;
        ResetStatBoost();
    }
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.PhysicsAttack, Mathf.FloorToInt(Base.PhysicsAttack + Level * Base.PhysicsAttack_Inc));
        Stats.Add(Stat.PhysicsDefence, Mathf.FloorToInt(Base.PhysicsDefence + Level * Base.PhysicsDefence_Inc));
        Stats.Add(Stat.MagicalAttack, Mathf.FloorToInt(Base.MagicalAttack + Level * Base.MagicalAttack_Inc));
        Stats.Add(Stat.MagicalDefence, Mathf.FloorToInt(Base.MagicalDefence + Level * Base.MagicalDefence_Inc));
        Stats.Add(Stat.Speed, Mathf.FloorToInt(Base.Speed + Level * Base.Speed_Inc));
        Stats.Add(Stat.Evasion, Mathf.FloorToInt(Base.Evasion + Level * Base.Evasion_Inc));

        MaxHp = Base.MaxHp + Level * Base.MaxHp_Inc;
        MaxEnergy = Base.MaxEnergy + Level * Base.MaxEnergy_Inc;
    }
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.PhysicsAttack, 0 },
            { Stat.PhysicsDefence, 0 },
            { Stat.MagicalAttack, 0 },
            { Stat.MagicalDefence, 0 },
            { Stat.Speed, 0 },
            { Stat.Evasion, 0 },
        };
        BuffDic = new Dictionary<Stat, Queue<BuffInfo>>()
        {             
            { Stat.PhysicsAttack, new Queue<BuffInfo>() },
            { Stat.PhysicsDefence, new Queue<BuffInfo>() },
            { Stat.MagicalAttack, new Queue<BuffInfo>() },
            { Stat.MagicalDefence, new Queue<BuffInfo>() },
            { Stat.Speed, new Queue<BuffInfo>() },
            { Stat.Evasion, new Queue<BuffInfo>() },

        };
    }
    int GetStat(Stat stat)
    {
        SetStatBoosts(stat);
        int statVal = Stats[stat];
        int boost = StatBoosts[stat];

        statVal = Mathf.FloorToInt(statVal + statVal * (boost / 100));

        return statVal;
    }
    void SetStatBoosts(Stat stat)
    {
        int maxVal = 0;
        int minVal = 0;
        Queue<BuffInfo> tmp = new Queue<BuffInfo>();
        while (BuffDic[stat].Count != 0)
        {
            var a = BuffDic[stat].Dequeue();
            a.Turn--;
            if (a.Val > 0)
                maxVal = Mathf.Max(maxVal, a.Val);
            else
                minVal = Mathf.Max(minVal, a.Val);

            if (a.Turn > 0)
                tmp.Enqueue(a);
        }

        while (tmp.Count != 0)
        {
            BuffDic[stat].Enqueue(tmp.Dequeue());
        }

        StatBoosts[stat] = maxVal + minVal;

    }
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;
            var turn = statBoost.turn;
            BuffDic[stat].Enqueue(new BuffInfo(boost, turn));
            if (boost > 0)
                StateChanges.Enqueue($"{Base.Name}의 {stat}이(가) {boost}% 만큼 증가!");
            else
                StateChanges.Enqueue($"{Base.Name}의 {stat}이(가) {-boost}% 만큼 감소!");
        }
    }
    public bool CheckForLevelUp()
    {
        if(Exp > Base.GetExpForLevel(Level + 1))
        {
            level++;
            return true;
        }

        return false;
    }
    public LearnableSkill GetLearnableSkillAtCurLevel()
    {
        return Base.LearnableSkills.Where(x => x.Level == level).FirstOrDefault();
    }
    public void LearnSkill(LearnableSkill newSkill)
    {
        if (Skills.Count > UnitBase.MaxNumOfSKills)
            return;

        Skills.Add(new Skill(newSkill.Base));
    }
    public int PhysicsAttack
    {
        get { return GetStat(Stat.PhysicsAttack); }
    }

    public int MagicalAttack
    {
        get { return GetStat(Stat.MagicalAttack); }
    }    
    public int PhysicsDefense
    {
        get { return GetStat(Stat.PhysicsDefence); }
    }
    public int MagicalDefense
    {
        get { return GetStat(Stat.MagicalDefence); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int Evasion
    {
        get { return GetStat(Stat.Evasion); }
    }
    public int MaxHp{ get; private set; }
    public int MaxEnergy{ get; private set; }
    //public int Energy
    //{
    //    get { return Base.Energy + (Level * Base.MaxEnergy_Inc); }
    //}
    public DamageDetails TakeDamage(Skill skill, Unit attacker)
    {
        var damagedatails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            TypeEffectiveness = 1f,
            Damage = 0
        };
        int damage = 0;
        if (skill.Base.SkillCategoty == SkillCategory.물리공격)
            damage = Mathf.CeilToInt(skill.Base.Power + attacker.PhysicsAttack - this.PhysicsDefense / 2);
        else if(skill.Base.SkillCategoty == SkillCategory.마법공격)
            damage = Mathf.CeilToInt(skill.Base.Power + attacker.MagicalAttack - this.MagicalDefense / 2);

        if (damage <= 5)
            damage = 5;

        HP -= damage;

        damagedatails.Damage = damage;

        if (HP <= 0)
        {
            HP = 0;
            damagedatails.Fainted = true;
        }

        return damagedatails;
    }
    public DamageDetails TakeDamage(int Attack, Unit attacker)
    {
        var damagedatails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            TypeEffectiveness = 1f,
            Damage = 0
        };

        int cal_Val = attacker.PhysicsAttack - this.PhysicsDefense;

        if (cal_Val <= 0)
            cal_Val = 1;

        int damage = 2 * cal_Val;

        HP -= damage;

        if(HP <= 0)
        {
            HP = 0;
            damagedatails.Fainted = true;
        }
        damagedatails.Damage = damage;

        return damagedatails;
    }
    public int GetRandomUnit(List<Unit> units)
    {
        int r = UnityEngine.Random.Range(0, units.Count);
        return r;
    }
    public UnitSaveData GetSaveData()
    {
        var saveData = new UnitSaveData()
        {
            name = Base.Name,
            hp = HP,
            level = Level,
            exp = Exp,
            energy = energy,
            skills = Skills.Select(x => x.GetSaveData()).ToList()
        };

        return saveData;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
    public int Damage { get; set; }
}

public class BuffInfo
{
    public int Val { get; set; }
    public int Turn { get; set; }
    public BuffInfo(int val, int turn)
    {
        this.Val = val;
        this.Turn = turn;
    }
}
[Serializable]
public class UnitSaveData
{
    public string name;
    public int hp;
    public int energy;
    public int level;
    public int exp;
    public List<SkillSaveData> skills;
}