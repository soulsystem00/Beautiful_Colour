using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour, IPointerClickHandler
{
    public Object hero;
    public enum ButtonType
    {
        GameStart,

    }

    public ButtonType buttonType;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(buttonType == ButtonType.GameStart)
        {
            SceneManager.LoadScene("First_island_BaseCamp_Outside");
            Instantiate(hero);
        }
    }
}
