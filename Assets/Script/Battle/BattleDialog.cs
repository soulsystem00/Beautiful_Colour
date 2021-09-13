using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialog : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color highlightedColor;
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
    
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        yield return new WaitForSeconds(1f);
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
        enemyInfo_details.HpBar.transform.localScale = new Vector3(unit.HP / unit.MaxHp, 1f, 1f);

        //ppText.text = $"PP {move.PP}/{move.Base.PP}";
        //typeText.text = move.Base.Type.ToString();
    }
    public void UpdateSkillSelection(int selectedMove, Skill skill)
    {
        for (int i = 0; i < skillTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                skillTexts[i].color = highlightedColor;
            }
            else
            {
                skillTexts[i].color = Color.black;
            }
        }
        skillDetailText[0].text = skill.Base.Power.ToString();
        skillDetailText[1].text = $"PP {skill.PP}/{skill.Base.PP}";
        //ppText.text = $"PP {move.PP}/{move.Base.PP}";
        //typeText.text = move.Base.Type.ToString();
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
    public void SetSkillNames(List<Skill> skills)
    {
        for (int i = 0; i < skillTexts.Count; i++)
        {
            if (i < skills.Count)
            {
                skillTexts[i].text = skills[i].Base.Name;
            }
            else
            {
                skillTexts[i].text = "-";
            }
        }
    }

}
