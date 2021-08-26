using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public GameObject textui;

    public GameObject UI;
    // Start is called before the first frame update
    void Start()
    {
        textui = GameObject.Find("GameManager").GetComponent<GameManager>().TextUI;
    }

    // Update is called once per frame
    void Update()
    {
        UI.SetActive(textui.activeSelf);
    }
}
