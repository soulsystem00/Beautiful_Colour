using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, EnemySelect, EnemySkill, Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleDialog battleDialog;

    [SerializeField] UnitHud playerHud;
    [SerializeField] UnitHud enemyHud;

    public event Action<bool> OnBattleOver;

    BattleState state;
    public int currentAction;
    public int currentSkill;
    public int currentUnit;
    public int currentEnemy;

    UnitParty playerParty;
    List<Unit> enemyUnits;
    // Start is called before the first frame update
    public void StartBattle(UnitParty playerParty, List<Unit> enemyUnits)
    {
        currentUnit = 0;
        this.playerParty = playerParty;
        this.enemyUnits = enemyUnits;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        for (int i = 0; i < playerParty.Units.Count; i++)
        {
            playerHud.unitHudElements[i].SetData(playerParty.Units[i]);
            playerHud.unitHudElements[i].SetSprite();
        }
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            enemyHud.unitHudElements[i].SetData(enemyUnits[i]);
            enemyHud.unitHudElements[i].SetSprite();
        }

        battleDialog.SetEnemyNames(enemyUnits);

        yield return battleDialog.TypeDialog("야생의 적을 만났다.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(battleDialog.TypeDialog("행동을 선택하세요."));
        battleDialog.EnableActionSelector(true);
        battleDialog.SetSkillNames(playerParty.Units[currentUnit].Skills);
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
        yield return battleDialog.TypeDialog($"{playerParty.Units[currentUnit].Base.Name}이 {enemyUnits[currentEnemy].Base.Name}에게 {playerParty.Units[currentUnit].Skills[currentSkill].Base.Name}을 사용합니다.");
        yield return new WaitForSeconds(1f);
        if (currentUnit < playerParty.Units.Count - 1)
        {
            currentUnit++;
            PlayerAction();
        }
        else
        {
            currentUnit = 0;
            StartCoroutine(EnemySkill());
        }
    }
    IEnumerator PerformPlayerAttack()
    {
        yield return battleDialog.TypeDialog($"{playerParty.Units[currentUnit].Base.Name}이 {enemyUnits[currentEnemy].Base.Name}을 공격합니다.");
        yield return new WaitForSeconds(1f);
        if(currentUnit < playerParty.Units.Count - 1)
        {
            currentUnit++;
            PlayerAction();
        }
        else
        {
            currentUnit = 0;
            StartCoroutine(EnemySkill());
        }
    }
    IEnumerator EnemySkill()
    {
        state = BattleState.EnemySkill;
        UnityEngine.Debug.Log("Enemy Turn Start");
        yield return battleDialog.TypeDialog("적이 공격 합니다.");
        yield return new WaitForSeconds(1f);
        yield return battleDialog.TypeDialog("적의 공격이 끝났습니다.");
        UnityEngine.Debug.Log("Enemy Turn End");
        PlayerAction();
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
