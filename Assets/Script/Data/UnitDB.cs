using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDB
{
    static Dictionary<string, UnitBase> units;

    public static void Init()
    {
        units = new Dictionary<string, UnitBase>();

        var unitArray = Resources.LoadAll<UnitBase>("");

        foreach(var unit in unitArray)
        {
            if(units.ContainsKey(unit.Name))
            {
                Debug.LogError($"같은 이름 {unit.Name}을 가진 유닛이 있습니다.");
                continue;
            }
            units[unit.Name] = unit;
        }
    }
    public static UnitBase GetUnitByName(string name)
    {
        if(!units.ContainsKey(name))
        {
            Debug.LogError($"{name} 이름을 가진 유닛이 데이터베이스에 존재하지 않습니다.");
            return null;
        }
        return units[name];
    }
}
