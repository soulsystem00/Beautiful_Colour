using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    Color highlightedColor;
    [SerializeField] Text dialogText;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject enemySelector;
    [SerializeField] GameObject enemyInfo;
    [SerializeField] GameObject skillDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> enemyTexts;
    [SerializeField] List<Text> skillTexts;
    [SerializeField] List<Text> skillDetailText;

    [SerializeField] EnemyInfo enemyInfo_details;
    [SerializeField] SkillInfo skillInfo_Details;
    public event Action ChangeState;
    private int prevSkill;

    public event Action StartTyping;
    public event Action EndTyping;
    private string curString;
    private bool IsTyping = false;
    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z) && IsTyping)
        {

        }
    }
    public IEnumerator TypeDialog(string dialog)
    {
        StartTyping?.Invoke();
        IsTyping = true;
        curString = dialog;
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            if(IsTyping)
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(1f / letterPerSecond);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                yield break;
            }
        }
        yield return new WaitForSeconds(1f);
        IsTyping = false;
        EndTyping?.Invoke();
    }
    public void StopTyping()
    {
        StopCoroutine(TypeDialog(null));
        IsTyping = false;
        dialogText.text = curString;
        EndTyping?.Invoke();
    }
    public IEnumerator TypeDialog2(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        yield return new WaitForSeconds(1f);
        ChangeState.Invoke();
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }
    public void UpdateEnemySelection(int selectedMove, Unit unit)
    {
        for (int i = 0; i < enemyTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                enemyTexts[i].color = highlightedColor;
            }
            else
            {
                enemyTexts[i].color = Color.black;
            }
        }

        enemyInfo_details.Face.sprite = unit.Base.FrontSprite;
        enemyInfo_details.Description.text = unit.Base.Description;
        enemyInfo_details.Name.text = unit.Base.name;
        enemyInfo_details.HP.text = $"{unit.HP} / {unit.MaxHp}";
        enemyInfo_details.HpBar.transform.localScale = new Vector3((float)unit.HP / unit.MaxHp, 1f, 1f);
    }
    public void UpdateSkillSelection2(int selectedSkill, Skill skill)
    {
        for (int i = 0; i < skillInfo_Details.Texts.Count; i++)
        {
            if (i == selectedSkill)
            {
                skillInfo_Details.Texts[i].color = highlightedColor;
                //skillInfo_Details.Scroll.value = 1 - (float)i / 3;
            }
            else
            {
                skillInfo_Details.Texts[i].color = Color.black;
            }
        }
        skillInfo_Details.DescriptionText.text = skill.Base.Description;
        if(prevSkill != selectedSkill)
            skillInfo_Details.UpdateScrollSmooth(1 - (float)selectedSkill / 3);
        prevSkill = selectedSkill;
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }
    public void EnableEnemySelector(bool enabled)
    {
        enemySelector.SetActive(enabled);
        enemyInfo.SetActive(enabled);
    }
    public void EnableEnemyInfo(bool enabled)
    {
        enemyInfo.SetActive(enabled);
    }
    public void EnableSkillInfo(bool enabled)
    {
        skillInfo_Details.gameObject.SetActive(enabled);
    }
    public void SetEnemyNames(List<Unit> units)
    {
        for (int i = 0; i < enemyTexts.Count; i++)
        {
            if(i < units.Count)
            {
                enemyTexts[i].text = units[i].Base.Name;
            }
            else
            {
                enemyTexts[i].text = "-";
            }
        }
    }
    public void SetSkillNames2(List<Skill> skills, Unit unit)
    {
        for (int i = 0; i < skillInfo_Details.Texts.Count; i++)
        {
            if (i < skills.Count)
            {
                skillInfo_Details.Texts[i].text = ($"{skills[i].Base.Name}  {skills[i].PP}");
            }
            else
            {
                skillInfo_Details.Texts[i].text = "-";
            }
        }
        skillInfo_Details.SpText.text = $"남은 기 : {unit.energy}";
        skillInfo_Details.SpBar.transform.localScale = new Vector3(((float)unit.energy / unit.MaxEnergy), 1f, 1f);
    }
}
