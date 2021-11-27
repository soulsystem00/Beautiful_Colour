using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] List<Text> skillTexts;
    Color highlightColor;
    int currentSelection = 0;
    private void Start()
    {
        highlightColor = GlobalSettings.i.HighlightedColor;
    }
    public void SetSkillData(List<SkillBase> currentSkills, SkillBase newSkill)
    {
        for(int i = 0; i < currentSkills.Count; i++)
        {
            skillTexts[i].text = currentSkills[i].Name;
        }
        skillTexts[currentSkills.Count].text = newSkill.Name;
    }
    public void HandleSkillSelection(Action<int> onSelected)
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelection++;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelection--;
        }
        currentSelection = Mathf.Clamp(currentSelection, 0, UnitBase.MaxNumOfSKills);

        UpdateSkillSelection(currentSelection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentSelection);
        }
    }
    void UpdateSkillSelection(int selection)
    {
        for (int i = 0; i < UnitBase.MaxNumOfSKills + 1; i++)
        {
            if(selection == i)
            {
                skillTexts[i].color = highlightColor;
            }
            else
            {
                skillTexts[i].color = Color.black;
            }
        }
    }
}
