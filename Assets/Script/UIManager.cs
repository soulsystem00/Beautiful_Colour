using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerInput PlayerInput;
    List<Text> menuItems;
    public int selectedItem = 0;

    private void Awake()
    {
        menuItems = GetComponentsInChildren<Text>().ToList();

    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerInput.MenuFunction += () => Debug.Log(selectedItem);
    }

    // Update is called once per frame
    void Update()
    {
        int prevSelection = selectedItem;

        if (PlayerInput.DownArrow)
            selectedItem++;
        else if (PlayerInput.UpArrow)
            selectedItem--;


        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        //if(prevSelection != selectedItem)
            ChangeColor();
    }

    void ChangeColor()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].color = i == selectedItem ? Color.blue : Color.black;
        }
    }

    void MenuFunction()
    {
        
    }

}
