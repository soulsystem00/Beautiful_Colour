using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuSelector : MonoBehaviour
{
    [SerializeField] List<Image> subMenuTags;
    [SerializeField] List<GameObject> subMenus;
    int selection = 0;

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selection--;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        { 
            selection++;
        }

        selection = Mathf.Clamp(selection, 0, subMenuTags.Count - 1);

        UpdateSelection();

        if(Input.GetKeyDown(KeyCode.Z) && selection == 2)
        {
            ResetSelection(true);
            onSelected?.Invoke();
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            ResetSelection();
            onBack?.Invoke();
        }
    }
    public void UpdateSelection(bool check = false)
    {
        for (int i = 0; i < subMenuTags.Count; i++)
        {
            if(!check)
            {
                if (i == selection)
                {
                    subMenuTags[i].color = Color.red;
                    subMenus[i].SetActive(true);
                }
                else
                {
                    subMenuTags[i].color = Color.white;
                    subMenus[i].SetActive(false);
                }
            }
            else
            {
                selection = 0;
                if (i == selection)
                {
                    subMenuTags[i].color = Color.blue;
                    subMenus[i].SetActive(true);
                }
                else
                {
                    subMenuTags[i].color = Color.white;
                    subMenus[i].SetActive(false);
                }
            }
        }
    }
    public void SetSubMenu(Unit unit)
    {
        for (int i = 0; i < subMenus.Count; i++)
        {
            subMenus[i].GetComponent<IMenuInitializer>().InitMenu(unit);
        }
    }
    public void ResetSelection(bool check = false)
    {
        for (int i = 0; i < subMenuTags.Count; i++)
        {
            if(!check)
            {
                selection = 0;
                if (i == 0)
                    subMenus[i].SetActive(true);
                else
                    subMenus[i].SetActive(false);
            }
            
            subMenuTags[i].color = Color.white;
        }
        
    }
}
