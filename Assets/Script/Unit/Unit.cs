using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;

    public UnitBase Base { get => _base; }
    public int Level { get => level; }

    public List<Skill> Skills { get; set; }

    public int HP { get; set; }

    public void init()
    {
        HP = MaxHp;

        Skills = new List<Skill>();
        foreach(var skill in Base.LearnableSkills)
        {
            if(skill.Level <= this.Level)
            {
                Skills.Add(new Skill(skill.Base));
            }

            if (Skills.Count >= 4)
                break;
        }
    }

    public int PhysicsAttack
    {
        get { return Base.PhysicsAttack + (Level * Base.PhysicsAttack_Inc); }
    }

    public int MagicalAttack
    {
        get { return Base.MagicalAttack + (Level * Base.MagicalAttack_Inc); }
    }    
    public int PhysicsDefense
    {
        get { return Base.PhysicsDefence + (Level * Base.PhysicsDefence_Inc); }
    }
    public int MagicalDefense
    {
        get { return Base.MagicalDefence + (Level * Base.MagicalDefence_Inc); }
    }
    public int Speed
    {
        get { return Base.Speed + (Level * Base.Speed_Inc); }
    }
    public int Evasion
    {
        get { return Base.Evasion + (Level * Base.Evasion_Inc); }
    }
    public int MaxHp
    {
        get { return Base.MaxHp + (Level * Base.MaxHp_Inc); }
    }
    public int Energy
    {
        get { return Base.Energy + (Level * Base.Energy_Inc); }
    }
    public DamageDetails TakeDamage(Skill skill, Unit attacker)
    {
        var damagedatails = new DamageDetails()
        {
            Fainted = false,
            Critical = 1f,
            TypeEffectiveness = 1f
        };

        return damagedatails;
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}