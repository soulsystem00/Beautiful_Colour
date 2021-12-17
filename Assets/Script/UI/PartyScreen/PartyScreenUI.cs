using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyScreenUI : MonoBehaviour
{
    [SerializeField] List<PartyScreenSlot> partyScreenSlots;

    List<Unit> units;
    UnitParty party;

    int selection = 0;

    public Unit SelectedMember => units[selection];
    public void Init()
    {
        party = UnitParty.GetUnitParty();
        SetPartyData();

        party.OnUpdated += SetPartyData;
    }
    public void SetPartyData()
    {
        units = party.Units;
        for (int i = 0; i < partyScreenSlots.Count; i++)
        {
            if(i<units.Count)
            {
                partyScreenSlots[i].gameObject.SetActive(true);
                partyScreenSlots[i].Init(units[i]);
            }
            else
            {
                partyScreenSlots[i].gameObject.SetActive(false);
            }
        }
        UpdateMemberSelection(selection);
    }
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        int prevSelection = selection;

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection++;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection--;
        }
        selection = Mathf.Clamp(selection, 0, units.Count - 1);
        if(selection != prevSelection)
        {
            UpdateMemberSelection(selection);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }
    private void UpdateMemberSelection(int selection)
    {
        for (int i = 0; i < units.Count; i++)
        {
            partyScreenSlots[i].SetSelection(i == selection);
        }
    }
}
