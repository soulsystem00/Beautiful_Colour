using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDB
{
    static Dictionary<string, SkillBase> skills;

    public static void Init()
    {
        skills = new Dictionary<string, SkillBase>();

        var skillArray = Resources.LoadAll<SkillBase>("");

        foreach (var skill in skillArray)
        {
            if (skills.ContainsKey(skill.Name))
            {
                Debug.LogError($"같은 이름 {skill.Name}을 가진 유닛이 있습니다.");
                continue;
            }
            skills[skill.Name] = skill;
        }
    }
    public static SkillBase GetSkillByName(string name)
    {
        if (!skills.ContainsKey(name))
        {
            Debug.LogError($"{name} 이름을 가진 스킬이 데이터베이스에 존재하지 않습니다.");
            return null;
        }
        return skills[name];
    }
}
