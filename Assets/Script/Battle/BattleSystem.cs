using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, EnemySelect, EnemySkill, Busy, BattleOver
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleDialog battleDialog;

    [SerializeField] UnitHud playerHud;
    [SerializeField] UnitHud enemyHud;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentSkill;
    int currentUnit;
    int currentEnemy;

    UnitParty playerParty;
    List<Unit> enemyUnits;
    List<Unit> battleUnits = new List<Unit>();
    // Start is called before the first frame update
    public void StartBattle(UnitParty playerParty, List<Unit> enemyUnits)
    {
        currentUnit = 0;
        this.playerParty = playerParty;
        this.enemyUnits = enemyUnits;

        battleUnits.AddRange(enemyUnits);
        battleUnits.AddRange(playerParty.Units);

        battleUnits = battleUnits.OrderByDescending(unit => unit.Base.Speed).ThenBy(unit => unit.Base.IsEnemy).ToList();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        for (int i = 0; i < playerHud.unitHudElements.Count; i++)
        {
            if(i < playerParty.Units.Count)
            {
                playerHud.unitHudElements[i].SetData(playerParty.Units[i]);
            }
            else
            {
                playerHud.unitHudElements[i].gameObject.SetActive(false);
            }

        }
        for (int i = 0; i < enemyHud.unitHudElements.Count; i++)
        {
            if(i < enemyUnits.Count)
            {
                enemyHud.unitHudElements[i].SetData(enemyUnits[i]);
            }
            else
            {
                enemyHud.unitHudElements[i].gameObject.SetActive(false);
            }
        }
        
        battleDialog.SetEnemyNames(enemyUnits);

        yield return battleDialog.TypeDialog("야생의 적을 만났다.");

        CheckEnemy();
    }
    void CheckEnemy()
    {
        var pUnits = playerParty.Units.Where(x => x.HP > 0).FirstOrDefault();
        var eUnits = enemyUnits.Where(x => x.HP > 0).FirstOrDefault();

        while (battleUnits[currentUnit].HP <= 0)
            currentUnit++;

        if (pUnits == null)
        {
            // enemy win;
            BattleOver(false);
        }
        else if (eUnits == null)
        {
            // player win;
            BattleOver(true);
        }
        else
        {
            if (battleUnits[currentUnit].Base.IsEnemy)
            {
                StartCoroutine(EnemySkill());
            }
            else
            {
                PlayerAction();
            }
        }

    }
    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(battleDialog.TypeDialog("행동을 선택하세요."));
        battleDialog.EnableActionSelector(true);
        battleDialog.SetSkillNames(battleUnits[currentUnit].Skills);
    }
    void PlayerSkill()
    {
        state = BattleState.PlayerSkill;
        battleDialog.EnableSkillSelector(true);
        battleDialog.EnableDialogText(false);
    }
    void EnemySelect()
    {
        state = BattleState.EnemySelect;
        battleDialog.EnableEnemySelector(true);
        battleDialog.EnableEnemyInfo(true);
        battleDialog.EnableDialogText(false);
    }

    IEnumerator PerformPlayerSkill()
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog
            ($"{battleUnits[currentUnit].Base.Name}이 {enemyUnits[currentEnemy].Base.Name}에게 {battleUnits[currentUnit].Skills[currentSkill].Base.Name}을 사용합니다.");

        // attack animation
        // hit animation
        enemyHud.unitHudElements[currentEnemy].PlayHitAnimaion();
        yield return new WaitForSeconds(1f);

        var damageDetails = enemyUnits[currentEnemy].TakeDamage(battleUnits[currentUnit].Skills[currentSkill], battleUnits[currentUnit]);
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 {damageDetails.Damage}의 피해를 줬습니다. ");
        yield return enemyHud.unitHudElements[currentEnemy].UpdateHP();

        if (damageDetails.Fainted)
        {
            yield return battleDialog.TypeDialog($"{enemyUnits[currentEnemy].Base.Name}가 쓰러졌습니다.");
        }

        if (currentUnit < battleUnits.Count - 1)
        {
            currentUnit++;
            CheckEnemy();
        }
        else
        {
            currentUnit = 0;
            CheckEnemy();
        }
    }
    IEnumerator PerformPlayerAttack()
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이 {enemyUnits[currentEnemy].Base.Name}을 공격합니다.");
        yield return new WaitForSeconds(1f);

        enemyHud.unitHudElements[currentEnemy].PlayHitAnimaion();
        yield return new WaitForSeconds(1f);

        var damageDetails = enemyUnits[currentEnemy].TakeDamage(battleUnits[currentUnit].PhysicsAttack, battleUnits[currentUnit]);
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 {damageDetails.Damage}의 피해를 줬습니다. ");
        yield return enemyHud.unitHudElements[currentEnemy].UpdateHP();

        if(damageDetails.Fainted)
        {
            enemyHud.unitHudElements[currentEnemy].PlayFaintedAnimation();
            yield return battleDialog.TypeDialog($"{enemyUnits[currentEnemy].Base.Name}이(가) 쓰러졌습니다.");
        }

        if (currentUnit < battleUnits.Count - 1)
        {
            currentUnit++;
            CheckEnemy();
        }
        else
        {
            currentUnit = 0;
            CheckEnemy();
        }
    }
    IEnumerator EnemySkill()
    {
        state = BattleState.Busy;

        var unit = battleUnits[currentUnit].GetRandomUnit(playerParty.Units);
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이 {unit.Base.Name}을 공격합니다.");
        yield return new WaitForSeconds(1f);

        enemyHud.unitHudElements.Where(x => x.Unit == battleUnits[currentUnit]).FirstOrDefault().PlayAttackAnimation();
        var damageDetails = unit.TakeDamage(battleUnits[currentUnit].Skills[0], battleUnits[currentUnit]);
        yield return playerHud.unitHudElements.Where(x => x.Unit == unit).FirstOrDefault().UpdateHP();
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {unit.Base.Name}에게 {damageDetails.Damage}의 피해를 줬습니다. ");

        yield return new WaitForSeconds(1f);

        if(damageDetails.Fainted)
        {
            
            yield return battleDialog.TypeDialog($"{unit.Base.Name}이(가) 쓰러졌습니다.");
        }

        if(currentUnit < battleUnits.Count - 1)
        {
            currentUnit++;
            CheckEnemy();
        }
        else
        {
            currentUnit = 0;
            CheckEnemy();
        }

    }

    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerSkill)
        {
            HandleSkillSelection();
        }
        else if(state == BattleState.EnemySelect)
        {
            HandleEnemySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 4)
            {
                ++currentAction;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }

        battleDialog.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                EnemySelect();
                //fight
            }
            else if (currentAction == 1)
            {
                //Defense
            }
            else if(currentAction == 2)
            {
                PlayerSkill();
                //color
            }
            else if(currentAction == 3)
            {
                //tool
            }
            else if(currentAction == 4)
            {
                //run
            }
        }
    }


    private void HandleSkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentSkill < playerParty.Units[currentUnit].Skills.Count - 1)
            {
                ++currentSkill;
            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentSkill > 0)
            {
                --currentSkill;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentSkill < playerParty.Units[currentUnit].Skills.Count - 2)
            {
                currentSkill += 2;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentSkill > 1)
            {
                currentSkill -= 2;
            }
        }

        battleDialog.UpdateSkillSelection(currentSkill, playerParty.Units[currentUnit].Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            battleDialog.EnableSkillSelector(false);
            EnemySelect();
        }
    }
    private void HandleEnemySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentEnemy < enemyUnits.Count - 1)
            {
                ++currentEnemy;
            }

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentEnemy > 0)
            {
                --currentEnemy;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentEnemy < enemyUnits.Count - 2)
            {
                currentEnemy += 2;
            }

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentEnemy > 1)
            {
                currentEnemy -= 2;
            }
        }
        battleDialog.UpdateEnemySelection(currentEnemy, enemyUnits[currentEnemy]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            battleDialog.EnableEnemySelector(false);
            battleDialog.EnableEnemyInfo(false);
            battleDialog.EnableDialogText(true);
            if(currentAction == 0)
            {
                StartCoroutine(PerformPlayerAttack());
            }
            else if(currentAction == 2)
            {
                StartCoroutine(PerformPlayerSkill());
            }
            
        }
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

}
