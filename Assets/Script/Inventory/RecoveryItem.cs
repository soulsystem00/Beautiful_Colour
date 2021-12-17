using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("Energy")]
    [SerializeField] int energyAmount;
    [SerializeField] bool restoreMaxEnergy;

    [Header("Stat")]
    [SerializeField] Stat stat;
    [SerializeField] bool recoverAllstat;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Unit unit)
    {
        // revive item mechanism
        if(revive || maxRevive)
        {
            if (unit.HP > 0)
                return false;

            if(revive)
            {
                unit.IncreaseHP(unit.MaxHp / 2);
            }
            else if(maxRevive)
            {
                unit.IncreaseHP(unit.MaxHp);
            }
            return true;
        }

        if(recoverAllstat || stat != Stat.none)
        {
            if(recoverAllstat)
            {
                return unit.CureDeBuffAll();
            }
            else if(unit.BuffDic[stat].Count > 0)
            {
                return unit.CureDeBuff(stat);
            }
            else
            {
                return false;
            }
        }
        // not use when unit was dead
        if(unit.HP <= 0)
        {
            return false;
        }

        // restore hp
        if(restoreMaxHP || hpAmount > 0)
        {
            if (unit.HP == unit.MaxHp)
                return false;
            if (restoreMaxHP)
                unit.IncreaseHP(unit.MaxHp);
            else
                unit.IncreaseHP(hpAmount);
        }

        // restore mp
        if(restoreMaxEnergy || energyAmount > 0)
        {
            if (unit.energy == unit.MaxEnergy)
                return false;

            if (restoreMaxEnergy)
                unit.IncreaseMP(unit.MaxEnergy);
            else
                unit.IncreaseMP(energyAmount);
        }

        return true;
    }
}
