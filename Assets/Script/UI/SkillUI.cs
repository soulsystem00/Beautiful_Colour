using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour, IMenuInitializer
{
    [SerializeField] List<Text> texts;
    [SerializeField] Text description;

    Unit unit;

    int selection = 0;
    public void InitMenu(Unit unit)
    {
        // TODO : 스킬메뉴 초기화
        this.unit = unit;
        for (int i = 0; i < texts.Count; i++)
        {
            if(i < unit.Skills.Count)
            {
                texts[i].gameObject.SetActive(true);
                texts[i].text = unit.Skills[i].Base.Name;
            }
            else
            {
                texts[i].gameObject.SetActive(false);
            }
        }
        description.text = unit.Skills[selection].Base.Description;
    }
    public void HandleUpdate(Action onBack)
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection++;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection--;
        }
        selection = Mathf.Clamp(selection, 0, unit.Skills.Count - 1);

        UpdateSelection();

        if(Input.GetKeyDown(KeyCode.X))
        {
            ResetSelection();
            onBack?.Invoke();
        }
    }
    void UpdateSelection()
    {
        for (int i = 0; i < unit.Skills.Count; i++)
        {
            texts[i].color = (i == selection) ? GlobalSettings.i.HighlightedColor : Color.white;
        }
        description.text = unit.Skills[selection].Base.Description;
    }
    void ResetSelection()
    {
        for (int i = 0; i < unit.Skills.Count; i++)
        {
            texts[i].color = Color.white;
        }
        description.text = unit.Skills[selection].Base.Description;
        selection = 0;
    }
}
