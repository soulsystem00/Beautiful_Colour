using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState { FreeRome, Battle, Dialog, Menu, UnitInfo, UnitInfoDetail, Paused, Cutscene }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerActions playerActions;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] UnitInfoController unitInfoController;

    GameState state;
    GameState stateBeforePause;
    public static GameController Instance { get; private set; }

    MenuController menuController;

    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        UnitDB.Init();
        SkillDB.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerActions.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerActions.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if(trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerActions));
            }
        };
        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRome;
        };

        menuController.onBack += () =>
        {
            state = GameState.FreeRome;
        };
        menuController.onMenuSelected += onMenuSelected;

        unitInfoController.onBack += () =>
        {
            state = GameState.Menu;
        };
    }
    public void PauseGame(bool pause)
    {
        if(pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;
        }
    }
    public void PauseGame2(bool pause)
    {
        if (pause)
        {
            state = GameState.Paused;
        }
        else
        {
            state = GameState.FreeRome;
        }
    }
    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle(playerActions.GetComponent<UnitParty>(), FindObjectOfType<MapArea>().GetComponent<MapArea>().GetWildUnit());
    }
    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerActions.GetComponent<UnitParty>();
        var trainerParty = trainer.GetComponent<UnitParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    void EndBattle(bool won)
    {
        if(trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }
        state = GameState.FreeRome;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        if(state == GameState.FreeRome)
        {
            playerActions.HandleUpdate();

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
            }
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if(state == GameState.UnitInfo)
        {
            unitInfoController.HandleUpdate();
        }
    }
    void onMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
        {
            // unit info
            unitInfoController.OpenScreen(playerActions.GetComponent<UnitParty>());
            state = GameState.UnitInfo;
        }
        else if(selectedItem == 1)
        {
            // inven
        }
        else if(selectedItem == 2)
        {
            // map
        }
        else if(selectedItem == 3)
        {
            // save
            state = GameState.FreeRome;
            menuController.CloseMenu();
            SavingSystem.i.Save("saveSlot1");
        }
        else if(selectedItem == 4)
        {
            // load
            stateBeforePause = GameState.FreeRome;
            menuController.CloseMenu();
            SavingSystem.i.Load("saveSlot1");
        }
    }
}
