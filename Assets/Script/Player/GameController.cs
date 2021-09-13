using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState { FreeRome, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerActions playerActions;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    public GameState state;
    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerActions.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle(playerActions.GetComponent<UnitParty>(), FindObjectOfType<MapArea>().GetComponent<MapArea>().GetWildUnit());
    }

    void EndBattle(bool won)
    {
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
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }

    }

    void setState()
    {

    }

}
