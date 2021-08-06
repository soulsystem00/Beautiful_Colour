using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    enum Input_type
    {
        Horizontal,
        Vertical,

    }

    public bool hDown { get; set; }
    public bool vDown { get; set; }
    public bool hUp { get; set; }
    public bool vUp { get; set; }

    public float hRaw { get; set; }
    public float vRaw { get; set; }
    public bool actionDown { get; set; }

    public bool menuactive { get; set; }

    public bool DownArrow { get; set; }
    public bool UpArrow { get; set; }
    public bool EnterDown { get; set; }
    public bool escapeDown { get; set; }

    public GameObject menu;

    public UnityAction MenuFunction;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hDown = Input.GetButtonDown("Horizontal");
        vDown = Input.GetButtonDown("Vertical");
        hUp = Input.GetButtonUp("Horizontal");
        vUp = Input.GetButtonUp("Vertical");

        hRaw = Input.GetAxisRaw("Horizontal");
        vRaw = Input.GetAxisRaw("Vertical");

        actionDown = Input.GetKeyDown(KeyCode.Tab);

        DownArrow = Input.GetKeyDown(KeyCode.DownArrow);
        UpArrow = Input.GetKeyDown(KeyCode.UpArrow);
        EnterDown = Input.GetKeyDown(KeyCode.Return);
        escapeDown = Input.GetKeyDown(KeyCode.Escape);

        if (actionDown && SceneManager.GetActiveScene().buildIndex != 0)
        {
            menu.SetActive(!menu.activeSelf);
            menuactive = true;
        }

        if(EnterDown && menuactive)
        {
            MenuFunction.Invoke();
        }
    }

}
