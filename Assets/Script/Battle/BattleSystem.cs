using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Assertions.Must;
using UnityEditor.Connect;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, EnemySelect, EnemySkill, Busy, PlayerDefanse, SkillToForget, BattleOver, Typing
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleDialog battleDialog;

    [SerializeField] UnitHud playerHud;
    [SerializeField] UnitHud enemyHud;
    [SerializeField] SkillSelectionUI skillSelectionUI;
    public event Action<bool> OnBattleOver;
    BattleState state;
    int currentAction;
    public int currentSkill;
    int currentUnit;
    int currentEnemy;
    bool isTrainerBattle = false;
    PlayerActions player;
    TrainerController trainer;

    int escapeAttempts;
    SkillBase skillToLearn;
    Unit unitToLearn;

    UnitParty playerParty;
    List<Unit> enemyUnits;
    //List<Unit> battleUnits = new List<Unit>();
    List<BattleUnit> battleUnits;
    List<BattleUnit> playerBattleUnits;
    List<BattleUnit> enemyBattleUnits;

    BattleState prevState;
    private void Awake()
    {
        battleDialog.ChangeState += () =>
        {
            state = BattleState.PlayerAction;
        };
        battleDialog.StartTyping += () =>
        {
            prevState = state;
            state = BattleState.Typing;
        };
        battleDialog.EndTyping += () =>
        {
            state = prevState;
        };
    }
    public void StartBattle(UnitParty playerParty, List<Unit> enemyUnits)
    {
        currentUnit = 0;
        this.playerParty = playerParty;
        this.enemyUnits = enemyUnits;
        isTrainerBattle = false;
        //battleUnits.AddRange(enemyUnits);
        //battleUnits.AddRange(playerParty.Units);

        //battleUnits = battleUnits.OrderByDescending(unit => unit.Base.Speed).ThenBy(unit => unit.Base.IsEnemy).ToList();

        StartCoroutine(SetupBattle());
    }
    public void StartTrainerBattle(UnitParty playerParty, UnitParty trainerParty)
    {
        currentUnit = 0;
        this.playerParty = playerParty;
        this.enemyUnits = trainerParty.Units;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerActions>();
        trainer = trainerParty.GetComponent<TrainerController>();
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        state = BattleState.Busy;
        playerBattleUnits = new List<BattleUnit>();
        enemyBattleUnits = new List<BattleUnit>();
        battleUnits = new List<BattleUnit>();
        for (int i = 0; i < playerHud.unitHudElements.Count; i++)
        {
            if(i < playerParty.Units.Count)
            {
                //playerHud.unitHudElements[i].SetData(playerParty.Units[i]);
                playerHud.unitHudElements[i].gameObject.SetActive(true);
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
                enemyHud.unitHudElements[i].gameObject.SetActive(true);
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

        if(isTrainerBattle)
        {
            yield return battleDialog.TypeDialog($"{trainer.Name}이(가) 싸움을 걸어왔다!");
        }
        else
        {
            yield return battleDialog.TypeDialog("정적을 갉아먹고 적이 나타났다.");
        }
        escapeAttempts = 0;
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
        state = BattleState.Busy;
        currentAction = 0;
        currentSkill = 0;
        
        battleDialog.EnableDialogText(true);
        StartCoroutine(battleDialog.TypeDialog2("어떤 행동을 취할 것인가?"));
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
    IEnumerator ChooseSkillToForget(Unit unit, SkillBase newSkill)
    {
        state = BattleState.Busy;
        yield return battleDialog.TypeDialog($"없애고 싶은 색채 스킬을 선택하세요.");
        skillSelectionUI.gameObject.SetActive(true);
        skillSelectionUI.SetSkillData(unit.Skills.Select(x => x.Base).ToList(), newSkill);
        skillToLearn = newSkill;
        state = BattleState.SkillToForget;
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
            //yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) 자신에게 {skill.Base.Name}을(를) 사용.");
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
            yield return HandleUnitFainted(targetUnit);
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
                    yield return HandleUnitFainted(targetUnit);
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
        yield return battleDialog.TypeDialog($"{sourceUnit.unit.Base.Name}이(가) {targetUnit.unit.Base.Name}에게 {damageDetails.Damage}의 피해를 입혔다.");
        yield return targetUnit.Hud.UpdateHP();

        if (targetUnit.unit.HP <= 0)
        {
            yield return HandleUnitFainted(targetUnit);
        }
        GotoNext();
    }
    IEnumerator RunSkillEffects(Skill skill, Unit source, Unit target)
    {
        state = BattleState.Busy;
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
        state = BattleState.Busy;
        while (unit.StateChanges.Count > 0)
        {
            var message = unit.StateChanges.Dequeue();
            yield return battleDialog.TypeDialog(message);
        }
    }
    IEnumerator HandleUnitFainted(BattleUnit faintedUnit)
    {
        yield return battleDialog.TypeDialog($"{faintedUnit.unit.Base.Name}이(가) 쓰러졌다!");
        yield return new WaitForSeconds(1f);

        if(faintedUnit.IsEnemyUnit)
        {
            // Exp Gain
            int expYield = faintedUnit.unit.Base.ExpYield;
            int enemyLevel = faintedUnit.unit.Level;

            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);

            foreach(var i in playerBattleUnits)
            {
                i.unit.IncreaseExp(expGain);
                yield return battleDialog.TypeDialog($"{i.unit.Base.Name}이(가) {expGain}의 경험치를 얻었습니다.");
                yield return i.Hud.SetExpSmooth();

                // Check Level Up
                while (i.unit.CheckForLevelUp())
                {
                    yield return battleDialog.TypeDialog($"{i.unit.Base.Name}의 레벨이 {i.unit.Level}(으)로 올랐습니다! ");

                    // Try to learn a new move
                    var newSkill = i.unit.GetLearnableSkillAtCurLevel();

                    if(newSkill != null)
                    {
                        if(i.unit.Skills.Count < UnitBase.MaxNumOfSKills)
                        {
                            i.unit.LearnSkill(newSkill);
                            yield return battleDialog.TypeDialog($"{i.unit.Base.Name}(이)가 {newSkill.Base.Name}(을)를 배웠습니다! ");
                            battleDialog.SetSkillNames2(battleUnits[currentUnit].unit.Skills, battleUnits[currentUnit].unit);
                        }
                        else
                        {
                            unitToLearn = i.unit;
                            yield return battleDialog.TypeDialog($"{i.unit.Base.Name}(이)가 {newSkill.Base.Name}(을)를 배우려고 합니다. ");
                            yield return battleDialog.TypeDialog($"하지만 {UnitBase.MaxNumOfSKills}개 이상의 기술을 가질 수는 없습니다. ");
                            yield return ChooseSkillToForget(i.unit, newSkill.Base);
                            yield return new WaitUntil(() => state != BattleState.SkillToForget);
                            yield return new WaitForSeconds(1f);
                        }
                    }
                    yield return i.Hud.SetExpSmooth(true);
                }
            }
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
        state = BattleState.Busy;
        battleDialog.EnableEnemySelector(false);
        battleDialog.EnableEnemyInfo(false);
        battleDialog.EnableDialogText(true);
        yield return battleDialog.TypeDialog("해당 유닛은 이미 처치했습니다.");
        //UnityEngine.Debug.Log("해당 유닛은 이미 처치했습니다.");
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
        else if(state == BattleState.SkillToForget)
        {
            Action<int> onSkillSelected = (skillIndex) =>
            {
                skillSelectionUI.gameObject.SetActive(false);
                if(skillIndex == UnitBase.MaxNumOfSKills)
                {
                    StartCoroutine(battleDialog.TypeDialog($"{unitToLearn.Base.Name}은 {skillToLearn.Name}(을)를 배우지 않았습니다."));
                }
                else
                {
                    UnityEngine.Debug.Log(unitToLearn.Skills[skillIndex].Base);
                    var selectedSkill = unitToLearn.Skills[skillIndex].Base;
                    StartCoroutine(battleDialog.TypeDialog($"{unitToLearn.Base.Name}이(가) {selectedSkill.Name}대신 {skillToLearn.Name}(을)를 배웠습니다."));
                    unitToLearn.Skills[skillIndex] = new Skill(skillToLearn);
                }
                skillToLearn = null;
                unitToLearn = null;
                state = BattleState.Busy;
            };
            skillSelectionUI.HandleSkillSelection(onSkillSelected);
        }
        else if(state == BattleState.Typing)
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                battleDialog.StopTyping();
            }
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
                StartCoroutine(TryToEscape());
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
        currentSkill = Mathf.Clamp(currentSkill, 0, battleUnits[currentUnit].unit.Skills.Count - 1);

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
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if(isTrainerBattle)
        {
            yield return battleDialog.TypeDialog($"도망 못감 ㅅㄱ");
            GotoNext();
            yield break;
        }
        int playerSpeed = playerParty.Units.Sum(x => x.Base.Speed);
        int enemySpeed = enemyUnits.Sum(x => x.Base.Speed);

        escapeAttempts++;

        if(enemySpeed < playerSpeed)
        {
            yield return battleDialog.TypeDialog($"무사히 도망쳤다!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if(UnityEngine.Random.Range(0,256) < f)
            {
                yield return battleDialog.TypeDialog($"무사히 도망쳤다!");
                BattleOver(true);
            }
            else
            {
                yield return battleDialog.TypeDialog($"도망 못감 ㅅㄱ");
                GotoNext();
                yield break;
            }
        }
    }
}
