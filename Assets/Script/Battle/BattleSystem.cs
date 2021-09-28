using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, EnemySelect, EnemySkill, Busy, PlayerDefanse, BattleOver
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
    //List<Unit> battleUnits = new List<Unit>();
    List<BattleUnit> battleUnits = new List<BattleUnit>();
    // Start is called before the first frame update
    public void StartBattle(UnitParty playerParty, List<Unit> enemyUnits)
    {
        currentUnit = 0;
        this.playerParty = playerParty;
        this.enemyUnits = enemyUnits;

        //battleUnits.AddRange(enemyUnits);
        //battleUnits.AddRange(playerParty.Units);

        //battleUnits = battleUnits.OrderByDescending(unit => unit.Base.Speed).ThenBy(unit => unit.Base.IsEnemy).ToList();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        for (int i = 0; i < playerHud.unitHudElements.Count; i++)
        {
            if(i < playerParty.Units.Count)
            {
                //playerHud.unitHudElements[i].SetData(playerParty.Units[i]);
                battleUnits.Add(new BattleUnit(playerParty.Units[i], playerHud.unitHudElements[i]));
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
                //enemyHud.unitHudElements[i].SetData(enemyUnits[i]);
                battleUnits.Add(new BattleUnit(enemyUnits[i], enemyHud.unitHudElements[i]));
            }
            else
            {
                enemyHud.unitHudElements[i].gameObject.SetActive(false);
            }
        }

        battleUnits = battleUnits.OrderByDescending(battleunit => battleunit.unit.Speed).ThenBy(battleunit => battleunit.unit.Base.IsEnemy).ToList();
        battleDialog.SetEnemyNames(enemyUnits);

        yield return battleDialog.TypeDialog("정적을 갉아먹고 적이 나타났다.");

        CheckEnemy();
    }
    void CheckEnemy()
    {
        var pUnits = playerParty.Units.Where(x => x.HP > 0).FirstOrDefault();
        var eUnits = enemyUnits.Where(x => x.HP > 0).FirstOrDefault();

        while (battleUnits[currentUnit].unit.HP <= 0)
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
            if (battleUnits[currentUnit].unit.Base.IsEnemy)
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
        battleDialog.EnableDialogText(true);
        StartCoroutine(battleDialog.TypeDialog("어떤 행동을 취할 것인가?"));
        battleDialog.EnableActionSelector(true);
        battleDialog.SetSkillNames(battleUnits[currentUnit].unit.Skills);
    }
    void PlayerDefanse()
    {
        state = BattleState.PlayerDefanse;
        battleDialog.EnableDialogText(true);
        battleDialog.EnableSkillSelector(false);
        battleDialog.EnableEnemySelector(false);
        battleDialog.EnableEnemyInfo(false);
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

        // todo
        // 1. 적은 선택 되어있음 current enemy
        // 2. 스킬도 선택 되어있음 currentskill
        // 3. run attack 발동

        yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == enemyUnits[currentEnemy]).FirstOrDefault(), battleUnits[currentUnit].unit.Skills[currentSkill]);

        /* yield return battleDialog.TypeDialog
            ($"{battleUnits[currentUnit].unit.Base.Name}이 {enemyUnits[currentEnemy].Base.Name}에게 {battleUnits[currentUnit].Skills[currentSkill].Base.Name}을 사용.");

        // attack animation
        // hit animation
        /*enemyHud.unitHudElements[currentEnemy].PlayHitAnimaion();
        yield return new WaitForSeconds(1f);

        var damageDetails = enemyUnits[currentEnemy].TakeDamage(battleUnits[currentUnit].Skills[currentSkill], battleUnits[currentUnit]);
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 {damageDetails.Damage}의 피해를 입혔다. ");
        yield return enemyHud.unitHudElements[currentEnemy].UpdateHP();

        if (damageDetails.Fainted)
        {
            yield return battleDialog.TypeDialog($"{enemyUnits[currentEnemy].Base.Name}가 쓰러졌다!");
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
        }*/
    }
    IEnumerator PerformPlayerAttack()
    {
        state = BattleState.Busy;

        // todo
        // 1. 적은 선택 되어있음 current enemy
        // 2. 스킬도 선택 되어있음 currentskill
        // 3. run attack 발동

        yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == enemyUnits[currentEnemy]).FirstOrDefault(), battleUnits[currentUnit].unit.PhysicsAttack);

        //state = BattleState.Busy;
        //yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이 {enemyUnits[currentEnemy].Base.Name}을 공격.");
        //yield return new WaitForSeconds(1f);

        //enemyHud.unitHudElements[currentEnemy].PlayHitAnimaion();
        //yield return new WaitForSeconds(1f);

        //var damageDetails = enemyUnits[currentEnemy].TakeDamage(battleUnits[currentUnit].unit.PhysicsAttack, battleUnits[currentUnit]);
        //yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 {damageDetails.Damage}의 피해를 주었다. ");
        //yield return enemyHud.unitHudElements[currentEnemy].UpdateHP();

        //if(damageDetails.Fainted)
        //{
        //    enemyHud.unitHudElements[currentEnemy].PlayFaintedAnimation();
        //    yield return battleDialog.TypeDialog($"{enemyUnits[currentEnemy].Base.Name}이(가) 쓰러졌다!");
        //}

        //GotoNext();
    }
    IEnumerator EnemySkill()
    {
        state = BattleState.Busy;
        // todo
        // 적이 공격할 때
        // 1. 아군 유닛 선택
        // 2. 공격 스킬 선택 (일단은 0번 스킬로)
        // 3. runattack 발동

        

        var unit = battleUnits[currentUnit].unit.GetRandomUnit(playerParty.Units);
        var skill = battleUnits[currentUnit].unit.Skills[0];

        yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == playerParty.Units[unit]).FirstOrDefault(), skill);

        /*yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이 {unit.Base.Name}을 공격.");
        yield return new WaitForSeconds(1f);

        enemyHud.unitHudElements.Where(x => x.Unit == battleUnits[currentUnit]).FirstOrDefault().PlayAttackAnimation();
        var damageDetails = unit.TakeDamage(battleUnits[currentUnit].Skills[0], battleUnits[currentUnit]);
        yield return playerHud.unitHudElements.Where(x => x.Unit == unit).FirstOrDefault().UpdateHP();
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].Base.Name}이(가) {unit.Base.Name}에게 {damageDetails.Damage}의 피해를 주었다. ");

        yield return new WaitForSeconds(1f);

        if(damageDetails.Fainted)
        {
            yield return battleDialog.TypeDialog($"{unit.Base.Name}이(가) 쓰러졌다!");
        }

        GotoNext();*/

    }

    IEnumerator RunAttack(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {
        state = BattleState.Busy;


        yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {skill.Base.Name}을(를) 사용.");

        sourceUnit.unit.energy -= skill.PP;
        yield return sourceUnit.Hud.UpdateHP();
        // attack animation
        // hit animation

        if (sourceUnit.unit.Base.IsEnemy)
            sourceUnit.Hud.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        targetUnit.Hud.PlayHitAnimaion();

        if (skill.Base.SkillCategoty == SkillCategory.버프)
        {
            yield return RunSkillEffects(skill, sourceUnit.unit, targetUnit.unit);
        }
        else
        {
            var damageDetails = targetUnit.unit.TakeDamage(skill, sourceUnit.unit);
            yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {damageDetails.Damage}의 피해를 입혔다. ");
            yield return targetUnit.Hud.UpdateHP();
        }
        if (targetUnit.unit.HP <= 0)
        {
            yield return battleDialog.TypeDialog($"{targetUnit.unit.Base.Name}이(가) 쓰러졌다!");
        }
        GotoNext();
    }
    IEnumerator RunAttack(BattleUnit sourceUnit, BattleUnit targetUnit, int damage)
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 일반 공격을 사용.");

        // attack animation
        // hit animation
        targetUnit.Hud.PlayHitAnimaion();
        //enemyHud.unitHudElements[currentEnemy].PlayHitAnimaion();
        yield return new WaitForSeconds(1f);

        var damageDetails = targetUnit.unit.TakeDamage(damage, sourceUnit.unit);
        yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {damageDetails.Damage}의 피해를 입혔다. ");
        yield return targetUnit.Hud.UpdateHP();

        if (damageDetails.Fainted)
        {
            yield return battleDialog.TypeDialog($"{targetUnit.unit.Base.Name}이(가) 쓰러졌다!");
        }
        GotoNext();
    }
    IEnumerator RunSkillEffects(Skill skill, Unit source, Unit target)
    {
        var effects = skill.Base.Effects;
        if(effects.Boosts != null)
        {
            if(skill.Base.SkillTarget == SkillTarget.자신)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator ShowStatusChanges(Unit unit)
    {
        while (unit.StateChanges.Count > 0)
        {
            var message = unit.StateChanges.Dequeue();
            yield return battleDialog.TypeDialog(message);
        }
    }
    IEnumerator RunDefanse()
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"{battleUnits[currentUnit].unit.Base.Name}의 방어력 증가");
        yield return RunSkillEffects(battleUnits[currentUnit].unit.Skills[battleUnits[currentUnit].unit.Skills.Count - 1], battleUnits[currentUnit].unit, battleUnits[currentUnit].unit);
        GotoNext();
    }
    void GotoNext()
    {
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
    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if(state == BattleState.PlayerDefanse)
        {
            HandleDefanseStart();
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
                PlayerDefanse();
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
                BattleOver(false);
            }
        }
    }
    void HandleDefanseStart()
    {
        StartCoroutine(RunDefanse());
    }

    private void HandleSkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
                ++currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
                --currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
                currentSkill += 2;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
                currentSkill -= 2;
        }
        currentSkill = Mathf.Clamp(currentSkill, 0, battleUnits[currentUnit].unit.Skills.Count - 2);

        battleDialog.UpdateSkillSelection(currentSkill, battleUnits[currentUnit].unit.Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (battleUnits[currentUnit].unit.energy < battleUnits[currentUnit].unit.Skills[currentSkill].PP) return;
            battleDialog.EnableSkillSelector(false);
            EnemySelect();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            battleDialog.EnableSkillSelector(false);
            PlayerAction();
        }
    }
    private void HandleEnemySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
                ++currentEnemy;

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
                --currentEnemy;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
                currentEnemy += 2;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
                currentEnemy -= 2;
        }
        currentEnemy = Mathf.Clamp(currentEnemy, 0, enemyUnits.Count - 1);
        battleDialog.UpdateEnemySelection(currentEnemy, enemyUnits[currentEnemy]);

        if (Input.GetKeyDown(KeyCode.Z) && enemyUnits[currentEnemy].HP > 0)
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
        if(Input.GetKeyDown(KeyCode.Z) && enemyUnits[currentEnemy].HP <= 0)
        {
            UnityEngine.Debug.Log("해당 유닛은 이미 처치했습니다.");
            EnemySelect();
        }
        if(Input.GetKeyDown(KeyCode.X) && currentAction == 0)
        {
            battleDialog.EnableEnemySelector(false);
            battleDialog.EnableEnemyInfo(false);
            PlayerAction();
        }
        if(Input.GetKeyDown(KeyCode.X) && currentAction == 2)
        {
            battleDialog.EnableEnemySelector(false);
            battleDialog.EnableEnemyInfo(false);
            PlayerSkill();
        }
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

}
