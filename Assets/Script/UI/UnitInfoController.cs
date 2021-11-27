using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum MenuState { Unitinfo, SubMenuSelect, SkillDetail, }

public class UnitInfoController : MonoBehaviour
{
    [SerializeField] Image unitImage;
    [SerializeField] List<GameObject> nameTags;
    [SerializeField] Text unitName;
    [SerializeField] Text unitDescription;
    [SerializeField] SubMenuSelector subMenuSelector;
    [SerializeField] SkillUI skillUI;

    public event Action onBack;

    MenuState state;

    int selection = 0;

    List<Unit> units;
    public void OpenScreen(UnitParty unitParty)
    {
        state = MenuState.Unitinfo;
        gameObject.SetActive(true);
        units = unitParty.Units;
        Init();
        UpdateSelection();
        subMenuSelector.ResetSelection();
        //subMenuSelector.UpdateSelection();
    }
    void CloseScreen()
    {
        gameObject.SetActive(false);
    }
    private void Init()
    {
        // 유닛 파티에 있는 유닛 가져와서 초기화하기
        for (int i = 0; i < nameTags.Count; i++)
        {
            if(i < units.Count)
            {
                nameTags[i].GetComponentInChildren<Text>().text = units[i].Base.Name;
            }
            else
            {
                nameTags[i].SetActive(false);
            }
        }
    }

    public void HandleUpdate()
    {
        if(state == MenuState.Unitinfo)
        {
            UnitSelection();
        }
        else if(state == MenuState.SubMenuSelect)
        {
            Action onSelected = () =>
            {
                state = MenuState.SkillDetail;
            };

            Action onBack = () =>
            {
                state = MenuState.Unitinfo;
                UpdateSelection();
            };
            subMenuSelector.HandleUpdate(onSelected, onBack);
        }
        else if(state == MenuState.SkillDetail)
        {
            Action onBack = () =>
            {
                state = MenuState.SubMenuSelect;
            };
            skillUI.HandleUpdate(onBack);
        }
    }

    void UnitSelection()
    {
        int prevSelection = selection;
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selection--;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            selection++;
        }

        selection = Mathf.Clamp(selection, 0, units.Count - 1);

        if(prevSelection != selection)
        {
            UpdateSelection();
            //subMenuSelector.UpdateSelection(true);
            subMenuSelector.ResetSelection();
        }
            

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // GOTO Deep Menu
            SetTagColor(true);
            state = MenuState.SubMenuSelect;
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseScreen();
        }
    }
    void UpdateSelection()
    {
        unitImage.sprite = units[selection].Base.FrontSprite;
        unitName.text = units[selection].Base.Name;
        unitDescription.text = units[selection].Base.Description;

        SetTagColor();
        subMenuSelector.SetSubMenu(units[selection]);
    }
    public void SetTagColor(bool check = false)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if(!check)
                nameTags[i].GetComponent<Image>().color = (i == selection) ? Color.red : Color.white;
            else
                nameTags[i].GetComponent<Image>().color = Color.white;
        }
    }
}
