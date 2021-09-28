using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalMenu : MonoBehaviour
{
    //public PlayerInput playerinput;
    public GameObject menu;
    public Colour colour;
    public GameObject player;
    List<Text> menuItems;
    public int selectedItem = 0;
    // Start is called before the first frame update
    void Start()
    {
        menuItems = GetComponentsInChildren<Text>().ToList();
        //playerinput.tele += Menufunction;
    }

    // Update is called once per frame
    void Update()
    {
        int prevSelection = selectedItem;

        //if (playerinput.DownArrow)
        //    selectedItem++;
        //else if (playerinput.UpArrow)
        //    selectedItem--;


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


    void Menufunction()
    {
        if(selectedItem == 0 && colour.Character == Colour.character.화이)
        {
            colour.teleport(player);
        }
        else if(selectedItem == 0 && colour.Character == Colour.character.이매)
        {
            colour.function();
        }
        gameObject.SetActive(false);
        menu.SetActive(false);
    }
    //private void OnEnable()
    //{
    //    playerinput.state = PlayerInput.State.Additional;
    //}

    //private void OnDisable()
    //{
    //    playerinput.state = PlayerInput.State.Move;
    //}
}
