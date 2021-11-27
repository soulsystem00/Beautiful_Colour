using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour, IMenuInitializer
{
    [SerializeField] Text level;

    [SerializeField] Text hp;
    [SerializeField] Text sp;
    [SerializeField] Text exp;

    [SerializeField] GameObject hpbar;
    [SerializeField] GameObject spbar;
    [SerializeField] GameObject expbar;

    [SerializeField] List<Text> stats;
    void IMenuInitializer.InitMenu(Unit unit)
    {
        level.text = unit.Level.ToString();
        hp.text = unit.HP + " / " + unit.MaxHp;
        sp.text = unit.energy + " / " + unit.MaxEnergy;
        exp.text = unit.Exp + " / " + unit.Base.GetExpForLevel(unit.Level + 1);

        var hpscale = Mathf.Clamp01((float)unit.HP / unit.MaxHp);
        var spscale = Mathf.Clamp01((float)unit.energy / unit.MaxEnergy);
        var expscale = Mathf.Clamp01((float)(unit.Exp - unit.Base.GetExpForLevel(unit.Level)) / (unit.Base.GetExpForLevel(unit.Level + 1) - unit.Base.GetExpForLevel(unit.Level)));

        hpbar.transform.localScale = new Vector3(hpscale, 1f, 1f);
        spbar.transform.localScale = new Vector3(spscale, 1f, 1f);
        expbar.transform.localScale = new Vector3(expscale, 1f, 1f);

        stats[0].text = $"물리공격\t{unit.PhysicsAttack}";
        stats[1].text = $"마법공격\t{unit.MagicalAttack}";
        stats[2].text = $"물리방어\t{unit.PhysicsDefense}";
        stats[3].text = $"마법방어\t{unit.MagicalDefense}";
        stats[4].text = $"속도\t{unit.Speed}";
    }
}
