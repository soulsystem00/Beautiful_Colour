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
    public int currentSkill;
    int currentUnit;
    int currentEnemy;

    UnitParty playerParty;
    List<Unit> enemyUnits;
    //List<Unit> battleUnits = new List<Unit>();
    List<BattleUnit> battleUnits = new List<BattleUnit>();
    List<BattleUnit> playerBattleUnits = new List<BattleUnit>();
    List<BattleUnit> enemyBattleUnits = new List<BattleUnit>();
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
                var unit = new BattleUnit(playerParty.Units[i], playerHud.unitHudElements[i]);
                battleUnits.Add(unit);
                playerBattleUnits.Add(unit);
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
                var unit = new BattleUnit(enemyUnits[i], enemyHud.unitHudElements[i]);
                battleUnits.Add(unit);
                enemyBattleUnits.Add(unit);
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
        currentAction = 0;
        currentSkill = 0;
        state = BattleState.PlayerAction;
        battleDialog.EnableDialogText(true);
        StartCoroutine(battleDialog.TypeDialog("어떤 행동을 취할 것인가?"));
        battleDialog.EnableActionSelector(true);
        //battleDialog.SetSkillNames(battleUnits[currentUnit].unit.Skills);
        battleDialog.SetSkillNames2(battleUnits[currentUnit].unit.Skills, battleUnits[currentUnit].unit);
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
        //battleDialog.EnableSkillSelector(true);
        //battleDialog.EnableDialogText(false);

        battleDialog.EnableSkillInfo(true);
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
        yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == enemyUnits[currentEnemy]).FirstOrDefault(), battleUnits[currentUnit].unit.Skills[currentSkill]);

    }
    IEnumerator PerformPlayerWideSkill(List<BattleUnit> units)
    {
        state = BattleState.Busy;
        yield return RunAttack(battleUnits[currentUnit], units, battleUnits[currentUnit].unit.Skills[currentSkill]);
    }
    IEnumerator PerformPlayerAttack()
    {
        state = BattleState.Busy;
        yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == enemyUnits[currentEnemy]).FirstOrDefault(), battleUnits[currentUnit].unit.PhysicsAttack);

    }
    IEnumerator EnemySkill()
    {
        state = BattleState.Busy;
        var unit = battleUnits[currentUnit].unit.GetRandomUnit(playerParty.Units);
        var skill = battleUnits[currentUnit].unit.Skills[0];

        if(skill.Base.SkillTarget == SkillTarget.상대광역)
        {
            yield return RunAttack(battleUnits[currentUnit], playerBattleUnits, skill);
        }
        else if(skill.Base.SkillTarget == SkillTarget.아군광역)
        {
            yield return RunAttack(battleUnits[currentUnit], enemyBattleUnits, skill);
        }
        else
        {
            yield return RunAttack(battleUnits[currentUnit], battleUnits.Where(x => x.unit == playerParty.Units[unit]).FirstOrDefault(), skill);
        }
        
    }
    IEnumerator RunAttack(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {
        state = BattleState.Busy;

        if (skill.Base.SkillCategoty == SkillCategory.버프)
        {
            yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) 자신에게 {skill.Base.Name}을(를) 사용.");
            sourceUnit.unit.energy -= skill.PP;
            yield return sourceUnit.Hud.UpdateHP();
            yield return RunSkillEffects(skill, sourceUnit.unit, targetUnit.unit);
        }
        else if(skill.Base.SkillCategoty == SkillCategory.물리공격 || skill.Base.SkillCategoty == SkillCategory.마법공격)
        {
            yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {skill.Base.Name}을(를) 사용.");
            sourceUnit.unit.energy -= skill.PP;
            yield return sourceUnit.Hud.UpdateHP();
            if (sourceUnit.unit.Base.IsEnemy)
                sourceUnit.Hud.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.Hud.PlayHitAnimaion();

            var damageDetails = targetUnit.unit.TakeDamage(skill, sourceUnit.unit);
            yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {damageDetails.Damage}의 피해를 입혔다.");
            yield return targetUnit.Hud.UpdateHP();
        }
        if (targetUnit.unit.HP <= 0)
        {
            yield return battleDialog.TypeDialog($"{targetUnit.unit.Base.Name}이(가) 쓰러졌다!");
        }
        GotoNext();
    }
    IEnumerator RunAttack(BattleUnit sourceUnit, List<BattleUnit> targetUnits, Skill skill)
    {
        state = BattleState.Busy;

        if (skill.Base.SkillCategoty == SkillCategory.버프)
        {
            foreach(var targetUnit in targetUnits)
            {
                string targetStr = (skill.Base.SkillTarget == SkillTarget.상대광역) ? "적군" : "아군";
                yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetStr}에게 {skill.Base.Name}을(를) 사용.");
                sourceUnit.unit.energy -= skill.PP;
                yield return sourceUnit.Hud.UpdateHP();
                yield return RunSkillEffects(skill, sourceUnit.unit, targetUnit.unit);
            }
        }
        else if (skill.Base.SkillCategoty == SkillCategory.물리공격 || skill.Base.SkillCategoty == SkillCategory.마법공격)
        {
            string targetStr = (skill.Base.SkillTarget == SkillTarget.상대광역) ? "적군" : "아군";
            yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetStr}에게 {skill.Base.Name}을(를) 사용.");
            sourceUnit.unit.energy -= skill.PP;
            yield return sourceUnit.Hud.UpdateHP();

            if (sourceUnit.unit.Base.IsEnemy)
                sourceUnit.Hud.PlayAttackAnimation();

            DamageDetails damageDetails = new DamageDetails();
            foreach (var targetUnit in targetUnits)
            {
                yield return new WaitForEndOfFrame();
                targetUnit.Hud.PlayHitAnimaion();
                damageDetails = targetUnit.unit.TakeDamage(skill, sourceUnit.unit);
                
                yield return targetUnit.Hud.UpdateHP();

                if (targetUnit.unit.HP <= 0)
                {
                    yield return battleDialog.TypeDialog($"{targetUnit.unit.Base.Name}이(가) 쓰러졌다!");
                }
            }
        }
        GotoNext();
    }
    IEnumerator RunAttack(BattleUnit sourceUnit, BattleUnit targetUnit, int damage)
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {enemyUnits[currentEnemy].Base.Name}에게 일반 공격을 사용.");

        targetUnit.Hud.PlayHitAnimaion();
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
                yield return battleDialog.TypeDialog($"{source.Base.Name}이(가) 자신에게 {skill.Base.Name}을(를) 사용.");
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                yield return battleDialog.TypeDialog($"{source.Base.Name}이(가) {target.Base.Name}에게 {skill.Base.Name}을(를) 사용.");
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
    IEnumerator ShowEnemyDeadMsg()
    {
        battleDialog.EnableEnemySelector(false);
        battleDialog.EnableEnemyInfo(false);
        battleDialog.EnableDialogText(true);
        yield return battleDialog.TypeDialog("해당 유닛은 이미 처치했습니다.");
        UnityEngine.Debug.Log("해당 유닛은 이미 처치했습니다.");
        EnemySelect();
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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSkill += 1;

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSkill -= 1;
        }
        currentSkill = Mathf.Clamp(currentSkill, 0, battleUnits[currentUnit].unit.Skills.Count - 2);

        //battleDialog.UpdateSkillSelection(currentSkill, battleUnits[currentUnit].unit.Skills[currentSkill]);

        battleDialog.UpdateSkillSelection2(currentSkill, battleUnits[currentUnit].unit.Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (battleUnits[currentUnit].unit.energy < battleUnits[currentUnit].unit.Skills[currentSkill].PP) return;
            //battleDialog.EnableSkillSelector(false);
            battleDialog.EnableSkillInfo(false);
            if (battleUnits[currentUnit].unit.Skills[currentSkill].Base.SkillTarget == SkillTarget.자신)
            {
                currentEnemy = 0;
                StartCoroutine(PerformPlayerSkill());
                return;
            }
            else if(battleUnits[currentUnit].unit.Skills[currentSkill].Base.SkillTarget == SkillTarget.상대광역)
            {
                currentEnemy = 0;
                StartCoroutine(PerformPlayerWideSkill(enemyBattleUnits));
                return;
            }
            else if(battleUnits[currentUnit].unit.Skills[currentSkill].Base.SkillTarget == SkillTarget.아군광역)
            {
                currentEnemy = 0;
                StartCoroutine(PerformPlayerWideSkill(playerBattleUnits));
                return;
            }
            EnemySelect();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            //battleDialog.EnableSkillSelector(false);
            battleDialog.EnableSkillInfo(false);
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
            StartCoroutine(ShowEnemyDeadMsg());
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
