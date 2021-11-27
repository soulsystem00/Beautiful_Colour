using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Skill
{
    public SkillBase Base { get; set; }
    public int PP { get; set; }

    public Skill(SkillBase skillBase)
    {
        Base = skillBase;
        PP = skillBase.Energy;
    }
    public Skill(SkillSaveData saveData)
    {
        Base = SkillDB.GetSkillByName(saveData.name);
        PP = saveData.pp;
    }
    public SkillSaveData GetSaveData()
    {
        var saveData = new SkillSaveData()
        {
            name = Base.Name,
            pp = PP
        };
        return saveData;
    }
}
[Serializable]
public class SkillSaveData
{
    public string name;
    public int pp;
}
