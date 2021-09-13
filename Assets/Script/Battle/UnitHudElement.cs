using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHudElement : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] HPBar hPBar;
    [SerializeField] HPBar spBar;
    [SerializeField] Image sprite;
    [SerializeField] bool isEnemy;
    Unit _unit;

    public void SetSprite()
    {
        if(isEnemy)
        {
            sprite.sprite = _unit.Base.FrontSprite;
        }
        else
        {
            sprite.sprite = _unit.Base.PortraitSprite;
        }
    }

    public void SetData(Unit unit)
    {
        _unit = unit;

        if(isEnemy)
        {
            hPBar.SetHP((float)_unit.HP / _unit.MaxHp);
        }
        else
        {
            nameText.text = _unit.Base.Name;
            hPBar.SetHP((float)_unit.HP / _unit.MaxHp);
            spBar.SetHP((float)_unit.energy / _unit.MaxEnergy);
        }
    }

    public IEnumerator UpdateHP()
    {
        if(isEnemy)
            yield return hPBar.SetHPSmooth((float)_unit.HP / _unit.MaxHp);
        else
        {
            yield return hPBar.SetHPSmooth((float)_unit.HP / _unit.MaxHp);
            yield return hPBar.SetHPSmooth((float)_unit.energy / _unit.MaxEnergy);
        }
            
    }
}
