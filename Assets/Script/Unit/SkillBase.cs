using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Create new skill")]
public class SkillBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] UnitType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int energy;
    [SerializeField] SkillCategory skillCategoty;
    [SerializeField] SkillEffects effects;
    [SerializeField] SkillTarget skillTarget;
    public string Name { get => name; }
    public string Description { get => description; }
    public UnitType Type { get => type; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public int Energy { get => energy; }
    public bool IsSpecial
    {
        get
        {
            return true;
        }
    }

    public SkillCategory SkillCategoty { get => skillCategoty; }
    public SkillEffects Effects { get => effects; }
    public SkillTarget SkillTarget { get => skillTarget; }
}
[System.Serializable]
public class SkillEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts { get => boosts; }
}
[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
    public int turn;
}
public enum SkillCategory
{
    일반공격, 특수공격, 버프
}
public enum SkillTarget
{
    상대, 자신
}