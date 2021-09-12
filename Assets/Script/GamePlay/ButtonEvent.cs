using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour,IPointerClickHandler
{
    public Object hero;
    public SceneAsset Scene;
    public enum ButtonType
    {
        GameStart,

    }

    public ButtonType buttonType;
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SceneManager.LoadScene(Scene.name);
        Instantiate(hero);
    }
}
