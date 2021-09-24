using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new Unit")]
public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite portraitSprite;
    [SerializeField] Sprite dialogSprite;

    [SerializeField] UnitType type1;

    //Base Stats
    [Header("기본 능력치")]
    [SerializeField] int physicsAttack;
    [SerializeField] int magicalAttack;
    [SerializeField] int physicsDefence;
    [SerializeField] int magicalDefence;
    [SerializeField] int speed;
    [SerializeField] int evasion;
    [SerializeField] int maxHp;
    [SerializeField] int energy;
    [SerializeField] int maxEnergy;
    [SerializeField] bool isEnemy;

    [Header("레벨에 따른 증가량")]
    [SerializeField] int physicsAttack_Inc;
    [SerializeField] int magicalAttack_Inc;
    [SerializeField] int physicsDefence_Inc;
    [SerializeField] int magicalDefence_Inc;
    [SerializeField] int speed_Inc;
    [SerializeField] int evasion_Inc;
    [SerializeField] int maxHp_Inc;
    [SerializeField] int maxEnergy_Inc;

    [SerializeField] List<LearnableSkill> learnableSkills;
    public string Name
    {
        get { return name; }
    }

    public string Description { get => description; }
    public Sprite FrontSprite { get => frontSprite; }
    public Sprite BackSprite { get => backSprite; }
    public Sprite PortraitSprite { get => portraitSprite; }
    public Sprite DialogSprite { get => dialogSprite; }
    public UnitType Type1 { get => type1; }
    public int MaxHp { get => maxHp; }
    public int MaxEnergy { get => maxEnergy; }
    public int PhysicsAttack { get => physicsAttack; }
    public int MagicalAttack { get => magicalAttack; }
    public int PhysicsDefence { get => physicsDefence; }
    public int MagicalDefence { get => magicalDefence; }
    public int Speed { get => speed; }
    public int Evasion { get => evasion; }

    public int MaxHp_Inc { get => maxHp_Inc; }
    public int MaxEnergy_Inc { get => maxEnergy_Inc; }
    public int PhysicsAttack_Inc { get => physicsAttack_Inc; }
    public int MagicalAttack_Inc { get => magicalAttack_Inc; }
    public int PhysicsDefence_Inc { get => physicsDefence_Inc; }
    public int MagicalDefence_Inc { get => magicalDefence_Inc; }
    public int Speed_Inc { get => speed_Inc; }
    public int Evasion_Inc { get => evasion_Inc; }

    public List<LearnableSkill> LearnableSkills { get => learnableSkills; }
    public bool IsEnemy { get => isEnemy; }
}
public enum UnitType
{
    동물,
    인간,
    악귀,
    마왕
}
public enum Stat
{
    PhysicsAttack,
    MagicalAttack,
    PhysicsDefence,
    MagicalDefence,
    Speed,
    Evasion,

}

[System.Serializable]
public class LearnableSkill
{
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase Base { get => skillBase; }
    public int Level { get => level; }
}