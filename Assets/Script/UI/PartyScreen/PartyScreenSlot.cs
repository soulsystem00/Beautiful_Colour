using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenSlot : MonoBehaviour
{
    [SerializeField] Text name;
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;
    [SerializeField] Image expBar;

    Unit _unit;
    public void Init(Unit unit)
    {
        _unit = unit;
        SetData();

        _unit.OnHPChanged += SetData;
    }

    public void SetData()
    {
        this.name.text = _unit.Base.Name;

        var hpscale = Mathf.Clamp01((float)_unit.HP / _unit.MaxHp);
        var spscale = Mathf.Clamp01((float)_unit.energy / _unit.MaxEnergy);
        var expscale = Mathf.Clamp01((float)(_unit.Exp - _unit.Base.GetExpForLevel(_unit.Level)) / (_unit.Base.GetExpForLevel(_unit.Level + 1) - _unit.Base.GetExpForLevel(_unit.Level)));

        this.hpBar.transform.localScale = new Vector3(hpscale, 1f, 1f);
        this.mpBar.transform.localScale = new Vector3(spscale, 1f, 1f);
        this.expBar.transform.localScale = new Vector3(expscale, 1f, 1f);
    }
    public void SetSelection(bool selection)
    {
        name.color = (selection) ? GlobalSettings.i.HighlightedColor : Color.black;
    }
}
